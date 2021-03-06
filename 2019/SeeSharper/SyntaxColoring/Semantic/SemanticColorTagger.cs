﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using SeeSharper.Options;
using SeeSharper.SyntaxColoring.Tags;

namespace SeeSharper.SyntaxColoring.Semantic
{
    public class SemanticColorTagger : ITagger<IClassificationTag>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private readonly ITextBuffer _buffer;
        private readonly IOccurrenceTaggingOptions _options;
        private readonly IDictionary<string, IClassificationType> _clasificationTypes = new Dictionary<string, IClassificationType>();
        private Thingy _thingy;

        public SemanticColorTagger(ITextBuffer buffer, IClassificationTypeRegistryService registry, IOccurrenceTaggingOptions options)
        {
            _buffer = buffer;
            _options = options;

            foreach(string key in typeof(TagTypes).GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.FieldType == typeof(string)).Select(f => f.GetValue(null)))
            {
                _clasificationTypes[key] = registry.GetClassificationType(key);
            }
            _clasificationTypes[ClassificationTypeNames.ClassName] = registry.GetClassificationType(ClassificationTypeNames.ClassName);
            _clasificationTypes[ClassificationTypeNames.EnumName] = registry.GetClassificationType(ClassificationTypeNames.EnumName);
            _clasificationTypes[ClassificationTypeNames.StructName] = registry.GetClassificationType(ClassificationTypeNames.StructName);
            _clasificationTypes[ClassificationTypeNames.InterfaceName] = registry.GetClassificationType(ClassificationTypeNames.InterfaceName);
            _options.PropertyChanged += OnOptionsChanged;
        }

        ~SemanticColorTagger()
        {
            _options.PropertyChanged -= OnOptionsChanged;
        }

        private void OnOptionsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_options.SemanticColoringEnabled)
            {
                _thingy = null;
            }
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
        }

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (!_options.SemanticColoringEnabled || spans.Count == 0)
            {
                return Enumerable.Empty<ITagSpan<IClassificationTag>>();
            }

            var thingy = _thingy;
            if (thingy == null || thingy.Snapshot != spans[0].Snapshot)
            {
                try
                {
                    thingy = ThreadHelper.JoinableTaskFactory.Run(async () => await Thingy.GetAsync(_buffer, spans[0].Snapshot));
                    _thingy = thingy;
                }
                catch
                {
                    return Enumerable.Empty<ITagSpan<IClassificationTag>>();
                }
            }
            
            if (thingy == null)
            {
                return Enumerable.Empty<ITagSpan<IClassificationTag>>();
            }

            return thingy.GetClassifiedSpans(spans).Select(GetTagSpan).Where(s => s!= null);
        }

        private ITagSpan<IClassificationTag> GetTagSpan(ClassifiedSpan span)
        {
            var meta = _thingy.GetMeta(span.TextSpan);
            if (!meta.HasSymbol)
            {
                return null;
            }
            var symbol = meta.Symbol;
            switch (symbol.Kind)
            {
                case SymbolKind.Method:
                    switch (symbol)
                    {
                        case IMethodSymbol method:
                        {
                            if (method.IsExtensionMethod)
                            {
                                switch (span.ClassificationType)
                                {
                                    case ClassificationTypeNames.MethodName:
                                    case ClassificationTypeNames.ExtensionMethodName:
                                    case ClassificationTypeNames.Identifier:
                                        return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.ExtensionMethod]);
                                    default:
                                        if (meta.Node is MethodDeclarationSyntax declaration) // Stupid Classifier.
                                        {
                                            return declaration.Identifier.Span.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.ExtensionMethod]);
                                        }
                                        return null;
                                }
                            }

                            switch (method.MethodKind)
                            {
                                case MethodKind.StaticConstructor:
                                    switch (span.ClassificationType)
                                    {
                                        case ClassificationTypeNames.ClassName:
                                            return span.TextSpan.ToTagSpan(_thingy.Snapshot, _clasificationTypes[TagTypes.StaticClass]);
                                        default:
                                            return null;
                                    }
                                case MethodKind.Constructor:
                                    switch (span.ClassificationType)
                                    {
                                        case ClassificationTypeNames.Identifier:
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
                                        default:
                                            return null;
                                    }
                                default:
                                    return null;
                            }
                        }
                        default:
                            return null;
                    }
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
            return null;
        }
    }
}