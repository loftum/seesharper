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
    }
}