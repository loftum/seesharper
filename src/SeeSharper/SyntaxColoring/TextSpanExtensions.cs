using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace SeeSharper.SyntaxColoring
{
    public static class TextSpanExtensions
    {
        public static ITagSpan<IClassificationTag> ToTagSpan(this TextSpan span, ITextSnapshot snapShot, IClassificationType classificationType)
        {
            return new TagSpan<IClassificationTag>(new SnapshotSpan(snapShot, span.Start, span.Length), new ClassificationTag(classificationType));
        }
    }
}