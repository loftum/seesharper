using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace SeeSharper.OccurrenceColoring
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(TextMarkerTag))]
    internal class OccurrenceTaggerProvider : IViewTaggerProvider
    {
        [Import]
        internal ITextSearchService TextSearchService { get; set; }
        [Import]
        internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelector { get; set; }


        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView.TextBuffer != buffer)
            {
                return null;
            }
            var textStructureNavigator = TextStructureNavigatorSelector.GetTextStructureNavigator(buffer);
            return new OccurrenceTagger(textView, buffer, TextSearchService, textStructureNavigator) as ITagger<T>;
        }
    }
}