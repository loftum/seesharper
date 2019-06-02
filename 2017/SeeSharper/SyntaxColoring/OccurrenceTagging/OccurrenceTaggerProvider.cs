using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using SeeSharper.Options;

namespace SeeSharper.SyntaxColoring.OccurrenceTagging
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(IClassificationTag))]
    internal class OccurrenceTaggerProvider : IViewTaggerProvider
    {
        [Import]
        internal ITextSearchService TextSearchService { get; set; }
        [Import]
        internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelector { get; set; }
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry { get; set; }
        [Import]
        internal IOccurrenceTaggingOptions Options { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView.TextBuffer != buffer)
            {
                return null;
            }
            var textStructureNavigator = TextStructureNavigatorSelector.GetTextStructureNavigator(buffer);
            
            return (ITagger<T>) new OccurrenceTagger(textView,
                buffer,
                TextSearchService,
                textStructureNavigator,
                ClassificationRegistry,
                Options);
        }
    }

    public static class SVsServiceProviderExtensions
    {
        public static WritableSettingsStore GetWritableSettingsStore(this SVsServiceProvider serviceProvider)
        {
            var manager = new ShellSettingsManager(serviceProvider);
            return manager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }
    }
}