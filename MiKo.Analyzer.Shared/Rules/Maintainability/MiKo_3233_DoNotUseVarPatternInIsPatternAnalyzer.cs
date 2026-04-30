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

        private static bool HasIssue(VarPatternSyntax pattern)
        {
            if (pattern.Designation is SingleVariableDesignationSyntax s && pattern.Parent is SyntaxNode parent)
            {
                if (parent.IsAnyKind(ParentKinds))
                {
                    if (parent.Parent is BinaryExpressionSyntax b && b.IsKind(SyntaxKind.LogicalAndExpression) && parent == b.Left && b.Right.IsNullCheck(s.Identifier.ValueText))
                    {
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        private void AnalyzeVarPattern(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is VarPatternSyntax pattern && HasIssue(pattern))
            {
                ReportDiagnostics(context, Issue(pattern.VarKeyword));
            }
        }
    }
}