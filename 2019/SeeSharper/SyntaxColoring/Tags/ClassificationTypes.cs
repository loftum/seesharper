using System.ComponentModel.Composition;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SeeSharper.SyntaxColoring.Tags
{
    public static class ClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition(ClassificationTypeNames.Identifier)]
        [Name(TagTypes.Method)]
        public static ClassificationTypeDefinition Method;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition(ClassificationTypeNames.Identifier)]
        [Name(TagTypes.ExtensionMethod)]
        public static ClassificationTypeDefinition ExtensionMethod;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition(ClassificationTypeNames.ClassName)]
        [Name(TagTypes.StaticClass)]
        public static ClassificationTypeDefinition StaticClass;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagTypes.Property)]
        public static ClassificationTypeDefinition Property;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagTypes.Field)]
        public static ClassificationTypeDefinition Field;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagTypes.Constant)]
        public static ClassificationTypeDefinition Constant;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagTypes.Event)]
        public static ClassificationTypeDefinition Event;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagTypes.Dim)]
        public static ClassificationTypeDefinition Dim;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagTypes.Highlight)]
        public static ClassificationTypeDefinition Highlight;
    }
}