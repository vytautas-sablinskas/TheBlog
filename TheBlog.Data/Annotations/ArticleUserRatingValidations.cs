using System.ComponentModel.DataAnnotations;
using TheBlog.MVC.ViewModels.Articles;

namespace TheBlog.MVC.Annotations
{
    public class ArticleUserRatingValidations : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (ArticleUserRatingViewModel)validationContext.ObjectInstance;

            if (model.LikedByUser && model.DislikedByUser)
            {
                return new ValidationResult("LikedByUser and DislikedByUser cannot both be true.");
            }

            return ValidationResult.Success;
        }
    }
}