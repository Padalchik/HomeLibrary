namespace HomeLibrary.Web.Models.Books;

public sealed class BookListItemViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Author { get; init; } = string.Empty;

    public int PublicationYear { get; init; }
}
