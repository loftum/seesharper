﻿using System.ComponentModel.Composition;
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
            ForegroundColor = Colors.Yellow;
            IsBold = true;
        }
    }
}