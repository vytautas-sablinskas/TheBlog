using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        public string Token { get; set; }

        public DateTime ExpiryDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public string UserId { get; set; }
    }
}