# AutoMapper и слоистая архитектура

Markdown-версия инструкции «AutoMapper и Слои.pdf» (12 страниц).
Сохранены все 12 шагов и учебная предметная область BooksApi. Код со скриншотов
переписан в текст; добавлены необходимые `using`, восстановлены отступы и объединены
фрагменты файлов, разорванные между страницами. Примечания к неоднозначностям
исходника и применение к StoreServer вынесены в конец.

## Шаг 1. Проверить базу данных

Открыть SSMS (или Azure Data Studio) и убедиться, что в LibraryDb физически
существуют таблицы Books, Authors, Tags, BookTags со всеми связями.
Код на этом шаге не пишем.

## Шаг 2. Создать проект (если его ещё нет)

File → New → Project → ASP.NET Core Web API, или из терминала:

```powershell
dotnet new webapi -n BooksApi
cd BooksApi
```

Если проект уже есть (свой BooksApi или p3-api с прошлых пар), этот шаг
пропускается: работаем в нём же.

## Шаг 3. Установить NuGet-пакеты

Tools → NuGet Package Manager → Package Manager Console, или из терминала
в папке проекта:

```powershell
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package AutoMapper
```

Swashbuckle (Swagger) обычно уже стоит в шаблоне Web API.

## Шаг 4. Строка подключения

Открыть `appsettings.json` в корне проекта. На скриншоте используется ключ `con`
и база `booksApi` (имя SQL Server замените своим):

```json
{
  "ConnectionStrings": {
    "con": "Data Source=YOUR_SERVER\\SQLEXPRESS;Initial Catalog=booksApi;Integrated Security=True;Trust Server Certificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## Шаг 5. Папка Models - сущности

Правой кнопкой по проекту → Add → New Folder → Models.
Затем Add → Class, по одному файлу на сущность.

`Models/Author.cs`:

```csharp
namespace BooksApi.Models;

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
```

`Models/Tag.cs`:

```csharp
namespace BooksApi.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
```

`Models/Book.cs`:

```csharp
namespace BooksApi.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int AuthorId { get; set; }
    public virtual Author Author { get; set; } = null!;
    public string? ISBN { get; set; }
    public string? Genre { get; set; }
    public int? PublishedYear { get; set; }
    public decimal Price { get; set; }
    public int CopiesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
```

Имена свойств должны совпадать с именами колонок в базе, включая регистр
(`ISBN`, а не `Isbn`). Сверьте с таблицей Books в SSMS, прежде чем идти дальше.

## Шаг 6. AppDbContext

В папке Models → Add → Class → `AppDbContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;

namespace BooksApi.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Один-ко-многим: Author (1) --- (N) Book.
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Многие-ко-многим: Book (N) --- (N) Tag.
        // Имена FK должны совпасть с существующей таблицей BookTags.
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Tags)
            .WithMany(t => t.Books)
            .UsingEntity<Dictionary<string, object>>(
                "BookTags",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                j => j.HasOne<Book>().WithMany().HasForeignKey("BookId"));

        modelBuilder.Entity<Book>()
            .Property(b => b.Price).HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name).IsUnique();
    }
}
```

## Шаг 7. Папка Dtos - пять классов

Add → New Folder → Dtos, по одному классу на DTO.

`Dtos/AuthorDto.cs`:

```csharp
namespace BooksApi.Dtos;

public class AuthorDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
}
```

`Dtos/BookDto.cs` - для чтения:

```csharp
namespace BooksApi.Dtos;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public AuthorDto Author { get; set; } = null!;
    public List<string> Tags { get; set; } = new();
    public string? ISBN { get; set; }
    public string? Genre { get; set; }
    public int? PublishedYear { get; set; }
    public decimal Price { get; set; }
    public int CopiesCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

`Dtos/CreateBookDto.cs` - для записи:

```csharp
namespace BooksApi.Dtos;

public class CreateBookDto
{
    public string Title { get; set; } = "";
    public int AuthorId { get; set; }
    public List<int> TagIds { get; set; } = new();
    public string? ISBN { get; set; }
    public string? Genre { get; set; }
    public int? PublishedYear { get; set; }
    public decimal Price { get; set; }
    public int CopiesCount { get; set; } = 1;
}
```

`Dtos/UpdateBookDto.cs` - PUT, все поля передаются при полной замене:

```csharp
namespace BooksApi.Dtos;

public class UpdateBookDto
{
    public string Title { get; set; } = "";
    public int AuthorId { get; set; }
    public List<int> TagIds { get; set; } = new();
    public string? ISBN { get; set; }
    public string? Genre { get; set; }
    public int? PublishedYear { get; set; }
    public decimal Price { get; set; }
    public int CopiesCount { get; set; }
}
```

