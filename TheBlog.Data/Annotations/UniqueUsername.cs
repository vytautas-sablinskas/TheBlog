using System.ComponentModel.DataAnnotations;
using TheBlog.Data.Database;

namespace TheBlog.MVC.Annotations
{
    public class UniqueUsername : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _context = (TheBlogDbContext)validationContext.GetService(typeof(TheBlogDbContext));

            var entity = _context.Users.FirstOrDefault(u => u.UserName.ToLower() == value.ToString().ToLower());
            if (entity != null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}