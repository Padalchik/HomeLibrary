using HomeLibrary.Domain.Entities;

namespace HomeLibrary.Application.Books;

public sealed class BookService(IBookRepository repository) : IBookService
{
    public Task<IReadOnlyCollection<Book>> GetAllAsync() => repository.SearchAsync(null);

    public Task<IReadOnlyCollection<Book>> SearchAsync(string? search) =>
        repository.SearchAsync(search);

    public Task<Book?> GetByIdAsync(int id) => repository.GetByIdAsync(id);

    public Task<int> CreateAsync(CreateBookRequest request)
    {
        Validate(request.Title, request.Author, request.PublicationYear, request.TableOfContents);
        return repository.CreateAsync(request);
    }

    public async Task<bool> UpdateAsync(UpdateBookRequest request)
    {
        Validate(request.Title, request.Author, request.PublicationYear, request.TableOfContents);

        if (await repository.GetByIdAsync(request.Id) is null)
        {
            return false;
        }

        await repository.UpdateAsync(request);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (await repository.GetByIdAsync(id) is null)
        {
            return false;
        }

        await repository.DeleteAsync(id);
        return true;
    }

    private static void Validate(
        string title,
        string author,
        int publicationYear,
        string tableOfContents)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title must not be empty.", nameof(title));
        }

        if (title.Length > 300)
        {
            throw new ArgumentException(
                "Title must not exceed 300 characters.",
                nameof(title));
        }

        if (string.IsNullOrWhiteSpace(author))
        {
            throw new ArgumentException("Author must not be empty.", nameof(author));
        }

        if (author.Length > 200)
        {
            throw new ArgumentException(
                "Author must not exceed 200 characters.",
                nameof(author));
        }

        if (publicationYear <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(publicationYear),
                publicationYear,
                "Publication year must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(tableOfContents))
        {
            throw new ArgumentException(
                "Table of contents must not be empty.",
                nameof(tableOfContents));
        }
    }
}
