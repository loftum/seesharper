using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SeeSharper.SyntaxColoring.Tags
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationTypeNames.Identifier)]
    [Name(TagTypes.Identifier)]
    [UserVisible(false)]
    [Order(After = Priority.Default)]
    internal class OverrideIdentifierFormatDefinition : ClassificationFormatDefinition
    {
        public OverrideIdentifierFormatDefinition()
        {
            DisplayName = "SeeSharper Identifier";
            BackgroundColor = null;
            ForegroundColor = null;
            IsBold = null;
            IsItalic = null;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagTypes.ExtensionMethd)]
    [BaseDefinition(ClassificationTypeNames.Identifier)]
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
    [ClassificationType(ClassificationTypeNames = TagTypes.Method)]
    [Name(TagTypes.Method)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal class MethodFormatDefinition : ClassificationFormatDefinition
    {
        public MethodFormatDefinition()
        {
            DisplayName = "SeeSharper Extension method";
            //ForegroundColor = Color.FromRgb(220, 220, 220);
            IsBold = false;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    //[ClassificationType(ClassificationTypeNames = TagTypes.StaticClass)]
    [ClassificationType(ClassificationTypeNames = ClassificationTypeNames.ClassName)]
    [Name(TagTypes.StaticClass)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
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
    [ClassificationType(ClassificationTypeNames = TagTypes.Constant)]
    [Name(TagTypes.Constant)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal class ConstantFormatDefinition : ClassificationFormatDefinition
    {
        public ConstantFormatDefinition()
        {
            DisplayName = "SeeSharper Constant";
            ForegroundColor = Color.FromRgb(255, 255, 0);
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