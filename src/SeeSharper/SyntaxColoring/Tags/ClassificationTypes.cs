using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SeeSharper.SyntaxColoring.Tags
{
    public static class ClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagTypes.ExtensionMethd)]
        public static ClassificationTypeDefinition ExtensionMethod;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagTypes.StaticClass)]
        public static ClassificationTypeDefinition StaticClass;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagTypes.Property)]
        public static ClassificationTypeDefinition Property;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagTypes.Field)]
        public static ClassificationTypeDefinition Field;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagTypes.Event)]
        public static ClassificationTypeDefinition Event;
    }
}