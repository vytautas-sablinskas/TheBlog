using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.ViewModels.Articles
{
    [ExcludeFromCodeCoverage]
    public class AddArticleCommentViewModel
    {
        [Required]
        public int ArticleId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(500, ErrorMessage = "The comment cannot exceed 500 characters.")]
        public string Text { get; set; }
    }
}