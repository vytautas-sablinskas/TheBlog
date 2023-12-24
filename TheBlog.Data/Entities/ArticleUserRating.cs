using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class ArticleUserRating
    {
        public int Id { get; set; }

        [ForeignKey("ArticleId")]
        public virtual Article Article { get; set; }

        public int ArticleId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public string UserId { get; set; }
        public bool LikedByUser { get; set; }
        public bool DislikedByUser { get; set; }
    }
}