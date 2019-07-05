using System.Collections.Generic;

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
                                                                        };

        public MiKo_3084_YodaExpressionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpressionLanguageAware, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);

        private static bool IsResponsibleNode(CSharpSyntaxNode syntax) => syntax != null && IsResponsibleNode(syntax.Kind());

        private static bool IsResponsibleNode(SyntaxKind kind) => ExpressionValues.Contains(kind);

        private void AnalyzeExpressionLanguageAware(SyntaxNodeAnalysisContext context)
        {
            if (context.IsSupported(LanguageVersion.CSharp7))
            {
                AnalyzeExpression(context);
            }
        }

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (BinaryExpressionSyntax)context.Node;

            var literal = node.Left;

            if (IsResponsibleNode(literal))
            {
                ReportIssue(context, node.OperatorToken, literal);
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