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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private static bool IsLogManagerGetLoggerCall(MemberAccessExpressionSyntax node) => node.GetName() is "GetLogger"
                                                                                         && node.Expression is IdentifierNameSyntax i
                                                                                         && i.GetName().EndsWith("LogManager", StringComparison.Ordinal);

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            if (node.Parent is InvocationExpressionSyntax s)
            {
                var arguments = s.ArgumentList.Arguments;

                if (arguments.Count is 1 && IsLogManagerGetLoggerCall(node))
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
}