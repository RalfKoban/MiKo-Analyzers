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
            if (syntax is IdentifierNameSyntax i)
            {
                var t = context.FindContainingType();
                var isConst = t.GetMembers(i.Identifier.ValueText).OfType<IFieldSymbol>().Any(_ => _.IsConst);
                return isConst;
            }

            return false;
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

        private void ReportIssue(SyntaxNodeAnalysisContext context, SyntaxToken token, CSharpSyntaxNode node)
        {
            var location = node.GetLocation();
            var issue = Issue(string.Empty, location, token.ValueText);
            context.ReportDiagnostic(issue);
        }
    }
}