using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.ViewModels.Articles
{
    [ExcludeFromCodeCoverage]
    public class EditArticleViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "The title must be up to 150 characters long.")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "The description must be up to 1000 characters long.")]
        public string Description { get; set; }

        public string? ImageBase64Encoded { get; set; }

        public string? ImageType { get; set; }
    }
}