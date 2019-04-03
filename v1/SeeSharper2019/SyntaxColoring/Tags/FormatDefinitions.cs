using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using SeeSharper2019.SyntaxColoring.OccurrenceTagging;

namespace SeeSharper2019.SyntaxColoring.Tags
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagTypes.ExtensionMethd)]
    [Name(TagTypes.ExtensionMethd)]
    [UserVisible(true)]
    [Order(After = ClassificationTypeNames.Identifier)]
    internal class ExtensionMethodFormatDefinition : ClassificationFormatDefinition
    {
        public ExtensionMethodFormatDefinition()
        {
            DisplayName = "SeeSharper Extension method";
            IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagTypes.Method)]
    [Name(TagTypes.Method)]
    [UserVisible(true)]
    [Order(After = ClassificationTypeNames.Identifier)]
    internal class MethodFormatDefinition : ClassificationFormatDefinition
    {
        public MethodFormatDefinition()
        {
            DisplayName = "SeeSharper method";
            IsBold = false;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagTypes.StaticClass)]
    [Name(TagTypes.StaticClass)]
    [UserVisible(true)]
    [Order(After = ClassificationTypeNames.ClassName)]
    internal class StaticClassFormatDefinition : ClassificationFormatDefinition
    {
        public StaticClassFormatDefinition()
        {
            DisplayName = "SeeSharper Static class";
            ForegroundColor = Color.FromArgb(255, 0x9b, 0, 0x9b);
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
    [ClassificationType(ClassificationTypeNames = TagTypes.Constant)]
    [Name(TagTypes.Constant)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal class ConstantFormatDefinition : ClassificationFormatDefinition
    {
        public ConstantFormatDefinition()
        {
            DisplayName = "SeeSharper Constant";
            ForegroundColor = Colors.Yellow;
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

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagTypes.Dim)]
    [Name(TagTypes.Dim)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    internal class DimFormatDefinition : ClassificationFormatDefinition
    {
        public DimFormatDefinition()
        {
            DisplayName = "SeeSharper Dim";
            ForegroundOpacity = 0.3;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagTypes.Highlight)]
    [Name(TagTypes.Highlight)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    internal class HighlightFormatDefinition : ClassificationFormatDefinition
    {
        public HighlightFormatDefinition()
        {
            DisplayName = "SeeSharper Highlight";
            ForegroundColor = Colors.Yellow.WithAlpha(100);
        }
    }
}