using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3301_CodeFixProvider)), Shared]
    public sealed class MiKo_3301_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3301_ParenthesizedLambdaExpressionUsesExpressionBodyAnalyzer.Id;

        protected override string Title => Resources.MiKo_3301_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParenthesizedLambdaExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var parenthesized = (ParenthesizedLambdaExpressionSyntax)syntax;

            var body = GetBody(parenthesized);

            if (body is null)
            {
                // we cannot fix it
                return parenthesized;
            }

            var parameterList = parenthesized.ParameterList;
            var parameters = parameterList.Parameters;

            if (parameters.Count == 1)
            {
                return SyntaxFactory.SimpleLambdaExpression(parameters.First(), body);
            }

            return SyntaxFactory.ParenthesizedLambdaExpression(parameterList, body);
        }

        private static CSharpSyntaxNode GetBody(ParenthesizedLambdaExpressionSyntax node)
        {
            var body = node.Block.FirstChild<CSharpSyntaxNode>();

            switch (body)
            {
                case ReturnStatementSyntax statement:
                    return statement.Expression;

                case ExpressionStatementSyntax statement:
                    return statement.Expression;

                case ThrowStatementSyntax statement when statement.Expression != null:
                    return SyntaxFactory.ThrowExpression(statement.Expression);

                default:
                    return body;
            }
        }
    }
}