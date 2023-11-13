using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5017_StringLiteralVariableAssignmentIsConstantAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5017";

        public MiKo_5017_StringLiteralVariableAssignmentIsConstantAnalyzer() : base(Id)
        {
        }

        protected override DiagnosticSeverity Severity => DiagnosticSeverity.Info;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeStringLiteral, SyntaxKind.StringLiteralExpression);

        private void AnalyzeStringLiteral(SyntaxNodeAnalysisContext context) => ReportDiagnostics(context, AnalyzeStringLiteral(context.Node));

        private Diagnostic AnalyzeStringLiteral(SyntaxNode node)
        {
            if (node is LiteralExpressionSyntax literal && literal.Parent is EqualsValueClauseSyntax e && e.Parent is VariableDeclaratorSyntax v && v.Parent is VariableDeclarationSyntax declaration)
            {
                switch (declaration.Parent)
                {
                    case LocalDeclarationStatementSyntax localVariable when localVariable.IsConst is false:
                    {
                        return Issue(localVariable);
                    }

                    case FieldDeclarationSyntax field when field.IsConst() is false:
                    {
                        return Issue(field);
                    }
                }
            }

            return null;
        }
    }
}