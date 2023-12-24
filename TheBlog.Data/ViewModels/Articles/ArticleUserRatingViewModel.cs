using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TheBlog.MVC.Annotations;

namespace TheBlog.MVC.ViewModels.Articles
{
    [ExcludeFromCodeCoverage]
    [ArticleUserRatingValidations]
    public class ArticleUserRatingViewModel
    {
        [Required]
        public int ArticleId { get; set; }

        [Required]
        public bool LikedByUser { get; set; }

        [Required]
        public bool DislikedByUser { get; set; }
    }
}