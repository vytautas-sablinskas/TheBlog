using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using TheBlog.Data.Entities;

namespace TheBlog.Data.Database
{
    [ExcludeFromCodeCoverage]
    public class TheBlogDbContext : IdentityDbContext<User>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleUserRating> ArticleUserRatings { get; set; }
        public DbSet<ReportedComment> ReportedComments { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public TheBlogDbContext(DbContextOptions<TheBlogDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ArticleUserRating>()
                .HasOne(aur => aur.Article)
                .WithMany()
                .HasForeignKey(aur => aur.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ArticleUserRating>()
                .HasOne(aur => aur.User)
                .WithMany()
                .HasForeignKey(aur => aur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(aur => aur.User)
                .WithMany()
                .HasForeignKey(aur => aur.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}