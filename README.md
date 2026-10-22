# StoreServer

ASP.NET Core Web API (.NET 10) для существующей базы `web_store` в Microsoft SQL Server.

## Архитектура

`Controller → IService/Service → IRepository/Repository → WebStoreContext → MSSQL`.

- Контроллеры принимают DTO, вызывают сервис и выбирают HTTP-ответ.
- Сервисы содержат бизнес-логику, проверяют связанные Id и вызывают AutoMapper.
  В них нет DbContext, Entity Framework и HTTP-типов.
- Репозитории выполняют асинхронные запросы, загружают связи через Include и сохраняют изменения.
- `Mapping/MappingProfile.cs` задаёт карты чтения и записи. Идентификаторы и навигации
  не перезаписываются из DTO. PATCH изменяет только переданные непустые поля.
- DbContext, репозитории и сервисы имеют время жизни Scoped: один запрос использует
  один контекст для всех своих изменений. Между запросами контекст не разделяется.

Слои реализованы для ролей, пользователей, категорий, товаров, корзин, позиций корзин,
заказов, позиций заказов и резервов товаров. Аутентификация также использует сервис
и репозиторий. DTO чтения содержат связанные данные, краткие вложенные DTO исключают
циклы, хеши паролей не возвращаются.

## Запуск

1. Указать свою MSSQL-строку подключения `ConnectionStrings:con` в `StoreServer/appsettings.json`
   (или переменной `ConnectionStrings__con`). Используется база первой практической работы.
2. Выполнить в этой базе `StoreServer/Database/001_ProductReservations.sql`, например:

   ```powershell
   sqlcmd -S "YOUR_SERVER\SQLEXPRESS" -d web_store -E -C -b -i StoreServer/Database/001_ProductReservations.sql
   ```

   Скрипт идемпотентен: добавляет `Products.available_quantity` с исходным значением 0
   и таблицу `ProductReservations`; существующие записи не удаляет.
3. Собрать и запустить:

   ```powershell
   dotnet build StoreServer.slnx
   dotnet run --project StoreServer/StoreServer.csproj
   ```

Swagger доступен по `/swagger` в Development. SQL-скрипт уже применён к локальной
`web_store` при выполнении этой работы; повторный запуск допустим.

Установлен AutoMapper 16.2.0. При наличии лицензионного ключа он задаётся через
`AutoMapper:LicenseKey` / `AutoMapper__LicenseKey` либо `AUTOMAPPER_LICENSE_KEY`.
Условия использования и лицензирования: [официальная документация AutoMapper](https://docs.automapper.io/en/stable/License-configuration.html).

## CRUD

Коллекции: `/api/Roles`, `/api/Users`, `/api/Categories`, `/api/Products`, `/api/Carts`,
`/api/CartItems`, `/api/Orders`, `/api/OrderItems`, `/api/ProductReservations`.

| Запрос | Результат |
| --- | --- |
| `GET /api/Products?page=1&pageSize=10` | 200, массив DTO и заголовок `X-Total-Count` |
| `GET /api/Products/{id}` | 200 с DTO либо 404 |
| `POST /api/Products` | 201 с DTO и Location |
| `PUT /api/Products/{id}` | Полная замена полей записи, 204 либо 404 |
| `PATCH /api/Products/{id}` | Изменение непустых полей, 204 либо 404 |
| `DELETE /api/Products/{id}` | 204 либо 404 |

Аналогичный CRUD есть у остальных коллекций. Некорректные ссылки, пагинация,
нарушения уникальности и попытки удалить используемую запись дают 400 с ошибкой.
`null` в PATCH означает «не менять», а не «очистить поле».

## Бизнес-операция: резервирование и отмена

Ресурс - `Product.AvailableQuantity`. Сначала установить фактический остаток через
создание/полное обновление товара либо PATCH:

```http
PATCH /api/Products/1
Content-Type: application/json

{"availableQuantity": 10}
```

Id в примерах заменяются на существующие Id вашей базы.

```http
POST /api/ProductReservations
Content-Type: application/json

{"idProduct": 1, "idUser": 1, "quantity": 2}
```

`ProductReservationService.CreateAsync` проверяет положительное количество,
наличие товара и пользователя и достаточность остатка. Затем уменьшает остаток
и создаёт `ProductReservation`: кто, что, сколько и когда зарезервировал.
**Один вызов `SaveChangesAsync` сохраняет товар и событие.** Успех - 201,
недостаток товара - 400 с сообщением. Временные отметки задаются сервером в UTC.

```http
POST /api/ProductReservations/1/cancel
```

`CancelAsync` возвращает ресурс и устанавливает `CancelledAt`, также одним
`SaveChangesAsync`. Ответ - 204; при отсутствии активного события (включая повторную
отмену) - 404. DELETE резерва выполняет эту же отмену; событие остаётся в истории.

PUT/PATCH активного резерва изменяет только `quantity`, одновременно корректируя
остаток. Пользователь, товар и время события неизменяемы. Закрытые события доступны
для чтения, изменение закрытого события возвращает 404.

Репозиторий загружает обе связанные сущности одним Scoped-контекстом. EF Core
атомарно сохраняет их одним вызовом; отдельного UnitOfWork и явных транзакций в
бизнес-методах нет. Остаток и дата отмены являются concurrency token: конфликт
параллельных изменений даёт понятную ошибку 400, а не частичное сохранение или 500.

## Проверки

Проверочный консольный проект не подключает другие СУБД и не требует тестового
фреймворка. Запускать **из корня репозитория** с доступом к настроенному MSSQL:

```powershell
dotnet run --project StoreServer.Tests/StoreServer.Tests.csproj
```

Проверяются AutoMapper, создание зависимостей, реальный CRUD всех восьми исходных
сущностей с повторным чтением, вложенные DTO, BCrypt-вход, CRUD резервов, недостаток
ресурса, отмена и повторная отмена, число сохранений, атомарный откат при ошибке FK,
конкурентное изменение, HTTP-статусы, `X-Total-Count` и отмена запроса.

Тестовые строки имеют уникальные имена. Общая **тестовая** транзакция откатывается
после проверки: в рабочей базе не остаются тестовые записи и изменения остатков.
Identity-счётчики SQL Server могут продвинуться при откате; пропуски Id нормальны.
Приложение не открывает эту транзакцию: она существует только в проверочном проекте.

Полная Markdown-версия PDF: [AutoMapper и Слои](<docs/AutoMapper и Слои.md>).
Копия также сохранена рядом с PDF в `D:\Projects\RPM\ASP\Guides`.
