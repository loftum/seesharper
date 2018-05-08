using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using SeeSharper.SyntaxColoring.Tags;

namespace SeeSharper.SyntaxColoring
{
    public class SemanticColorTagger : ITagger<IClassificationTag>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private readonly ITextView _textView;
        private readonly ITextBuffer _buffer;
        private readonly IClassificationTypeRegistryService _registry;
        private readonly ITextStructureNavigator _textStructureNavigator;
        

        private readonly IClassificationType _extensionMethod;

        public SemanticColorTagger(ITextView textVicew, ITextBuffer buffer, IClassificationTypeRegistryService registry, ITextStructureNavigator textStructureNavigator)
        {
            _textView = textVicew;
            _buffer = buffer;
            _registry = registry;
            _textStructureNavigator = textStructureNavigator;
            _extensionMethod = registry.GetClassificationType(TagTypes.ExtensionMethd);
        }

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                yield break;
            }

            var thingy = Thingy.Get(_buffer, spans);
            if (thingy == null)
            {
                yield break;
            }

            foreach (var span in thingy.GetClassifiedSpans())
            {
                var symbol = thingy.GetSymbol(span);
                switch (symbol)
                {
                    case IMethodSymbol method:
                        if (method.IsExtensionMethod)
                        {
                            yield return span.TextSpan.ToTagSpan(thingy.Snapshot, _extensionMethod);
                        }
                        break;
                }
            }
        }
    }
}