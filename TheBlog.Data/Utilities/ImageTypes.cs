using System.Diagnostics.CodeAnalysis;

namespace TheBlog.Data.Utilities
{
    [ExcludeFromCodeCoverage]
    public static class ImageTypes
    {
        public static readonly List<string> TypesAllowed = new()
        {
            "jpeg",
            "png",
            "gif"
        };
    }
}