`Dtos/PatchBookDto.cs` - PATCH, все поля nullable:

```csharp
namespace BooksApi.Dtos;

public class PatchBookDto
{
    public string? Title { get; set; }
    public int? AuthorId { get; set; }
    public List<int>? TagIds { get; set; }
    public string? ISBN { get; set; }
    public string? Genre { get; set; }
    public int? PublishedYear { get; set; }
    public decimal? Price { get; set; }
    public int? CopiesCount { get; set; }
}
```

## Шаг 8. Папка Mapping - MappingProfile

Add → New Folder → Mapping → Add → Class → `MappingProfile.cs`.
Код со скриншота на странице 5:

```csharp
using AutoMapper;
using BooksApi.Dtos;
using BooksApi.Models;

namespace BooksApi.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity → DTO (чтение).
        CreateMap<Author, AuthorDto>();
        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.Tags,
                opt => opt.MapFrom(src => src.Tags.Select(t => t.Name)));

        // DTO → Entity (запись). TagIds/AuthorId сами по себе
        // не превращаются в навигации: сервис находит их в базе.
        CreateMap<CreateBookDto, Book>()
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore());

        CreateMap<UpdateBookDto, Book>()
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore());

        // Для PATCH сервис вручную копирует только непустые поля.
    }
}
```

## Шаг 9. Папка Repositories

Add → New Folder → Repositories. Сначала интерфейс (Add → New Item → Interface,
либо Class с ключевым словом `interface`), затем реализация.

`Repositories/IBookRepository.cs` (страница 5):

```csharp
using BooksApi.Models;

namespace BooksApi.Repositories;

public interface IBookRepository
{
    Task<List<Book>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
    Task<Book?> GetByIdAsync(int id, CancellationToken ct);
    Task<Author?> FindAuthorAsync(int authorId, CancellationToken ct);
    Task<List<Tag>> FindTagsAsync(List<int> tagIds, CancellationToken ct);
    Task AddAsync(Book book);
    void Update(Book book);
    void Remove(Book book);
    Task SaveChangesAsync();
}
```

`Repositories/BookRepository.cs` (страницы 5-6):

```csharp
using BooksApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksApi.Repositories;

// Единственный слой, которому разрешено видеть AppDbContext.
// Здесь только CRUD и простые запросы, никакого HTTP.
public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Book>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Tags)
            .OrderBy(b => b.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct) =>
        await _context.Books.CountAsync(ct);

    public async Task<Book?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Tags)
            .FirstOrDefaultAsync(b => b.Id == id, ct);
    }

    public async Task<Author?> FindAuthorAsync(int authorId, CancellationToken ct) =>
        await _context.Authors.FirstOrDefaultAsync(a => a.Id == authorId, ct);

    public async Task<List<Tag>> FindTagsAsync(List<int> tagIds, CancellationToken ct) =>
        await _context.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync(ct);

    public async Task AddAsync(Book book) => await _context.Books.AddAsync(book);
    public void Update(Book book) => _context.Books.Update(book);
    public void Remove(Book book) => _context.Books.Remove(book);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
```

## Шаг 10. Папка Services

Add → New Folder → Services. Снова интерфейс, потом реализация.
Сервис ничего не знает про HTTP: ни `IActionResult`, ни статус-кодов.
Отсутствие записи выражается через `null`, а не через 404: это решает контроллер.

`Services/IBookService.cs` (страница 6):

```csharp
using BooksApi.Dtos;

namespace BooksApi.Services;

public interface IBookService
{
    Task<(List<BookDto> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, CancellationToken ct);
    Task<BookDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<BookDto> CreateAsync(CreateBookDto dto, CancellationToken ct);
    Task<BookDto?> UpdateAsync(int id, UpdateBookDto dto, CancellationToken ct);
    Task<BookDto?> PatchAsync(int id, PatchBookDto dto, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
```

`Services/BookService.cs` (страницы 7-9):

