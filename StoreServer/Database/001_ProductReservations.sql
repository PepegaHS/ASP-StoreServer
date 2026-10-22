-- Run against the existing web_store database. Safe to run again.
SET XACT_ABORT ON;

IF COL_LENGTH(N'dbo.Products', N'available_quantity') IS NULL
BEGIN
    ALTER TABLE dbo.Products ADD available_quantity int NOT NULL
        CONSTRAINT DF_Products_available_quantity DEFAULT (0) WITH VALUES;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Products_available_quantity')
    ALTER TABLE dbo.Products ADD CONSTRAINT CK_Products_available_quantity
        CHECK (available_quantity >= 0);
GO

IF OBJECT_ID(N'dbo.ProductReservations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductReservations
    (
        ID_product_reservation int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProductReservations PRIMARY KEY,
        ID_product int NOT NULL,
        ID_user int NOT NULL,
        quantity int NOT NULL,
        created_at datetime2 NOT NULL,
        cancelled_at datetime2 NULL,
        CONSTRAINT FK_ProductReservations_Products FOREIGN KEY (ID_product) REFERENCES dbo.Products(ID_product),
        CONSTRAINT FK_ProductReservations_Users FOREIGN KEY (ID_user) REFERENCES dbo.Users(ID_user),
        CONSTRAINT CK_ProductReservations_quantity CHECK (quantity > 0),
        CONSTRAINT CK_ProductReservations_dates CHECK (cancelled_at IS NULL OR cancelled_at >= created_at)
    );
    CREATE INDEX IX_ProductReservations_ID_product ON dbo.ProductReservations(ID_product);
    CREATE INDEX IX_ProductReservations_ID_user ON dbo.ProductReservations(ID_user);
END;
GO
