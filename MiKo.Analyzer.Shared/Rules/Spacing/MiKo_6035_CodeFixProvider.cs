using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6035_CodeFixProvider)), Shared]
    public sealed class MiKo_6035_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6035";

        protected override string Title => Resources.MiKo_6035_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is InvocationExpressionSyntax invocation)
            {
                return invocation.WithArgumentList(invocation.ArgumentList.WithoutLeadingTrivia())
                                 .WithExpression(GetUpdatedExpression(invocation.Expression).WithoutTrailingTrivia());
            }

            return syntax;
        }

        private static ExpressionSyntax GetUpdatedExpression(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case MemberAccessExpressionSyntax maes:
                {
                    return maes.WithName((SimpleNameSyntax)GetUpdatedExpression(maes.Name));
                }

                case GenericNameSyntax genericName:
                {
                    var types = genericName.TypeArgumentList;
                    var arguments = types.Arguments;

                    var separators = Enumerable.Repeat(arguments.GetSeparator(0).WithoutTrivia().WithTrailingSpace(), arguments.Count - 1);

                    return genericName.WithTypeArgumentList(types.WithArguments(SyntaxFactory.SeparatedList(arguments.Select(_ => _.WithoutTrivia()), separators)));
                }

                default:
                {
                    return expression;
                }
            }
        }
    }
}