```csharp
using AutoMapper;
using BooksApi.Dtos;
using BooksApi.Models;
using BooksApi.Repositories;

namespace BooksApi.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _repository;
    private readonly IMapper _mapper;

    public BookService(IBookRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<(List<BookDto> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, CancellationToken ct)
    {
        var books = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);
        return (_mapper.Map<List<BookDto>>(books), total);
    }

    public async Task<BookDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var book = await _repository.GetByIdAsync(id, ct);
        return book is null ? null : _mapper.Map<BookDto>(book);
    }

    public async Task<BookDto> CreateAsync(CreateBookDto dto, CancellationToken ct)
    {
        _ = await _repository.FindAuthorAsync(dto.AuthorId, ct)
            ?? throw new InvalidOperationException($"Автор с Id {dto.AuthorId} не найден.");

        var book = _mapper.Map<Book>(dto);
        book.CreatedAt = DateTime.UtcNow;

        var tags = await _repository.FindTagsAsync(dto.TagIds, ct);
        foreach (var tag in tags) book.Tags.Add(tag);

        await _repository.AddAsync(book);
        await _repository.SaveChangesAsync();

        // Перечитываем с Include, чтобы в ответе был заполнен Author.
        var saved = await _repository.GetByIdAsync(book.Id, ct)
            ?? throw new InvalidOperationException("Не удалось загрузить только что созданную книгу.");
        return _mapper.Map<BookDto>(saved);
    }

    public async Task<BookDto?> UpdateAsync(int id, UpdateBookDto dto, CancellationToken ct)
    {
        var book = await _repository.GetByIdAsync(id, ct);
        if (book is null) return null;

        _ = await _repository.FindAuthorAsync(dto.AuthorId, ct)
            ?? throw new InvalidOperationException($"Автор с Id {dto.AuthorId} не найден.");

        _mapper.Map(dto, book);
        book.Tags.Clear();
        var tags = await _repository.FindTagsAsync(dto.TagIds, ct);
        foreach (var tag in tags) book.Tags.Add(tag);

        _repository.Update(book);
        await _repository.SaveChangesAsync();
        return _mapper.Map<BookDto>(book);
    }

    public async Task<BookDto?> PatchAsync(int id, PatchBookDto dto, CancellationToken ct)
    {
        var book = await _repository.GetByIdAsync(id, ct);
        if (book is null) return null;

        // Merge patch: копируем только непустые поля.
        if (dto.Title is not null) book.Title = dto.Title;
        if (dto.ISBN is not null) book.ISBN = dto.ISBN;
        if (dto.Genre is not null) book.Genre = dto.Genre;
        if (dto.PublishedYear is not null) book.PublishedYear = dto.PublishedYear.Value;
        if (dto.Price is not null) book.Price = dto.Price.Value;
        if (dto.CopiesCount is not null) book.CopiesCount = dto.CopiesCount.Value;

        if (dto.AuthorId is not null)
        {
            _ = await _repository.FindAuthorAsync(dto.AuthorId.Value, ct)
                ?? throw new InvalidOperationException($"Автор с Id {dto.AuthorId} не найден.");
            book.AuthorId = dto.AuthorId.Value;
        }

        if (dto.TagIds is not null)
        {
            book.Tags.Clear();
            var tags = await _repository.FindTagsAsync(dto.TagIds, ct);
            foreach (var tag in tags) book.Tags.Add(tag);
        }

        _repository.Update(book);
        await _repository.SaveChangesAsync();
        return _mapper.Map<BookDto>(book);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var book = await _repository.GetByIdAsync(id, ct);
        if (book is null) return false;

        _repository.Remove(book);
        await _repository.SaveChangesAsync();
        return true;
    }
}
```

## Шаг 11. Controllers - переписать BooksController

`Controllers/BooksController.cs` (страницы 10-11):

```csharp
using BooksApi.Dtos;
using BooksApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BooksApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // GET /api/books?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll(
        int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var (items, total) = await _bookService.GetAllAsync(page, pageSize, cancellationToken);
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var book = await _bookService.GetByIdAsync(id, cancellationToken);
        if (book is null) return NotFound();
        return Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var book = await _bookService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id, UpdateBookDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var book = await _bookService.UpdateAsync(id, dto, cancellationToken);
            if (book is null) return NotFound();
            return Ok(book);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(
        int id, PatchBookDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var book = await _bookService.PatchAsync(id, dto, cancellationToken);
            if (book is null) return NotFound();
            return Ok(book);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _bookService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
```

## Шаг 12. Program.cs - регистрация слоёв

Код со страницы 12:

