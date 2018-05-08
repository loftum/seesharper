using System.Linq;

namespace SeeSharper.SyntaxColoring
{
    internal static class ValueExtensions
    {
        public static bool In<T>(this T value, params T[] values)
        {
            return values.Contains(value);
        }
    }
}