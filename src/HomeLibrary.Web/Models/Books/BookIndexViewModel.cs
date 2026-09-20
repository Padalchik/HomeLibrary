namespace HomeLibrary.Web.Models.Books;

public sealed class BookIndexViewModel
{
    public string? Search { get; init; }

    public IReadOnlyCollection<BookListItemViewModel> Books { get; init; } =
        Array.Empty<BookListItemViewModel>();
}
