using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SeeSharper.SyntaxColoring.Semantic
{
    internal static class SyntaxNodeExtensions
    {
        public static SyntaxNode GetExpression(this SyntaxNode node)
        {
            if (node.Kind() == SyntaxKind.Argument)
            {
                return ((ArgumentSyntax)node).Expression;
            }
            if (node.Kind() == SyntaxKind.AttributeArgument)
            {
                return ((AttributeArgumentSyntax)node).Expression;
            }
            return node;
        }
    }
}