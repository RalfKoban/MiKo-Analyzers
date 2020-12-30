using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5002_DebugFormatInsteadDebugLogAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5002";

        internal const string Format = nameof(Format);

        public MiKo_5002_DebugFormatInsteadDebugLogAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;

            var diagnostic = AnalyzeInvocation(node, context.SemanticModel);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic AnalyzeInvocation(InvocationExpressionSyntax node, SemanticModel semanticModel) => node.Expression is MemberAccessExpressionSyntax methodCall && node.ArgumentList.Arguments.Count == 1
                                                                                                                  ? Analyze(methodCall, semanticModel)
                                                                                                                  : null;

        private Diagnostic Analyze(MemberAccessExpressionSyntax methodCall, SemanticModel semanticModel)
        {
            var methodName = methodCall.GetName();
            switch (methodName)
            {
                case Constants.ILog.DebugFormat:
                case Constants.ILog.InfoFormat:
                case Constants.ILog.WarnFormat:
                case Constants.ILog.ErrorFormat:
                case Constants.ILog.FatalFormat:
                {
                    // check for correct type (only ILog methods shall be reported)
                    var type = methodCall.GetTypeSymbol(semanticModel);

                    if (type.Name != Constants.ILog.TypeName)
                    {
                        return null;
                    }

                    var enclosingMethod = methodCall.GetEnclosingMethod(semanticModel);
                    return Issue(enclosingMethod.Name, methodCall.Name, methodName, methodName.Without(Format));
                }

                default:
                    return null;
            }
        }
    }
}