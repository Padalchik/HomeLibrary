namespace HomeLibrary.Application.Books;

public sealed record CreateBookRequest(
    string Title,
    string Author,
    int PublicationYear,
    string TableOfContents);
