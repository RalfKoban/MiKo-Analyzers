using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3201_CodeFixProvider)), Shared]
    public sealed class MiKo_3201_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        private static readonly Dictionary<SyntaxKind, SyntaxKind> OperatorMapping = new Dictionary<SyntaxKind, SyntaxKind>
                                                                                         {
                                                                                             { SyntaxKind.ExclamationEqualsToken, SyntaxKind.EqualsEqualsToken },
                                                                                             { SyntaxKind.EqualsEqualsToken, SyntaxKind.ExclamationEqualsToken },
                                                                                             { SyntaxKind.GreaterThanEqualsToken, SyntaxKind.LessThanToken },
                                                                                             { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanEqualsToken },
                                                                                             { SyntaxKind.LessThanEqualsToken, SyntaxKind.GreaterThanToken },
                                                                                             { SyntaxKind.LessThanToken, SyntaxKind.GreaterThanEqualsToken },
                                                                                         };

        public override string FixableDiagnosticId => MiKo_3201_InvertIfWhenFollowedByFewCodeLinesAnalyzer.Id;

        protected override string Title => Resources.MiKo_3201_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<IfStatementSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => syntax;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            if (syntax is IfStatementSyntax ifStatement && syntax.Parent is BlockSyntax block)
            {
                var statements = block.Statements.ToList();

                var index = statements.IndexOf(ifStatement);

                if (index < statements.Count)
                {
                    var others = statements.Skip(index + 1).Select(_ => _.WithAdditionalLeadingSpaces(Constants.Indentation)); // adjust spacing

                    var condition = ifStatement.Condition;
                    var newIf = ifStatement.WithCondition(InvertCondition(document, condition).WithTriviaFrom(condition))
                                           .WithStatement(SyntaxFactory.Block(others));

                    return root.ReplaceNodes(statements.Skip(index), (original, rewritten) => original == ifStatement ? newIf : null);
                }
            }

            return root;
        }

        private static ExpressionSyntax InvertCondition(Document document, ExpressionSyntax condition)
        {
            switch (condition)
            {
                case PrefixUnaryExpressionSyntax prefixed:
                {
                    return prefixed.Operand;
                }

                case BinaryExpressionSyntax binary when binary.Right.IsKind(SyntaxKind.NullLiteralExpression) && binary.OperatorToken.IsKind(SyntaxKind.ExclamationEqualsToken):
                {
                    return IsNullPattern(binary.Left);
                }

                case BinaryExpressionSyntax binary when OperatorMapping.TryGetValue(binary.OperatorToken.Kind(), out var replacement):
                {
                    return binary.WithOperatorToken(SyntaxFactory.Token(replacement));
                }

                case IsPatternExpressionSyntax pattern:
                {
                    if (pattern.Pattern is ConstantPatternSyntax c && c.Expression is LiteralExpressionSyntax literal)
                    {
                        switch (literal.Kind())
                        {
                            case SyntaxKind.NullLiteralExpression:
                                return SyntaxFactory.BinaryExpression(SyntaxKind.NotEqualsExpression, pattern.Expression, literal);

                            case SyntaxKind.FalseLiteralExpression:
                                return IsNullable(document, pattern)
                                       ? LogicalNot(pattern)
                                       : pattern.Expression.WithoutTrivia();

                            case SyntaxKind.TrueLiteralExpression:
                                return IsNullable(document, pattern)
                                       ? LogicalNot(pattern)
                                       : pattern.Expression.WithoutTrivia();
                        }
                    }

                    break;
                }
            }

            return IsFalsePattern(condition);
        }

        private static bool IsNullable(Document document, IsPatternExpressionSyntax pattern) => GetSymbol(document, pattern.Expression) is ITypeSymbol typeSymbol && typeSymbol.IsNullable();

        private static ExpressionSyntax LogicalNot(ExpressionSyntax expression) => SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, SyntaxFactory.ParenthesizedExpression(expression));
    }
}