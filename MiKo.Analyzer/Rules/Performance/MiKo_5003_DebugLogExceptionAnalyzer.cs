using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5003_DebugLogExceptionAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5003";

        private const string Debug = nameof(Debug);
        private const string Info = nameof(Info);
        private const string Warn = nameof(Warn);
        private const string Error = nameof(Error);
        private const string Fatal = nameof(Fatal);

        public MiKo_5003_DebugLogExceptionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);

        private static bool IsException(ArgumentSyntax argument, SemanticModel semanticModel)
        {
            var expression = argument.Expression is InvocationExpressionSyntax i && i.Expression is MemberAccessExpressionSyntax m
                                 ? m.Expression
                                 : argument.Expression;

            var argumentType = expression.GetTypeSymbol(semanticModel);

            return argumentType.IsException();
        }

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
                                                                                                                  ? Analyze(methodCall, node.ArgumentList.Arguments[0], semanticModel)
                                                                                                                  : null;

        private Diagnostic Analyze(MemberAccessExpressionSyntax methodCall, ArgumentSyntax argument, SemanticModel semanticModel)
        {
            var methodName = methodCall.GetName();
            switch (methodName)
            {
                case Debug:
                case Info:
                case Warn:
                case Error:
                case Fatal:
                {
                    // check for correct type (only ILog methods shall be reported)
                    var type = methodCall.Expression.GetTypeSymbol(semanticModel);

                    if (type.Name == Constants.ILog && IsException(argument, semanticModel))
                    {
                        var enclosingMethod = methodCall.GetEnclosingMethod(semanticModel);
                        return Issue(enclosingMethod.Name, methodCall.Name, methodName);
                    }

                    break;
                }
            }

            return null;
        }
    }
}