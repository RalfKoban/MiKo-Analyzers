using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3082_UsePatternMatchingForEqualsExpressionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3082";

        public MiKo_3082_UsePatternMatchingForEqualsExpressionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLogicalNotExpression, SyntaxKind.EqualsExpression);

        private void AnalyzeLogicalNotExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (BinaryExpressionSyntax)context.Node;

            if (IsBooleanComparison(node.Left) || IsBooleanComparison(node.Right))
            {
                var location = node.OperatorToken.GetLocation();
                var issue = Issue(string.Empty, location);
                context.ReportDiagnostic(issue);
            }
        }

        private static bool IsBooleanComparison(CSharpSyntaxNode syntax) => syntax != null && IsBooleanComparison(syntax.Kind());

        private static bool IsBooleanComparison(SyntaxKind kind) => kind == SyntaxKind.TrueLiteralExpression || kind == SyntaxKind.FalseLiteralExpression;
    }
}