namespace HomeLibrary.Application.Books;

public sealed record UpdateBookRequest(
    int Id,
    string Title,
    string Author,
    int PublicationYear,
    string TableOfContents);
