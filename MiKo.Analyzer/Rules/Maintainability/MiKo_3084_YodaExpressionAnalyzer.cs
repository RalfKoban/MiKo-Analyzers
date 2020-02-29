using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3084_YodaExpressionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3084";

        private static readonly HashSet<SyntaxKind> ExpressionValues = new HashSet<SyntaxKind>
                                                                        {
                                                                            SyntaxKind.TrueLiteralExpression,
                                                                            SyntaxKind.FalseLiteralExpression,
                                                                            SyntaxKind.NullLiteralExpression,
                                                                            SyntaxKind.NumericLiteralExpression,
                                                                            SyntaxKind.StringLiteralExpression,
                                                                        };

        private static readonly SyntaxKind[] Expressions =
            {
                SyntaxKind.EqualsExpression,
                SyntaxKind.NotEqualsExpression,
                SyntaxKind.LessThanExpression,
                SyntaxKind.LessThanOrEqualExpression,
                SyntaxKind.GreaterThanExpression,
                SyntaxKind.GreaterThanOrEqualExpression,
            };

        public MiKo_3084_YodaExpressionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpression, Expressions);

        private static bool IsResponsibleNode(CSharpSyntaxNode syntax) => syntax != null && IsResponsibleNode(syntax.Kind());

        private static bool IsResponsibleNode(SyntaxKind kind) => ExpressionValues.Contains(kind);

        private static bool IsConst(SyntaxNode syntax, SyntaxNodeAnalysisContext context)
        {
            switch (syntax)
            {
                case IdentifierNameSyntax i:
                {
                    var type = context.FindContainingType();
                    var isConst = type.GetMembers(i.GetName()).OfType<IFieldSymbol>().Any(_ => _.IsConst);
                    return isConst;
                }

                case MemberAccessExpressionSyntax m when m.IsKind(SyntaxKind.SimpleMemberAccessExpression):
                {
                    var type = m.Expression.GetTypeSymbol(context.SemanticModel);

                    // only get the real enum members, no local variables or something
                    return type?.IsEnum() is true;
                }

                default:
                {
                    return false;
                }
            }
        }

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (BinaryExpressionSyntax)context.Node;

            var left = node.Left;

            if (IsResponsibleNode(left) || IsConst(left, context))
            {
                ReportIssue(context, node.OperatorToken, left);
            }
        }

        private void ReportIssue(SyntaxNodeAnalysisContext context, SyntaxToken token, SyntaxNode node)
        {
            var issue = Issue(string.Empty, node, token.ValueText);
            context.ReportDiagnostic(issue);
        }
    }
}