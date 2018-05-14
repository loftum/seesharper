using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace SeeSharper.Options
{
    public class OccurrenceOptionsPage : DialogPage
    {
        public const string Category = "SeeSharper";
        public const string PageName = "Occurrences";

        [Category(Category)]
        [DisplayName("Occurrence Pattern")]
        [Description("Occurrence Pattern")]
        public string OccurrencePattern { get; set; }
    }
}