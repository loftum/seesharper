using System.ComponentModel;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;

namespace SeeSharper.Options
{
    public class OccurrenceTaggingOptionsPage : DialogPage
    {
        private IOccurrenceTaggingOptions _options;
        private IOccurrenceTaggingOptions Options => _options ??
            (_options = Site.GetService<IComponentModel>(typeof(SComponentModel)).DefaultExportProvider.GetExportedValue<IOccurrenceTaggingOptions>());

        private static int _nextId = 0;
        private readonly int _id = ++_nextId;

        public const string Category = "SeeSharper";
        public const string PageName = "Tagging";

        [Category(Category)]
        [DisplayName("Dim pattern")]
        [Description("Dim pattern")]
        public string DimPatternsString
        {
            get => Options?.DimPatternsString;
            set => Options.DimPatternsString = value;
        }

        [Category(Category)]
        [DisplayName("Highlight pattern")]
        [Description("Highlight pattern")]
        public string HighlightPatternsString
        {
            get => Options?.HighlightPatternsString;
            set => Options.HighlightPatternsString = value;
        }

        public override void LoadSettingsFromStorage()
        {
            Options.Load();
        }

        public override void SaveSettingsToStorage()
        {
            Options.Save();
        }
    }
}