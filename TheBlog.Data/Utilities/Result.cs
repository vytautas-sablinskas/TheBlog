using System.Diagnostics.CodeAnalysis;

namespace TheBlog.Data.Utilities
{
    [ExcludeFromCodeCoverage]
    public class Result
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }
}