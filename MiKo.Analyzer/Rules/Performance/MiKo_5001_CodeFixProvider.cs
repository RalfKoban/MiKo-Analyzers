using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_5001_CodeFixProvider)), Shared]
    public sealed class MiKo_5001_CodeFixProvider : PerformanceCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_5001_DebugLogIsEnabledAnalyzer.Id;

        protected override string Title => Resources.MiKo_5001_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ExpressionStatementSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var statement = (ExpressionStatementSyntax)syntax;
            var call = (InvocationExpressionSyntax)statement.Expression;
            var expression = (MemberAccessExpressionSyntax)call.Expression;

            var condition = CreateCondition(expression);

            // nest call in block
            var block = SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(call));

            return SyntaxFactory.IfStatement(condition, block);
        }

        private static ExpressionSyntax CreateCondition(MemberAccessExpressionSyntax expression)
        {
            var identifier = GetIdentifier(expression);
            var method = SyntaxFactory.IdentifierName(Constants.ILog.IsDebugEnabled);
            var condition = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, identifier, method);

            return condition;
        }

        private static ExpressionSyntax GetIdentifier(MemberAccessExpressionSyntax expression)
        {
            switch (expression.Expression)
            {
                case IdentifierNameSyntax i:
                    return SyntaxFactory.IdentifierName(i.GetName());

                case MemberAccessExpressionSyntax m:
                    return m.WithoutLeadingTrivia();

                default:
                    return null;
            }
        }
    }
}