using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using SeeSharper.SyntaxColoring.Tags;

namespace SeeSharper.OccurrenceColoring
{
    internal class OccurrenceTag: TextMarkerTag
    {
        public OccurrenceTag() : base(TagTypes.Dim)
        {

        }
    }


    [Export(typeof(EditorFormatDefinition))]
    [Name(TagTypes.Dim)]
    [UserVisible(true)]
    [Order(After = ClassificationTypeNames.Identifier)]
    internal class OccurrenceFormat : EditorFormatDefinition
    {
        public OccurrenceFormat()
        {
            ForegroundColor = Colors.DimGray.WithAlpha(100);
            BackgroundColor = Colors.DimGray.WithAlpha(100);
            DisplayName = "Dim Word";
        }
    }

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