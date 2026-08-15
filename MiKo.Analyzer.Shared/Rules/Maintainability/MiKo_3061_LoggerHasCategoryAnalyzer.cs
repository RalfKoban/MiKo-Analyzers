using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3061_LoggerHasCategoryAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3061";

        public MiKo_3061_LoggerHasCategoryAnalyzer() : base(Id)
        {
        }

        protected override bool IsApplicable(Compilation compilation) => compilation.GetTypeByMetadataName(Constants.ILog.FullTypeName) != null;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);

        private static bool IsLogManagerGetLoggerCall(MemberAccessExpressionSyntax node) => node.Is("GetLogger")
                                                                                         && node.Expression is IdentifierNameSyntax i
                                                                                         && i.GetName().EndsWith("LogManager", StringComparison.Ordinal);

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;

            var arguments = node.ArgumentList.Arguments;

            if (arguments.Count is 1 && node.Expression is MemberAccessExpressionSyntax maes && IsLogManagerGetLoggerCall(maes))
            {
                var argument = arguments[0];

                if (argument.IsString(context.SemanticModel) is false)
                {
                    ReportDiagnostics(context, Issue(context.ContainingSymbol?.Name, argument));
                }
            }
        }
    }
}