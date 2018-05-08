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
    internal struct SpanMeta
    {
        public SyntaxToken Token { get; }
        public SyntaxNode Node { get; }
        public ISymbol Symbol { get; }
        public bool HasSymbol => Symbol != null;

        public SpanMeta(SyntaxNode node, ISymbol symbol)
        {
            Node = node;
            Symbol = symbol;
            Token = default(SyntaxToken);
        }

        public SpanMeta(SyntaxToken token)
        {
            Node = null;
            Symbol = null;
            Token = token;
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
                .SelectMany(s => Classifier.GetClassifiedSpans(SemanticModel, s, Workspace));
                
        }

        public SpanMeta GetMeta(TextSpan span)
        {
            var node = SyntaxRoot.FindNode(span)?.GetExpression();
            if (node != null)
            {
                var symbol = SemanticModel.GetSymbolInfo(node).Symbol ??
                    SemanticModel.GetDeclaredSymbol(node) ??
                    SemanticModel.GetTypeInfo(node).Type;
                return new SpanMeta(node, symbol);
            }
            var child = SyntaxRoot.ChildThatContainsPosition(span.Start);
            return new SpanMeta(child.AsToken());
        }

        private Thingy() { }

        public static async Task<Thingy> Get(ITextBuffer buffer, ITextSnapshot snapshot)
        {
            var workspace = buffer.GetWorkspace();
            var document = snapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (document == null)
            {
                return null;
            }
            var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);
            var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

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