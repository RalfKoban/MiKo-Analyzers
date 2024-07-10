using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3089_CodeFixProvider)), Shared]
    public sealed class MiKo_3089_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3089";

        protected override string Title => Resources.MiKo_3089_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<IfStatementSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is IfStatementSyntax statement && statement.Condition is IsPatternExpressionSyntax p && p.Pattern is RecursivePatternSyntax r && r.PropertyPatternClause is PropertyPatternClauseSyntax pattern)
            {
                var subPatterns = pattern.Subpatterns;

                if (subPatterns.Count == 1)
                {
                    var subPattern = subPatterns[0];

                    if (subPattern.Pattern is ConstantPatternSyntax constantPattern && constantPattern.Expression is LiteralExpressionSyntax literal)
                    {
                        var updatedCondition = GetUpdatedCondition(p.Expression, subPattern.NameColon.GetName(), literal);

                        return statement.WithCondition(updatedCondition);
                    }
                }
            }

            return base.GetUpdatedSyntax(document, syntax, issue);
        }

        private static ExpressionSyntax GetUpdatedCondition(ExpressionSyntax expression, string name, LiteralExpressionSyntax literal)
        {
            var operand = SimpleMemberAccess(expression, name);

            switch (literal.Kind())
            {
                case SyntaxKind.TrueLiteralExpression: return IsTruePattern(operand);
                case SyntaxKind.FalseLiteralExpression: return IsFalsePattern(operand);
                case SyntaxKind.NullLiteralExpression: return IsNullPattern(operand);
                default:
                    return SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, operand, literal.WithoutTrivia());
            }
        }
    }
}