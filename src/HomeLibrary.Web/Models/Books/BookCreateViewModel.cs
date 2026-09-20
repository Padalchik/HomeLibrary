using System.ComponentModel.DataAnnotations;

namespace HomeLibrary.Web.Models.Books;

public sealed class BookCreateViewModel
{
    [Required(ErrorMessage = "Укажите название книги.")]
    [StringLength(300, ErrorMessage = "Название не должно превышать 300 символов.")]
    [Display(Name = "Название")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите автора.")]
    [StringLength(200, ErrorMessage = "Имя автора не должно превышать 200 символов.")]
    [Display(Name = "Автор")]
    public string Author { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Год должен быть больше нуля.")]
    [Display(Name = "Год")]
    public int PublicationYear { get; set; }

    [Required(ErrorMessage = "Укажите оглавление.")]
    [Display(Name = "Оглавление")]
    public string TableOfContentsHtml { get; set; } = string.Empty;
}
