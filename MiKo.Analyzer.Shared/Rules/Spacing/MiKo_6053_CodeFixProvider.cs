using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6051_CodeFixProvider)), Shared]
    public sealed class MiKo_6053_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6053";

        protected override string Title => Resources.MiKo_6053_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ArgumentSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ArgumentSyntax argument)
            {
                if (argument.Expression is MemberAccessExpressionSyntax expression)
                {
                    var updatedExpression = expression.WithExpression(expression.Expression.WithoutTrailingTrivia())
                                                      .WithOperatorToken(expression.OperatorToken.WithoutTrivia())
                                                      .WithName(expression.Name.WithoutLeadingTrivia());

                    return argument.WithExpression(updatedExpression);
                }
            }

            return syntax;
        }
    }
}