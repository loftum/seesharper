using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using SeeSharper.SyntaxColoring.Tags;
using System.Reflection;

namespace SeeSharper.SyntaxColoring
{
    public class SemanticColorTagger : ITagger<IClassificationTag>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private readonly ITextView _textView;
        private readonly ITextBuffer _buffer;
        private readonly IClassificationTypeRegistryService _registry;
        private readonly ITextStructureNavigator _textStructureNavigator;
        private readonly IDictionary<string, IClassificationType> _clasificationTypes = new Dictionary<string, IClassificationType>();
        private Thingy _thingy;

        public SemanticColorTagger(ITextView textView, ITextBuffer buffer, IClassificationTypeRegistryService registry, ITextStructureNavigator textStructureNavigator)
        {
            _textView = textView;
            _buffer = buffer;
            _registry = registry;
            _textStructureNavigator = textStructureNavigator;

            foreach(string key in typeof(TagTypes).GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.FieldType == typeof(string)).Select(f => f.GetValue(null)))
            {
                _clasificationTypes[key] = registry.GetClassificationType(key);
            }
            textView.LayoutChanged += ViewLayoutChanged;
        }

        private void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (e.OldSnapshot == e.NewSnapshot)
            {
                return;
            }
        }

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                yield break;
            }
            if (_thingy == null || _thingy.Snapshot != spans[0].Snapshot)
            {
                _thingy = Thingy.Get(_buffer, spans[0].Snapshot);
            }
            if (_thingy == null)
            {
                yield break;
            }

            foreach (var span in _thingy.GetClassifiedSpans(spans))
            {
                var symbol = _thingy.GetSymbol(span);
                switch (symbol)
                {
                    case IMethodSymbol method:
                        if (method.IsExtensionMethod)
                        {
                            yield return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.ExtensionMethd]);
                        }
                        break;
                    case ITypeSymbol type:
                        if (type.TypeKind == TypeKind.Class && type.IsStatic)
                        {
                            yield return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.StaticClass]);
                        }
                        break;
                    case IFieldSymbol _:
                        yield return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.Field]);
                        break;
                    case IPropertySymbol _:
                        yield return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.Property]);
                        break;
                    case IEventSymbol _:
                        yield return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.Event]);
                        break;
                }
            }
        }
    }
}