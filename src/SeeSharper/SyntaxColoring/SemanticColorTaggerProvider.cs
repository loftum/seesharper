using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace SeeSharper.SyntaxColoring
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(TextMarkerTag))]
    public class SemanticColorTaggerProvider : IViewTaggerProvider
    {
        [Import]
        internal ITextSearchService TextSearchService { get; set; }
        [Import]
        internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelector { get; set; }
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry;

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView.TextBuffer != buffer)
            {
                return null;
            }
            var navigator = TextStructureNavigatorSelector.GetTextStructureNavigator(buffer);
            return (ITagger<T>) new SemanticColorTagger(textView, buffer, ClassificationRegistry, navigator);
        }
    }
}