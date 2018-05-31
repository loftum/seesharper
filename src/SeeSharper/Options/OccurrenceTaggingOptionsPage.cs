using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace SeeSharper.Options
{
    
    public class OccurrenceTaggingOptionsPage : DialogPage
    {
        public const string Category = "SeeSharper";
        public const string PageName = "Tagging";

        [Category(Category)]
        [DisplayName("Dim pattern")]
        [Description("Dim pattern")]
        public string DimPatterns { get; set; }

        [Category(Category)]
        [DisplayName("Highlight pattern")]
        [Description("Highlight pattern")]
        public string HighlightPatterns { get; set; }
    }
}