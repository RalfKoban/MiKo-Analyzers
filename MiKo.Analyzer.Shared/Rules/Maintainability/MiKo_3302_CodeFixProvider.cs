using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3302_CodeFixProvider)), Shared]
    public sealed class MiKo_3302_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3302_SimpleLambdaExpressionIsUsedInsteadOfParenthesizedLambdaExpressionAnalyzer.Id;

        protected override string Title => Resources.MiKo_3302_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParenthesizedLambdaExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var parenthesized = (ParenthesizedLambdaExpressionSyntax)syntax;

            var body = parenthesized.ExpressionBody;

            if (body is null)
            {
                // we cannot fix it
                return parenthesized;
            }

            var parameter = parenthesized.ParameterList.Parameters.First();
            return SyntaxFactory.SimpleLambdaExpression(parameter, body);
        }
    }
}