using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3233_DoNotUseVarPatternInIsPatternAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3233";

        private static readonly SyntaxKind[] ParentKinds = { SyntaxKind.IsPatternExpression, SyntaxKind.NotPattern };

        public MiKo_3233_DoNotUseVarPatternInIsPatternAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeVarPattern, SyntaxKind.VarPattern);

        private void AnalyzeVarPattern(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is VarPatternSyntax pattern && pattern.Parent.IsAnyKind(ParentKinds))
            {
                ReportDiagnostics(context, Issue(pattern.VarKeyword));
            }
        }
    }
}