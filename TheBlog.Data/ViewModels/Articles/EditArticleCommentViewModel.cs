using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.ViewModels.Articles
{
    [ExcludeFromCodeCoverage]
    public class EditArticleCommentViewModel
    {
        [Required]
        public int CommentId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(500, ErrorMessage = "The comment cannot exceed 500 characters.")]
        public string Text { get; set; }
    }
}