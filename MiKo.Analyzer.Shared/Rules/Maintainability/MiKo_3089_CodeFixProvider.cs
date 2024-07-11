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
            if (syntax is IfStatementSyntax statement && statement.Condition is IsPatternExpressionSyntax p)
            {
                var useIsNotPattern = false;
                PatternSyntax pattern;

                if (p.Pattern is UnaryPatternSyntax u)
                {
                    useIsNotPattern = true;

                    pattern = u.Pattern;
                }
                else
                {
                    pattern = p.Pattern;
                }

                if (pattern is RecursivePatternSyntax r && r.PropertyPatternClause is PropertyPatternClauseSyntax clause)
                {
                    var subPatterns = clause.Subpatterns;

                    if (subPatterns.Count == 1)
                    {
                        var subPattern = subPatterns[0];

                        if (subPattern.Pattern is ConstantPatternSyntax constantPattern && constantPattern.Expression is LiteralExpressionSyntax literal)
                        {
                            var expressionKind = useIsNotPattern ? SyntaxKind.NotEqualsExpression : SyntaxKind.EqualsExpression;

                            var updatedCondition = GetUpdatedCondition(p.Expression, subPattern.NameColon.GetName(), expressionKind, literal);

                            if (useIsNotPattern && updatedCondition is IsPatternExpressionSyntax up)
                            {
                                updatedCondition = IsNotPattern(up);
                            }

                            return statement.WithCondition(updatedCondition);
                        }
                    }
                }
            }

            return base.GetUpdatedSyntax(document, syntax, issue);
        }

        private static ExpressionSyntax GetUpdatedCondition(ExpressionSyntax expression, string name, SyntaxKind expressionKind, LiteralExpressionSyntax literal)
        {
            var operand = SimpleMemberAccess(expression, name);

            switch (literal.Kind())
            {
                case SyntaxKind.TrueLiteralExpression: return IsTruePattern(operand);
                case SyntaxKind.FalseLiteralExpression: return IsFalsePattern(operand);
                case SyntaxKind.NullLiteralExpression: return IsNullPattern(operand);
                default:
                    return SyntaxFactory.BinaryExpression(expressionKind, operand, literal.WithoutTrivia());
            }
        }
    }
}