```csharp
using Microsoft.EntityFrameworkCore;
using BooksApi.Models;
using BooksApi.Repositories;
using BooksApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Controller → Service → Repository → AppDbContext.
// Repository и Service зависят от DbContext со временем жизни Scoped,
// поэтому оба тоже Scoped, а не Singleton или Transient.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();

// Подхватывает все Profile в сборке, включая MappingProfile.
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

## Примечания к исходной инструкции

1. В шаге 1 указана база LibraryDb, а на скриншоте шага 4 - booksApi.
   Выберите свою существующую базу, не создавайте другую только ради примера.
2. В шаге 4 ключ строки подключения - `con`, а в шаге 12 - `DefaultConnection`.
   Имена должны совпадать. В StoreServer используется `con` и существующая база `web_store`.
3. Регистрация AutoMapper в PDF относится к старому API. В установленной в StoreServer
   версии 16.2.0 используется `AddAutoMapper(cfg => { ... }, typeof(MappingProfile))`.
   Лицензионный ключ при наличии задаётся конфигурацией `AutoMapper:LicenseKey`
   или переменной окружения `AUTOMAPPER_LICENSE_KEY`.
   См. [официальную документацию](https://docs.automapper.io/en/stable/15.0-Upgrade-Guide.html)
   и [настройку лицензии](https://docs.automapper.io/en/stable/License-configuration.html).
4. В профиле исходного примера не игнорируются генерируемые `Id` и `CreatedAt`.
   При строгой проверке конфигурации их следует явно игнорировать для карт записи.
5. В примере `FindTagsAsync` молча пропускает несуществующие Id тегов.
   При необходимости строгой проверки сравнивайте найденные Id с переданными.
6. PATCH в этой инструкции игнорирует `null`: он не позволяет очистить nullable-поле,
   передав JSON `null`. Это сохранённая семантика учебного примера.
7. Проверять и ограничивать `page`/`pageSize` следует до вычисления `Skip`.
8. В современном EF Core один `SaveChangesAsync` атомарно сохраняет связанные
   изменения. Отдельный UnitOfWork и явная транзакция бизнес-методу не нужны.

## Применение к StoreServer и дополнительные требования практической работы

Учебный BooksApi не добавляется в магазин. Паттерн применён к `Role`, `User`,
`Category`, `Product`, `Cart`, `CartItem`, `Order`, `OrderItem` и новой сущности
`ProductReservation`. Для каждой имеются DTO, интерфейсы и реализации репозитория
и сервиса, а также тонкий контроллер. Вход в систему также вынесен в сервис.

Связи реальной базы MSSQL описаны в EF: пользователь/роль, товар/категория,
корзина/пользователь, позиция корзины/корзина и товар, заказ/пользователь,
позиция заказа/заказ и товар, резерв/пользователь и товар.
DTO чтения содержат вложенные связанные данные, DTO записи принимают Id.
Краткие вложенные DTO предотвращают циклы сериализации. Хеши паролей не выдаются.

Репозитории и сервисы зарегистрированы как Scoped: один HTTP-запрос получает
свои экземпляры и один общий Scoped DbContext. Контекст не разделяется между
запросами. Внутри сервиса нет HTTP-типов и прямых обращений к DbContext.

Ресурс - `Product.AvailableQuantity`. Факт операции - `ProductReservation`:
товар, пользователь, количество, UTC-время создания и время отмены.

```http
POST /api/ProductReservations
Content-Type: application/json

{"idProduct": 1, "idUser": 1, "quantity": 2}
```

Сервис проверяет количество, наличие пользователя и товара, достаточность
остатка, уменьшает остаток и создаёт событие. Единственный `SaveChangesAsync`
сохраняет обе записи. Успех - 201 с Location и DTO, нехватка товара - 400
с понятным сообщением. Существующие товары получают исходный остаток 0;
реальные остатки задаются через PUT/PATCH товара.

```http
POST /api/ProductReservations/1/cancel
```

Обратная операция возвращает количество на склад и устанавливает `CancelledAt`.
Один `SaveChangesAsync` сохраняет возврат и закрытие события. Успех - 204,
отсутствующий или уже закрытый резерв - 404. DELETE резерва вызывает ту же
операцию отмены, сохраняя историю. PUT/PATCH активного резерва меняют количество
с соответствующим пересчётом остатка; пользователь, товар и время события
неизменяемы. Закрытые события доступны для чтения.

Все списки имеют пагинацию и `X-Total-Count`; GET - 200/404, POST - 201,
PUT/PATCH и DELETE существующих записей - 204, отсутствующих - 404.
Некорректные ссылки и ограничения БД возвращают 400.

SQL Server использует исходное подключение. Скрипт расширения схемы находится
в `StoreServer/Database/001_ProductReservations.sql`, допускает повторный запуск
и не удаляет существующие данные. Оптимистическая проверка остатка и состояния
резерва защищает от конкурентного резервирования и повторного возврата.

Исходный PDF не содержит бизнес-операцию резервирования: этот раздел добавлен
по уточнённому заданию пользователя и отделён от переписанной инструкции.
