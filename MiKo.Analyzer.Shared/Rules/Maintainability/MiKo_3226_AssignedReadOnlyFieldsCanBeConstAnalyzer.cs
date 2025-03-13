using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3226_AssignedReadOnlyFieldsCanBeConstAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3226";

        public MiKo_3226_AssignedReadOnlyFieldsCanBeConstAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);

        private void AnalyzeField(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is FieldDeclarationSyntax field && field.IsReadOnly())
            {
                foreach (var variable in field.Declaration.Variables)
                {
                    if (variable.Initializer?.Value is LiteralExpressionSyntax)
                    {
                        ReportDiagnostics(context, Issue(field));
                    }
                }
            }
        }
    }
}