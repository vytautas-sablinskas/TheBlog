using System.Diagnostics.CodeAnalysis;

namespace TheBlog.Data.Utilities
{
    [ExcludeFromCodeCoverage]
    public class AppRoles
    {
        public static List<string> Roles = new()
        {
            AppRoleNames.Admin,
            AppRoleNames.ArticleWriter,
            AppRoleNames.ArticleRater,
            AppRoleNames.ArticleCommentator
        };
    }
}