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
            Control.SemanticColoringBox.IsChecked = Options.SemanticColoringEnabled;
            Control.OccurrenceTaggingBox.IsChecked = Options.OccurrenceTaggingEnabled;
            Control.DimPatterns.Text = Options.DimPatternsString;
            Control.HighlightPatterns.Text = Options.EmphasizePatternsString;
            base.OnActivate(e);
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);
            Options.SemanticColoringEnabled = Control.SemanticColoringBox.IsChecked.GetValueOrDefault();
            Options.OccurrenceTaggingEnabled = Control.OccurrenceTaggingBox.IsChecked.GetValueOrDefault();
            Options.DimPatternsString = Control.DimPatterns.Text;
            Options.EmphasizePatternsString = Control.HighlightPatterns.Text;
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