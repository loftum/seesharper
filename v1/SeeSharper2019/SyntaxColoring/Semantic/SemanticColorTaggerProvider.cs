﻿using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using SeeSharper2019.Options;

namespace SeeSharper2019.SyntaxColoring.Semantic
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("CSharp")]
    [TagType(typeof(IClassificationTag))]
    public class SemanticColorTaggerProvider : ITaggerProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry { get; set; }
        [Import]
        internal IOccurrenceTaggingOptions Options { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return (ITagger<T>) new SemanticColorTagger(buffer, ClassificationRegistry, Options);
        }
    }
}