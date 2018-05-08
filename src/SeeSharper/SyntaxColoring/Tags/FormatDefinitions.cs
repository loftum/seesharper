using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SeeSharper.SyntaxColoring.Tags
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagTypes.ExtensionMethd)]
    [Name(TagTypes.ExtensionMethd)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal class ExtensionMethodFormatDefinition : ClassificationFormatDefinition
    {
        public ExtensionMethodFormatDefinition()
        {
            DisplayName = "SeeSharper Extension method";
            ForegroundColor = Color.FromRgb(220, 220, 220);
            IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagTypes.StaticClass)]
    [Name(TagTypes.StaticClass)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    internal class StaticClassFormatDefinition : ClassificationFormatDefinition
    {
        public StaticClassFormatDefinition()
        {
            DisplayName = "SeeSharper Static class";
            ForegroundColor = Colors.DarkMagenta;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagTypes.Property)]
    [Name(TagTypes.Property)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal class PropertyFormatDefinition : ClassificationFormatDefinition
    {
        public PropertyFormatDefinition()
        {
            DisplayName = "SeeSharper Property";
            ForegroundColor = Color.FromRgb(175, 255, 255);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagTypes.Field)]
    [Name(TagTypes.Field)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal class FieldFormatDefinition : ClassificationFormatDefinition
    {
        public FieldFormatDefinition()
        {
            DisplayName = "SeeSharper Field";
            ForegroundColor = Color.FromRgb(175, 255, 255);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagTypes.Event)]
    [Name(TagTypes.Event)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal class EventFormatDefinition : ClassificationFormatDefinition
    {
        public EventFormatDefinition()
        {
            DisplayName = "SeeSharper Event";
            ForegroundColor = Colors.Magenta;
        }
    }
}