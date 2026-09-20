using HomeLibrary.Application.Books;
using HomeLibrary.Domain.Entities;
using HomeLibrary.Web.Models.Books;
using HomeLibrary.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeLibrary.Web.Controllers;

[Route("books")]
public sealed class BooksController(
    IBookService bookService,
    ITableOfContentsFormatter tableOfContentsFormatter) : Controller
{
    private const string EmptyTableOfContentsError = "Оглавление не может быть пустым.";

    [HttpGet("")]
    public async Task<IActionResult> Index(string? search)
    {
        var books = await bookService.SearchAsync(search);
        var model = new BookIndexViewModel
        {
            Search = search,
            Books = books.Select(book => new BookListItemViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublicationYear = book.PublicationYear
            }).ToArray()
        };

        return View(model);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var book = await bookService.GetByIdAsync(id);
        return book is null ? NotFound() : View(ToDetailsViewModel(book));
    }

    [HttpGet("create")]
    public IActionResult Create() => View(new BookCreateViewModel());

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!TryFormatTableOfContents(model.TableOfContentsHtml, out var tableOfContents))
        {
            return View(model);
        }

        var id = await bookService.CreateAsync(new CreateBookRequest(
            model.Title,
            model.Author,
            model.PublicationYear,
            tableOfContents));

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await bookService.GetByIdAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        return View(new BookEditViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            PublicationYear = book.PublicationYear,
            TableOfContentsHtml = tableOfContentsFormatter.FromXml(book.TableOfContents)
        });
    }

    [HttpPost("{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!TryFormatTableOfContents(model.TableOfContentsHtml, out var tableOfContents))
        {
            return View(model);
        }

        var updated = await bookService.UpdateAsync(new UpdateBookRequest(
            model.Id,
            model.Title,
            model.Author,
            model.PublicationYear,
            tableOfContents));

        return updated
            ? RedirectToAction(nameof(Details), new { id })
            : NotFound();
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await bookService.DeleteAsync(id);
        return deleted ? RedirectToAction(nameof(Index)) : NotFound();
    }

    private BookDetailsViewModel ToDetailsViewModel(Book book) => new()
    {
        Id = book.Id,
        Title = book.Title,
        Author = book.Author,
        PublicationYear = book.PublicationYear,
        TableOfContentsHtml = tableOfContentsFormatter.FromXml(book.TableOfContents),
        CreatedAt = book.CreatedAt,
        UpdatedAt = book.UpdatedAt
    };

    private bool TryFormatTableOfContents(string html, out string xml)
    {
        try
        {
            xml = tableOfContentsFormatter.ToXml(html);
            return true;
        }
        catch (ArgumentException)
        {
            ModelState.AddModelError(nameof(BookCreateViewModel.TableOfContentsHtml), EmptyTableOfContentsError);
            xml = string.Empty;
            return false;
        }
    }
}
