using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3081_CodeFixProvider)), Shared]
    public sealed class MiKo_3081_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3081_UsePatternMatchingForLogicalNotExpressionAnalyzer.Id;

        protected override string Title => Resources.MiKo_3081_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.First(_ => _.IsKind(SyntaxKind.LogicalNotExpression));

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var unary = (PrefixUnaryExpressionSyntax)syntax;

            var falsePattern = SyntaxFactory.ConstantPattern(Literal(SyntaxKind.FalseLiteralExpression));
            var pattern = SyntaxFactory.IsPatternExpression(unary.Operand, falsePattern);

            return pattern;
        }
    }
}