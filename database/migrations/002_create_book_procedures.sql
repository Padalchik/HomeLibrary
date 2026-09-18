SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

CREATE OR ALTER PROCEDURE dbo.Book_Create
    @Title NVARCHAR(300),
    @Author NVARCHAR(200),
    @PublicationYear INT,
    @TableOfContents XML
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME2 = SYSUTCDATETIME();

    INSERT INTO dbo.Book (Title, Author, PublicationYear, TableOfContents, CreatedAt, UpdatedAt)
    OUTPUT INSERTED.Id
    VALUES (@Title, @Author, @PublicationYear, @TableOfContents, @Now, @Now);
END;
GO

CREATE OR ALTER PROCEDURE dbo.Book_Update
    @Id INT,
    @Title NVARCHAR(300),
    @Author NVARCHAR(200),
    @PublicationYear INT,
    @TableOfContents XML
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Book
    SET Title = @Title,
        Author = @Author,
        PublicationYear = @PublicationYear,
        TableOfContents = @TableOfContents,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Book_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Book
    WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Book_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Title,
           Author,
           PublicationYear,
           TableOfContents,
           CreatedAt,
           UpdatedAt
    FROM dbo.Book
    WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Book_Search
    @Search NVARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NormalizedSearch NVARCHAR(300) = NULLIF(LTRIM(RTRIM(@Search)), N'');

    SELECT Id,
           Title,
           Author,
           PublicationYear,
           TableOfContents,
           CreatedAt,
           UpdatedAt
    FROM dbo.Book
    WHERE @NormalizedSearch IS NULL
       OR Title LIKE N'%' + @NormalizedSearch + N'%'
       OR Author LIKE N'%' + @NormalizedSearch + N'%'
       OR CONVERT(NVARCHAR(MAX), TableOfContents) LIKE N'%' + @NormalizedSearch + N'%'
    ORDER BY Id DESC;
END;
GO
