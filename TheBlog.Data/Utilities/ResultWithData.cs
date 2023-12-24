using System.Diagnostics.CodeAnalysis;

namespace TheBlog.Data.Utilities
{
    [ExcludeFromCodeCoverage]
    public class ResultWithData<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public T Value { get; set; }
    }
}