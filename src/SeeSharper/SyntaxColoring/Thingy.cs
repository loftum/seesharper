using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;

namespace SeeSharper.SyntaxColoring
{
    internal class Thingy
    {
        public Workspace Workspace { get; private set; }
        public Document Document { get; private set; }
        public SemanticModel SemanticModel { get; private set; }
        public SyntaxNode SyntaxRoot { get; private set; }
        public NormalizedSnapshotSpanCollection Spans { get; private set; }
        public ITextSnapshot Snapshot { get; private set; }

        public IEnumerable<ClassifiedSpan> GetClassifiedSpans()
        {
            var comparer = StringComparer.InvariantCultureIgnoreCase;
            var classifiedSpans =
                Spans.SelectMany(s => {
                    var textSpan = TextSpan.FromBounds(s.Start, s.End);
                    return Classifier.GetClassifiedSpans(SemanticModel, textSpan, Workspace);
                });

            return from cs in classifiedSpans
                where comparer.Compare(cs.ClassificationType, "identifier") == 0
                select cs;
        }

        public ISymbol GetSymbol(ClassifiedSpan span)
        {
            var node = SyntaxRoot.FindNode(span.TextSpan);
            return SemanticModel.GetSymbolInfo(node).Symbol ?? SemanticModel.GetDeclaredSymbol(node);
        }

        private Thingy() { }

        public static Thingy Get(ITextBuffer buffer, NormalizedSnapshotSpanCollection spans)
        {
            var workspace = buffer.GetWorkspace();
            var document = spans[0].Snapshot.GetOpenDocumentInCurrentContextWithChanges();
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
                Spans = spans,
                Snapshot = spans[0].Snapshot,
            };
        }
    }
}