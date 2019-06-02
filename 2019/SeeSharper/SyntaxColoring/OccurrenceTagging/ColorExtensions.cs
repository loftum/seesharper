using System.Windows.Media;

namespace SeeSharper.SyntaxColoring.OccurrenceTagging
{
    public static class ColorExtensions
    {
        public static Brush WithOpacity(this Brush color, double opacity)
        {
            color = color.IsFrozen ? color.Clone() : color;
            color.Opacity = opacity;
            return color;
        }

        public static Color WithAlpha(this Color color, byte alpha)
        {
            color.A = alpha;
            return color;
        }
    }
}