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
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Classification;

namespace SeeSharper.SyntaxColoring
{
    public static class SnapshotSpanExtensions
    {
        public static TextSpan GetTextSpan(this SnapshotSpan span)
        {
            return TextSpan.FromBounds(span.Start, span.End);
        }
    }

    public class SemanticColorTagger : ITagger<IClassificationTag>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private readonly ITextBuffer _buffer;
        private readonly IClassificationTypeRegistryService _registry;
        private readonly ITextStructureNavigator _textStructureNavigator;
        private readonly IDictionary<string, IClassificationType> _clasificationTypes = new Dictionary<string, IClassificationType>();
        private Thingy _thingy;
        private readonly IList<ITagSpan<IClassificationTag>> _tags = new List<ITagSpan<IClassificationTag>>();

        public SemanticColorTagger(ITextBuffer buffer, IClassificationTypeRegistryService registry)
        {
            _buffer = buffer;
            _registry = registry;

            foreach(string key in typeof(TagTypes).GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.FieldType == typeof(string)).Select(f => f.GetValue(null)))
            {
                _clasificationTypes[key] = registry.GetClassificationType(key);
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
                try
                {
                    var task = Thingy.Get(_buffer, spans[0].Snapshot);
                    task.Wait();
                    _thingy = task.Result;
                }
                catch
                {
                    yield break;
                }
            }
            
            if (_thingy == null)
            {
                yield break;
            }

            foreach (var span in _thingy.GetClassifiedSpans(spans))
            {
                var meta = _thingy.GetMeta(span.TextSpan);
                if (meta.HasSymbol)
                {
                    var symbol = meta.Symbol;
                    switch (symbol.Kind)
                    {
                        case SymbolKind.Method:
                            if (symbol is IMethodSymbol method && method.IsExtensionMethod)
                            {
                                yield return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.ExtensionMethd]);
                            }
                            break;
                        case SymbolKind.NamedType:
                            if (span.ClassificationType == ClassificationTypeNames.ClassName && symbol is INamedTypeSymbol type && type.TypeKind == TypeKind.Class && type.IsStatic)
                            {
                                yield return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.StaticClass]);
                            }
                            break;
                        case SymbolKind.Field:
                            yield return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.Field]);
                            break;
                        case SymbolKind.Property:
                            yield return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.Property]);
                            break;
                        case SymbolKind.Event:
                            yield return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.Event]);
                            break;
                    }
                }
            }
        }
    }
}