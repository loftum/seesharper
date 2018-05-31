using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using SeeSharper.SyntaxColoring.Tags;
using System.Reflection;
using Microsoft.CodeAnalysis.Classification;

namespace SeeSharper.SyntaxColoring
{
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
            _clasificationTypes[ClassificationTypeNames.ClassName] = registry.GetClassificationType(ClassificationTypeNames.ClassName);
            _clasificationTypes[ClassificationTypeNames.EnumName] = registry.GetClassificationType(ClassificationTypeNames.EnumName);
            _clasificationTypes[ClassificationTypeNames.StructName] = registry.GetClassificationType(ClassificationTypeNames.StructName);
            _clasificationTypes[ClassificationTypeNames.InterfaceName] = registry.GetClassificationType(ClassificationTypeNames.InterfaceName);
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
                        if (symbol is IMethodSymbol method)
                        {
                            if (method.IsExtensionMethod && span.ClassificationType == "extension method name")
                            {
                                return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.ExtensionMethd]);
                            }
                            if (method.MethodKind == MethodKind.Constructor && span.ClassificationType == "identifier")
                            {
                                switch (method.ContainingType?.TypeKind)
                                {
                                    case TypeKind.Class when method.ContainingType.IsStatic:
                                    {
                                        return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.StaticClass]);
                                    }
                                    case TypeKind.Class:
                                        return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[ClassificationTypeNames.ClassName]);
                                    case TypeKind.Enum:
                                        return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[ClassificationTypeNames.EnumName]);
                                    case TypeKind.Struct:
                                        return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[ClassificationTypeNames.StructName]);
                                    default:
                                        return null;
                                }
                            }
                            return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.Method]);
                        }
                        return null;
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