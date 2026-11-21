using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3247_DoNotUseNullForCollectionArgumentsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3247";

        public MiKo_3247_DoNotUseNullForCollectionArgumentsAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNull, SyntaxKind.NullLiteralExpression);

        private void AnalyzeNull(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is LiteralExpressionSyntax literal && literal.Parent is ArgumentSyntax a && a.Parent is ArgumentListSyntax al && al.Parent is InvocationExpressionSyntax invocation)
            {
                if (invocation.GetSymbol(context.SemanticModel) is IMethodSymbol method)
                {
                    var index = al.Arguments.IndexOf(a);
                    var parameter = method.Parameters[index];

                    if (parameter.IsParams || parameter.Type.IsEnumerable())
                    {
                        ReportDiagnostics(context, Issue(literal));
                    }
                }
            }
        }
    }
}