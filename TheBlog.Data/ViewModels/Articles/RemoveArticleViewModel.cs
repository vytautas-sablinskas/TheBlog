using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.ViewModels.Articles
{
    [ExcludeFromCodeCoverage]
    public class RemoveArticleViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}