using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using SeeSharper.SyntaxColoring.Tags;
using System.Reflection;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp;

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
        private readonly IDictionary<string, IClassificationType> _clasificationTypes = new Dictionary<string, IClassificationType>();
        private Thingy _thingy;

        public SemanticColorTagger(ITextBuffer buffer, IClassificationTypeRegistryService registry)
        {
            _buffer = buffer;

            foreach(string key in typeof(TagTypes).GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.FieldType == typeof(string)).Select(f => f.GetValue(null)))
            {
                _clasificationTypes[key] = registry.GetClassificationType(key);
            }
        }

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                return Enumerable.Empty<ITagSpan<IClassificationTag>>();
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
                    return Enumerable.Empty<ITagSpan<IClassificationTag>>();
                }
            }
            
            if (_thingy == null)
            {
                return Enumerable.Empty<ITagSpan<IClassificationTag>>();
            }
            return _thingy.GetClassifiedSpans(spans).Select(GetTagSpan).Where(s => s!= null);
            
        }

        private ITagSpan<IClassificationTag> GetTagSpan(ClassifiedSpan span)
        {
            var meta = _thingy.GetMeta(span.TextSpan);
            var s = meta.ToString();

            if (meta.HasSymbol)
            {
                var symbol = meta.Symbol;
                switch (symbol.Kind)
                {
                    case SymbolKind.Method:
                        if (span.ClassificationType == "extension method name" && symbol is IMethodSymbol method && method.IsExtensionMethod)
                        {
                            return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.ExtensionMethd]);
                        }
                        return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.Method]);
                    case SymbolKind.NamedType:
                        if (span.ClassificationType == ClassificationTypeNames.ClassName && symbol is INamedTypeSymbol type && type.TypeKind == TypeKind.Class && type.IsStatic)
                        {
                            return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.StaticClass]);
                        }
                        return null;
                    case SymbolKind.Field:
                        if (symbol is IFieldSymbol field && field.IsConst || symbol.ContainingType.TypeKind == TypeKind.Enum)
                        {
                            return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.Constant]);
                        }
                        return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.Field]);
                    case SymbolKind.Property:
                        return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.Property]);
                    case SymbolKind.Event:
                        return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.Event]);
                }
            }
            return null;
        }
    }
}