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

        public MiKo_5002_DebugFormatInsteadDebugLogAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override bool IsApplicable(CompilationStartAnalysisContext context) => context.Compilation.GetTypeByMetadataName(Constants.ILog.FullTypeName) != null;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;
            var issue = AnalyzeInvocation(node, context.SemanticModel);

            ReportDiagnostics(context, issue);
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
                    // only ILog methods shall be reported
                    var type = methodCall.GetTypeSymbol(semanticModel);

                    // it may happen that in some broken code Roslyn is unable to detect a type (e.g. due to missing code paths), hence 'type' could be null here
                    if (type?.Name != Constants.ILog.TypeName)
                    {
                        return null;
                    }

                    if (methodCall.IsInsideMoqCall())
                    {
                        // ignore Moq calls
                        return null;
                    }

                    var enclosingMethod = methodCall.GetEnclosingMethod(semanticModel);

                    return Issue(enclosingMethod.Name, methodCall.Name, methodName, methodName.Without("Format"));
                }

                default:
                    return null;
            }
        }
    }
}