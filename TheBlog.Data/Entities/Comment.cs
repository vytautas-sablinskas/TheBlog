using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        public string Text { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public string UserId { get; set; }

        public bool IsBlocked { get; set; }
    }
}