using HomeLibrary.Domain.Entities;

namespace HomeLibrary.Application.Books;

public interface IBookRepository
{
    Task<int> CreateAsync(CreateBookRequest request);

    Task UpdateAsync(UpdateBookRequest request);

    Task DeleteAsync(int id);

    Task<Book?> GetByIdAsync(int id);

    Task<IReadOnlyCollection<Book>> SearchAsync(string? search);
}
