using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3019_DoNotThrowInsideDisposeAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3019";

        public MiKo_3019_DoNotThrowInsideDisposeAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeThrow, SyntaxKind.ThrowStatement, SyntaxKind.ThrowExpression);

        private static bool HasIssue(IMethodSymbol method)
        {
            switch (method.MethodKind)
            {
                case MethodKind.PropertyGet:
                case MethodKind.PropertySet:
                    return method.AssociatedSymbol?.Name.GetNameOnlyPart() is "IsDisposed"; // IsDisposed properties are NOT allowed to throw because they should return a value indicating whether the instance is already disposed

                case MethodKind.ExplicitInterfaceImplementation:
                case MethodKind.Ordinary:
                {
                    switch (method.Name.GetNameOnlyPart())
                    {
                        case "Dispose":
                        case "DisposeCore":
                        case "DisposeAsync":
                        case "DisposeCoreAsync":
                            return true; // Dispose methods are NOT allowed to throw because they should support re-entrance

                        default:
                            return false;
                    }
                }
            }

            return false;
        }

        private void AnalyzeThrow(SyntaxNodeAnalysisContext context)
        {
            if (context.ContainingSymbol is IMethodSymbol method && HasIssue(method))
            {
                ReportDiagnostics(context, Issue(context.Node));
            }
        }
    }
}