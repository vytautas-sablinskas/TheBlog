using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? ImageBase64Encoded { get; set; }
        public DateTime CreatedTime { get; set; }

        [ForeignKey("UserId")]
        [Required]
        public virtual User User { get; set; }

        [Required]
        public string UserId { get; set; }

        public int Rating { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}