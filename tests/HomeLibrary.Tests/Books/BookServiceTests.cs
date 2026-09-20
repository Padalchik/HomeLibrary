using HomeLibrary.Application.Books;
using HomeLibrary.Domain.Entities;

namespace HomeLibrary.Tests.Books;

public sealed class BookServiceTests
{
    private const string ValidTableOfContents =
        "<TableOfContents><Chapter>Introduction</Chapter></TableOfContents>";

    [Fact]
    public async Task CreateAsync_WithValidRequest_CallsRepository()
    {
        var repository = new FakeBookRepository { CreatedId = 42 };
        var service = new BookService(repository);
        var request = new CreateBookRequest("Clean Code", "Robert Martin", 2008, ValidTableOfContents);

        var id = await service.CreateAsync(request);

        Assert.Equal(42, id);
        Assert.Same(request, repository.CreateRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_WithEmptyTitle_Throws(string title)
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(
            new CreateBookRequest(title, "Author", 2020, ValidTableOfContents)));

        Assert.Null(repository.CreateRequest);
    }

    [Fact]
    public async Task CreateAsync_WithTitleLongerThan300Characters_Throws()
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(
            new CreateBookRequest(new string('T', 301), "Author", 2020, ValidTableOfContents)));

        Assert.Null(repository.CreateRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_WithEmptyAuthor_Throws(string author)
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(
            new CreateBookRequest("Title", author, 2020, ValidTableOfContents)));

        Assert.Null(repository.CreateRequest);
    }

    [Fact]
    public async Task CreateAsync_WithAuthorLongerThan200Characters_Throws()
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(
            new CreateBookRequest("Title", new string('A', 201), 2020, ValidTableOfContents)));

        Assert.Null(repository.CreateRequest);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateAsync_WithInvalidPublicationYear_Throws(int year)
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.CreateAsync(
            new CreateBookRequest("Title", "Author", year, ValidTableOfContents)));

        Assert.Null(repository.CreateRequest);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyTableOfContents_Throws()
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(
            new CreateBookRequest("Title", "Author", 2020, " ")));

        Assert.Null(repository.CreateRequest);
    }

    [Fact]
    public async Task UpdateAsync_WhenBookExists_CallsRepository()
    {
        var repository = new FakeBookRepository { Book = CreateBook(7) };
        var service = new BookService(repository);
        var request = new UpdateBookRequest(7, "New title", "New author", 2024, ValidTableOfContents);

        var result = await service.UpdateAsync(request);

        Assert.True(result);
        Assert.Same(request, repository.UpdateRequest);
    }

    [Fact]
    public async Task UpdateAsync_WhenBookDoesNotExist_ReturnsFalse()
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        var result = await service.UpdateAsync(
            new UpdateBookRequest(7, "Title", "Author", 2024, ValidTableOfContents));

        Assert.False(result);
        Assert.Null(repository.UpdateRequest);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsRepositoryResult()
    {
        var book = CreateBook(3);
        var repository = new FakeBookRepository { Book = book };
        var service = new BookService(repository);

        var result = await service.GetByIdAsync(3);

        Assert.Same(book, result);
        Assert.Equal(3, repository.LastRequestedId);
    }

    [Fact]
    public async Task GetAllAsync_UsesSearchWithoutFilter()
    {
        var repository = new FakeBookRepository { Books = [CreateBook(1)] };
        var service = new BookService(repository);

        var result = await service.GetAllAsync();

        Assert.Single(result);
        Assert.True(repository.SearchWasCalled);
        Assert.Null(repository.LastSearch);
    }

    [Fact]
    public async Task DeleteAsync_WhenBookExists_CallsRepository()
    {
        var repository = new FakeBookRepository { Book = CreateBook(9) };
        var service = new BookService(repository);

        var result = await service.DeleteAsync(9);

        Assert.True(result);
        Assert.Equal(9, repository.DeletedId);
    }

    [Fact]
    public async Task DeleteAsync_WhenBookDoesNotExist_ReturnsFalse()
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        var result = await service.DeleteAsync(9);

        Assert.False(result);
        Assert.Null(repository.DeletedId);
    }

    private static Book CreateBook(int id) => new()
    {
        Id = id,
        Title = "Title",
        Author = "Author",
        PublicationYear = 2020,
        TableOfContents = ValidTableOfContents
    };

    private sealed class FakeBookRepository : IBookRepository
    {
        public int CreatedId { get; init; }

        public Book? Book { get; init; }

        public IReadOnlyCollection<Book> Books { get; init; } = [];

        public CreateBookRequest? CreateRequest { get; private set; }

        public UpdateBookRequest? UpdateRequest { get; private set; }

        public int? DeletedId { get; private set; }

        public int? LastRequestedId { get; private set; }

        public bool SearchWasCalled { get; private set; }

        public string? LastSearch { get; private set; }

        public Task<int> CreateAsync(CreateBookRequest request)
        {
            CreateRequest = request;
            return Task.FromResult(CreatedId);
        }

        public Task UpdateAsync(UpdateBookRequest request)
        {
            UpdateRequest = request;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            DeletedId = id;
            return Task.CompletedTask;
        }

        public Task<Book?> GetByIdAsync(int id)
        {
            LastRequestedId = id;
            return Task.FromResult(Book);
        }

        public Task<IReadOnlyCollection<Book>> SearchAsync(string? search)
        {
            SearchWasCalled = true;
            LastSearch = search;
            return Task.FromResult(Books);
        }
    }
}
