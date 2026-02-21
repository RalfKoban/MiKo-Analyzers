using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<IfStatementSyntax>().FirstOrDefault();

        protected override async Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
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

                    if (subPatterns.Count is 1)
                    {
                        var subPattern = subPatterns[0];

                        if (subPattern.Pattern is ConstantPatternSyntax constantPattern && constantPattern.Expression is LiteralExpressionSyntax literal)
                        {
                            var expressionKind = useIsNotPattern ? SyntaxKind.NotEqualsExpression : SyntaxKind.EqualsExpression;

                            var updatedCondition = await GetUpdatedConditionAsync(p.Expression, subPattern.NameColon.GetName(), expressionKind, literal, document, cancellationToken).ConfigureAwait(false);

                            if (useIsNotPattern && updatedCondition is IsPatternExpressionSyntax up)
                            {
                                updatedCondition = IsNotPattern(up);
                            }

                            return statement.WithCondition(updatedCondition);
                        }
                    }
                }
            }

            return null;
        }

        private static async Task<ExpressionSyntax> GetUpdatedConditionAsync(ExpressionSyntax expression, string name, SyntaxKind expressionKind, LiteralExpressionSyntax literal, Document document, CancellationToken cancellationToken)
        {
            var operand = await GetUpdatedExpressionAsync(expression, name, document, cancellationToken).ConfigureAwait(false);

            switch (literal.Kind())
            {
                case SyntaxKind.TrueLiteralExpression: return IsTruePattern(operand);
                case SyntaxKind.FalseLiteralExpression: return IsFalsePattern(operand);
                case SyntaxKind.NullLiteralExpression: return IsNullPattern(operand);
                default:
                    return SyntaxFactory.BinaryExpression(expressionKind, operand, literal.WithoutTrivia());
            }
        }

        private static async Task<ExpressionSyntax> GetUpdatedExpressionAsync(ExpressionSyntax expression, string name, Document document, CancellationToken cancellationToken)
        {
            var isNullable = await expression.IsNullableAsync(document, cancellationToken).ConfigureAwait(false);

            if (isNullable)
            {
                return ConditionalAccess(expression, name);
            }

            return Member(expression, name);
        }
    }
}