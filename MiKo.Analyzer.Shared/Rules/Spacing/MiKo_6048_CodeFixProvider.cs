using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6048_CodeFixProvider)), Shared]
    public sealed class MiKo_6048_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6048";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntax(syntax);

        private static T GetUpdatedSyntax<T>(T syntax) where T : SyntaxNode
        {
            switch (syntax)
            {
                case BinaryExpressionSyntax binary:
                {
                    var updated = binary.WithLeft(GetUpdatedSyntax(binary.Left))
                                        .WithOperatorToken(binary.OperatorToken.WithoutTrivia().WithLeadingSpace())
                                        .WithRight(GetUpdatedSyntax(binary.Right));

                    return updated as T;
                }

                case ParenthesizedExpressionSyntax parenthesized:
                {
                    var updated = parenthesized.WithExpression(GetUpdatedSyntax(parenthesized.Expression));

                    return updated as T;
                }

                default:
                    return syntax.WithoutTrivia();
            }
        }
    }
}