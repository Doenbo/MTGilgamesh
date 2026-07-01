IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CardsAsJson')
BEGIN
    CREATE TABLE CardsAsJson (
        Id UNIQUEIDENTIFIER NOT NULL,
        Name NVARCHAR(255) NOT NULL,
        CollectorSet NVARCHAR(255) NOT NULL,
        CollectorNumber NVARCHAR(255) NOT NULL,
        RawJson NVARCHAR(MAX) NOT NULL,
        AppVersion NVARCHAR(50) NOT NULL,
        CONSTRAINT PK_CardsAsJson PRIMARY KEY (Id)
    );
END