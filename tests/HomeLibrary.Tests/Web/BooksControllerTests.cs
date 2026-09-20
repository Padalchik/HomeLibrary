using HomeLibrary.Application.Books;
using HomeLibrary.Domain.Entities;
using HomeLibrary.Web.Controllers;
using HomeLibrary.Web.Models.Books;
using HomeLibrary.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeLibrary.Tests.Web;

public sealed class BooksControllerTests
{
    [Fact]
    public async Task Index_PassesSearchAndMapsBooksToViewModel()
    {
        const string search = "Martin";
        var book = new Book
        {
            Id = 17,
            Title = "Clean Code",
            Author = "Robert Martin",
            PublicationYear = 2008
        };
        var service = new FakeBookService { SearchResult = [book] };
        var controller = new BooksController(service, new StubTableOfContentsFormatter());

        var result = await controller.Index(search);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<BookIndexViewModel>(view.Model);
        Assert.Equal(search, service.LastSearch);
        Assert.Equal(search, model.Search);
        var item = Assert.Single(model.Books);
        Assert.Equal(book.Id, item.Id);
        Assert.Equal(book.Title, item.Title);
        Assert.Equal(book.Author, item.Author);
        Assert.Equal(book.PublicationYear, item.PublicationYear);
    }

    [Fact]
    public async Task Create_WhenSanitizedTableOfContentsIsEmpty_ReturnsFormWithValidationError()
    {
        var service = new FakeBookService();
        var controller = new BooksController(service, new TableOfContentsFormatter());
        var model = new BookCreateViewModel
        {
            Title = "Test book",
            Author = "Test author",
            PublicationYear = 2026,
            TableOfContentsHtml = "<script>alert(1)</script>"
        };

        var result = await controller.Create(model);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(model, view.Model);
        var error = Assert.Single(controller.ModelState[nameof(model.TableOfContentsHtml)]!.Errors);
        Assert.Equal("Оглавление не может быть пустым.", error.ErrorMessage);
        Assert.False(service.CreateWasCalled);
    }

    private sealed class FakeBookService : IBookService
    {
        public IReadOnlyCollection<Book> SearchResult { get; init; } = [];

        public string? LastSearch { get; private set; }

        public bool CreateWasCalled { get; private set; }

        public Task<IReadOnlyCollection<Book>> SearchAsync(string? search)
        {
            LastSearch = search;
            return Task.FromResult(SearchResult);
        }

        public Task<IReadOnlyCollection<Book>> GetAllAsync() =>
            Task.FromResult<IReadOnlyCollection<Book>>([]);

        public Task<Book?> GetByIdAsync(int id) => Task.FromResult<Book?>(null);

        public Task<int> CreateAsync(CreateBookRequest request)
        {
            CreateWasCalled = true;
            return Task.FromResult(0);
        }

        public Task<bool> UpdateAsync(UpdateBookRequest request) => Task.FromResult(false);

        public Task<bool> DeleteAsync(int id) => Task.FromResult(false);
    }

    private sealed class StubTableOfContentsFormatter : ITableOfContentsFormatter
    {
        public string ToXml(string html) => html;

        public string FromXml(string xml) => xml;
    }
}
