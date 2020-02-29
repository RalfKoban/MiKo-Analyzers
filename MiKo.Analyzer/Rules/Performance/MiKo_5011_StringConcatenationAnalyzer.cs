using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5011_StringConcatenationAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5011";

        public MiKo_5011_StringConcatenationAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeAddAssignmentExpression, SyntaxKind.AddAssignmentExpression);

        private void AnalyzeAddAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (AssignmentExpressionSyntax)context.Node;

            var diagnostic = AnalyzeAddAssignmentExpression(node, context.SemanticModel);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic AnalyzeAddAssignmentExpression(AssignmentExpressionSyntax node, SemanticModel semanticModel) => node.Left.IsString(semanticModel)
                                                                                                                               ? Issue(node.OperatorToken)
                                                                                                                               : null;
    }
}