
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3035_DebugLogIsEnabledAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3035";

        private const string Debug = "Debug";
        private const string DebugFormat = "DebugFormat";
        private const string IsDebugEnabled = "IsDebugEnabled";

        public MiKo_3035_DebugLogIsEnabledAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression); // SimpleMemberAccessExpression

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;

            var diagnostic = AnalyzeInvocation(node, context.SemanticModel);
            if (diagnostic != null) context.ReportDiagnostic(diagnostic);
        }

        private Diagnostic AnalyzeInvocation(InvocationExpressionSyntax node, SemanticModel semanticModel) => node.Expression is MemberAccessExpressionSyntax methodCall
                                                                                                              ? Analyze(methodCall, semanticModel)
                                                                                                              : null;

        private Diagnostic Analyze(MemberAccessExpressionSyntax methodCall, SemanticModel semanticModel)
        {
            var methodName = methodCall.Name.ToString();
            switch (methodName)
            {
                case Debug:
                case DebugFormat:
                {
                    // check if inside IsDebugEnabled call for if or block
                    if (methodCall.IsInsideIfStatementWithCallTo(IsDebugEnabled))
                        return null;

                    // check for correct type (only ILog methods shall be reported)
                    var type = semanticModel.GetTypeInfo(methodCall.Expression).Type;

                    if (type.Name != "ILog")
                        return null;

                    var method = methodCall.GetEnclosingMethod(semanticModel);

                    return ReportIssue(method, methodCall.GetLocation(), methodName, IsDebugEnabled);
                }
            }

            return null;
        }
    }
}