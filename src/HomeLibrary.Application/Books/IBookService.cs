using HomeLibrary.Domain.Entities;

namespace HomeLibrary.Application.Books;

public interface IBookService
{
    Task<IReadOnlyCollection<Book>> GetAllAsync();

    Task<Book?> GetByIdAsync(int id);

    Task<int> CreateAsync(CreateBookRequest request);

    Task<bool> UpdateAsync(UpdateBookRequest request);

    Task<bool> DeleteAsync(int id);
}
