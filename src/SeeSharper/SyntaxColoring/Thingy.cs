using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;

namespace SeeSharper.SyntaxColoring
{
    internal static class ValueExtensions
    {
        public static bool In<T>(this T value, params T[] values)
        {
            return values.Contains(value);
        }
    }

    internal class Thingy
    {
        public Workspace Workspace { get; private set; }
        public Document Document { get; private set; }
        public SemanticModel SemanticModel { get; private set; }
        public SyntaxNode SyntaxRoot { get; private set; }
        public ITextSnapshot Snapshot { get; private set; }

        public IEnumerable<ClassifiedSpan> GetClassifiedSpans(NormalizedSnapshotSpanCollection spans)
        {
            return spans
                .Select(s => TextSpan.FromBounds(s.Start, s.End))
                .SelectMany(s => Classifier.GetClassifiedSpans(SemanticModel, s, Workspace))
                .Where(cs => cs.ClassificationType.In(
                    ClassificationTypeNames.Identifier,
                    ClassificationTypeNames.ClassName,
                    ClassificationTypeNames.StructName,
                    ClassificationTypeNames.EnumName,
                    ClassificationTypeNames.DelegateName,
                    ClassificationTypeNames.ExcludedCode)
                );
        }

        public ISymbol GetSymbol(ClassifiedSpan span)
        {
            var node = SyntaxRoot.FindNode(span.TextSpan);
            return SemanticModel.GetSymbolInfo(node).Symbol ?? SemanticModel.GetDeclaredSymbol(node);
        }

        private Thingy() { }

        public static Thingy Get(ITextBuffer buffer, ITextSnapshot snapshot)
        {
            var workspace = buffer.GetWorkspace();
            var document = snapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (document == null)
            {
                return null;
            }

            if (!document.TryGetSemanticModel(out var semanticModel))
            {
                return null;
            }

            if (!document.TryGetSyntaxRoot(out var syntaxRoot))
            {
                return null;
            }
            return new Thingy
            {
                Workspace = workspace,
                Document = document,
                SemanticModel = semanticModel,
                SyntaxRoot = syntaxRoot,
                Snapshot = snapshot,
            };
        }
    }
}