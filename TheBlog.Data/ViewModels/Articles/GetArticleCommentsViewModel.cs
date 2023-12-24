using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.ViewModels.Articles
{
    [ExcludeFromCodeCoverage]
    public class GetArticleCommentsViewModel
    {
        [Required]
        public int ArticleId { get; set; }
    }
}