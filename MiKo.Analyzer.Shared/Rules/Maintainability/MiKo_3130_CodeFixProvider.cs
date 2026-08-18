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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3130_CodeFixProvider)), Shared]
    public sealed class MiKo_3130_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3130";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<IfStatementSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken) => Task.FromResult(syntax);

        protected override async Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var updatedRoot = GetUpdatedSyntaxRoot(root, syntax, issue, semanticModel);

            return updatedRoot;
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root, SyntaxNode syntax, Diagnostic issue, SemanticModel semanticModel)
        {
            var node = root.FindNode(issue.Location.SourceSpan);

            if (syntax is IfStatementSyntax ifStatement && node is InvocationExpressionSyntax invocation && invocation.Parent is ExpressionStatementSyntax assertFailStatement)
            {
                var whenTrue = ifStatement.Statement;

                var belowIf = whenTrue.DescendantNodesAndSelf().OfType<StatementSyntax>().Any(_ => _.IsEquivalentTo(assertFailStatement));

                var updatedArguments = UpdateArguments(ifStatement.Condition, belowIf, semanticModel);

                var assert = AssertThat(updatedArguments.Condition, updatedArguments.Constraint, invocation.ArgumentList.Arguments, 0);
                var assertStatement = Statement(assert).WithTriviaFrom(ifStatement);

                if (belowIf)
                {
                    return root.ReplaceNode(ifStatement, assertStatement);
                }

                if (whenTrue is BlockSyntax block)
                {
                    // copy the statements from the 'true' block as we still need them
                    var allStatements = block.Statements.Select(_ => _.WithIndentation()).ToSyntaxList()
                                             .Insert(0, assertStatement); // place assert first

                    return root.ReplaceNode(ifStatement, allStatements);
                }

                return root.ReplaceNode(ifStatement, new[] { assertStatement, whenTrue });
            }

            return root;
        }

        private static (ArgumentSyntax Condition, ArgumentSyntax Constraint) UpdateArguments(ExpressionSyntax condition, in bool belowIf, SemanticModel semanticModel)
        {
            switch (condition)
            {
                case PrefixUnaryExpressionSyntax unary when unary.IsKind(SyntaxKind.LogicalNotExpression):
                {
                    // we are not interested in the unary not condition, so we have to switch both the condition and the 'Is' part
                    return (Argument(unary.Operand), GetConstraintForFalse(belowIf));
                }

                case BinaryExpressionSyntax binary:
                {
                    var left = binary.Left;
                    var right = binary.Right;

                    switch (binary.Kind())
                    {
                        case SyntaxKind.EqualsExpression when right is LiteralExpressionSyntax l: return (Argument(left), GetConstraint(belowIf, l));
                        case SyntaxKind.EqualsExpression when left is LiteralExpressionSyntax l: return (Argument(right), GetConstraint(belowIf, l));
                        case SyntaxKind.EqualsExpression when right.IsEnumMember(semanticModel): return (Argument(left), GetConstraint(belowIf, right));
                        case SyntaxKind.EqualsExpression when left.IsEnumMember(semanticModel): return (Argument(right), GetConstraint(belowIf, left));

                        case SyntaxKind.NotEqualsExpression when right is LiteralExpressionSyntax l: return (Argument(left), GetConstraint(!belowIf, l));
                        case SyntaxKind.NotEqualsExpression when left is LiteralExpressionSyntax l: return (Argument(right), GetConstraint(!belowIf, l));
                        case SyntaxKind.NotEqualsExpression when right.IsEnumMember(semanticModel): return (Argument(left), GetConstraint(!belowIf, right));
                        case SyntaxKind.NotEqualsExpression when left.IsEnumMember(semanticModel): return (Argument(right), GetConstraint(!belowIf, left));

                        case SyntaxKind.IsExpression when right.IsEnum(semanticModel): return (Argument(left), GetConstraint(belowIf, right));
                    }

                    break;
                }

                case IsPatternExpressionSyntax isPattern:
                {
                    var expression = isPattern.Expression;

                    switch (isPattern.Pattern)
                    {
                        case ConstantPatternSyntax c when c.Expression is LiteralExpressionSyntax l:
                            return (Argument(expression), GetConstraint(belowIf, l));

                        case UnaryPatternSyntax u when u.Pattern is ConstantPatternSyntax c && c.Expression is LiteralExpressionSyntax l:
                            return (Argument(expression), GetConstraint(!belowIf, l));

                        case UnaryPatternSyntax u when u.Pattern is ConstantPatternSyntax c && c.Expression.IsEnumMember(semanticModel):
                            return (Argument(expression), GetConstraint(!belowIf, c.Expression));
                    }

                    break;
                }
            }

            return (Argument(condition), GetConstraintForTrue(belowIf));
        }

        private static ArgumentSyntax GetConstraint(in bool inverse, LiteralExpressionSyntax literal)
        {
            switch (literal.Kind())
            {
                case SyntaxKind.TrueLiteralExpression: return GetConstraintForTrue(inverse);
                case SyntaxKind.FalseLiteralExpression: return GetConstraintForFalse(inverse);
                case SyntaxKind.NullLiteralExpression: return GetConstraintForNull(inverse);
                default:
                    return GetConstraint(inverse, (ExpressionSyntax)literal);
            }
        }

        private static ArgumentSyntax GetConstraint(in bool inverse, ExpressionSyntax expression) => inverse ? Is("Not", "EqualTo", expression) : Is("EqualTo", expression);

        private static ArgumentSyntax GetConstraintForTrue(in bool inverse) => Is(inverse ? "False" : "True");

        private static ArgumentSyntax GetConstraintForFalse(in bool inverse) => Is(inverse ? "True" : "False");

        private static ArgumentSyntax GetConstraintForNull(in bool inverse) => inverse ? Is("Not", "Null") : Is("Null");
    }
}