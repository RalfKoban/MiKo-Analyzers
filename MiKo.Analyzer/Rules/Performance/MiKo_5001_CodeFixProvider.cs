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

        protected override string Title => "Place inside 'if'";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ExpressionStatementSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var statement = (ExpressionStatementSyntax)syntax;
            var call = (InvocationExpressionSyntax)statement.Expression;

            // create condition
            var identifierName = (IdentifierNameSyntax)((MemberAccessExpressionSyntax)call.Expression).Expression;
            var identifier = SyntaxFactory.IdentifierName(identifierName.GetName());
            var method = SyntaxFactory.IdentifierName(MiKo_5001_DebugLogIsEnabledAnalyzer.IsDebugEnabled);
            var condition = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, identifier, method);

            // nest call in block
            var block = SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(call));

            return SyntaxFactory.IfStatement(condition, block);
        }
    }
}