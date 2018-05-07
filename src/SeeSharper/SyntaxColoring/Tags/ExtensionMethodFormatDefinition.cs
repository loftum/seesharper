using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SeeSharper.SyntaxColoring.Tags
{
    [Export(typeof(EditorFormatDefinition))]
    [Name(TagTypes.ExtensionMethd)]
    [UserVisible(true)]
    internal class ExtensionMethodFormatDefinition : MarkerFormatDefinition
    {
        public ExtensionMethodFormatDefinition()
        {
            ForegroundColor = Colors.Yellow;
            ZOrder = 5;
        }
    }
}