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

        //[Category(Category)]
        //[DisplayName("Dim pattern")]
        //[Description("Dim pattern")]
        //public string DimPatternsString
        //{
        //    get => Options?.DimPatternsString;
        //    set => Options.DimPatternsString = value;
        //}

        //[Category(Category)]
        //[DisplayName("Highlight pattern")]
        //[Description("Highlight pattern")]
        //public string HighlightPatternsString
        //{
        //    get => Options?.HighlightPatternsString;
        //    set => Options.HighlightPatternsString = value;
        //}

        protected override void OnActivate(CancelEventArgs e)
        {
            LoadSettingsFromStorage();
            base.OnActivate(e);
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);
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
        private OptionsControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new OptionsControl();
                    _control.DataContext = Options;
                }
                return _control;
            }
        }

        protected override UIElement Child => Control;
    }
}