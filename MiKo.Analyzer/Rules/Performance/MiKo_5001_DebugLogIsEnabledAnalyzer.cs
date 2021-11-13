using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5001_DebugLogIsEnabledAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5001";

        public MiKo_5001_DebugLogIsEnabledAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;

            var diagnostic = AnalyzeInvocation(node, context.SemanticModel);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic AnalyzeInvocation(InvocationExpressionSyntax node, SemanticModel semanticModel) => node.Expression is MemberAccessExpressionSyntax methodCall
                                                                                                              ? Analyze(methodCall, semanticModel)
                                                                                                              : null;

        private Diagnostic Analyze(MemberAccessExpressionSyntax methodCall, SemanticModel semanticModel)
        {
            var methodName = methodCall.GetName();
            switch (methodName)
            {
                case Constants.ILog.Debug:
                case Constants.ILog.DebugFormat:
                {
                    if (methodCall.IsInsideIfStatementWithCallTo(Constants.ILog.IsDebugEnabled))
                    {
                        // skip call if inside IsDebugEnabled call for if or block
                        return null;
                    }

                    // only ILog methods shall be reported
                    var type = methodCall.GetTypeSymbol(semanticModel);
                    if (type.Name != Constants.ILog.TypeName)
                    {
                        // skip it as it's no matching type
                        return null;
                    }

                    if (type.GetMembers(Constants.ILog.IsDebugEnabled).None())
                    {
                        // skip it as it has no matching method
                        return null;
                    }

                    var enclosingMethod = methodCall.GetEnclosingMethod(semanticModel);

                    return Issue(enclosingMethod.Name, methodCall.Parent, methodName, Constants.ILog.IsDebugEnabled);
                }

                default:
                {
                    return null;
                }
            }
        }
    }
}