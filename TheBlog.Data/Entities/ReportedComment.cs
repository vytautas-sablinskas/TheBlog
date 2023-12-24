using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class ReportedComment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public string UserId { get; set; }

        [ForeignKey("CommentId")]
        public virtual Comment Comment { get; set; }

        public int CommentId { get; set; }

        public string Reason { get; set; }
    }
}