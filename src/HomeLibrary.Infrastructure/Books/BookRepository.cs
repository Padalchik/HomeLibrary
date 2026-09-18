using System.Data;
using Dapper;
using HomeLibrary.Application.Books;
using HomeLibrary.Domain.Entities;
using HomeLibrary.Infrastructure.Data;

namespace HomeLibrary.Infrastructure.Books;

public sealed class BookRepository(ISqlConnectionFactory connectionFactory) : IBookRepository
{
    public async Task<int> CreateAsync(CreateBookRequest request)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Title", request.Title, DbType.String, size: 300);
        parameters.Add("Author", request.Author, DbType.String, size: 200);
        parameters.Add("PublicationYear", request.PublicationYear, DbType.Int32);
        parameters.Add("TableOfContents", request.TableOfContents, DbType.Xml);

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        return await connection.QuerySingleAsync<int>(
            "dbo.Book_Create",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateAsync(UpdateBookRequest request)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Id", request.Id, DbType.Int32);
        parameters.Add("Title", request.Title, DbType.String, size: 300);
        parameters.Add("Author", request.Author, DbType.String, size: 200);
        parameters.Add("PublicationYear", request.PublicationYear, DbType.Int32);
        parameters.Add("TableOfContents", request.TableOfContents, DbType.Xml);

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await connection.ExecuteAsync(
            "dbo.Book_Update",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteAsync(int id)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await connection.ExecuteAsync(
            "dbo.Book_Delete",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        return await connection.QuerySingleOrDefaultAsync<Book>(
            "dbo.Book_GetById",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IReadOnlyCollection<Book>> SearchAsync(string? search)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Search", search, DbType.String, size: 300);

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var books = await connection.QueryAsync<Book>(
            "dbo.Book_Search",
            parameters,
            commandType: CommandType.StoredProcedure);

        return books.AsList();
    }
}
