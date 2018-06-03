using System.ComponentModel;
using System.Windows;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;

namespace SeeSharper.Options
{
    public class OccurrenceTaggingOptionsPage : UIElementDialogPage
    {
        private IOccurrenceTaggingOptions _options;
        private IOccurrenceTaggingOptions Options => _options ??
            (_options = Site.GetService<IComponentModel>(typeof(SComponentModel)).DefaultExportProvider.GetExportedValue<IOccurrenceTaggingOptions>());

        public const string Category = "SeeSharper";
        public const string PageName = "Tagging";

        protected override void OnActivate(CancelEventArgs e)
        {
            LoadSettingsFromStorage();
            Control.DimPatterns.Text = Options.DimPatternsString;
            Control.HighlightPatterns.Text = Options.HighlightPatternsString;
            base.OnActivate(e);
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);
            Options.DimPatternsString = Control.DimPatterns.Text;
            Options.HighlightPatternsString = Control.HighlightPatterns.Text;
            SaveSettingsToStorage();
        }

        public override void LoadSettingsFromStorage()
        {
            Options.Load();
        }

        public override void SaveSettingsToStorage()
        {
            Options.Save();
        }

        private OptionsControl _control;
        private OptionsControl Control => _control ?? (_control = new OptionsControl());

        protected override UIElement Child => Control;
    }
}