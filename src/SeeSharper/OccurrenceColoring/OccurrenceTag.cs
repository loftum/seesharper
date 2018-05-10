using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace SeeSharper.OccurrenceColoring
{
    internal class OccurrenceTag: TextMarkerTag
    {
        public OccurrenceTag() : base("MarkerFormatDefinition/OccurrenceFormatDefinition")
        {

        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [Name("MarkerFormatDefinition/OccurrenceFormatDefinition")]
    [UserVisible(true)]
    internal class OccurrenceFormat : MarkerFormatDefinition
    {
        public OccurrenceFormat()
        {
            BackgroundColor = Colors.YellowGreen;
            ForegroundColor = Colors.DarkBlue;
            DisplayName = "Highlight Word";
            ZOrder = 5;
        }
    }
}