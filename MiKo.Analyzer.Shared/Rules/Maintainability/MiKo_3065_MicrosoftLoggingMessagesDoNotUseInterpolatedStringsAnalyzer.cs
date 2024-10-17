using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3065_MicrosoftLoggingMessagesDoNotUseInterpolatedStringsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3065";

        public MiKo_3065_MicrosoftLoggingMessagesDoNotUseInterpolatedStringsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override bool IsApplicable(CompilationStartAnalysisContext context) => context.Compilation.GetTypeByMetadataName(Constants.MicrosoftLogging.FullTypeName) != null;

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInterpolatedString, SyntaxKind.InterpolatedStringExpression);
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static bool IsLoggingCall(SyntaxNode node, SemanticModel semanticModel)
        {
            if (node.Parent is ArgumentSyntax a && a.Parent is ArgumentListSyntax al && al.Parent is InvocationExpressionSyntax invocation && invocation.Expression is MemberAccessExpressionSyntax methodCall)
            {
                var methodName = methodCall.GetName();

                switch (methodName)
                {
                    case Constants.MicrosoftLogging.BeginScope:
                    case Constants.MicrosoftLogging.Log:
                    case Constants.MicrosoftLogging.LogCritical:
                    case Constants.MicrosoftLogging.LogDebug:
                    case Constants.MicrosoftLogging.LogError:
                    case Constants.MicrosoftLogging.LogInformation:
                    case Constants.MicrosoftLogging.LogTrace:
                    case Constants.MicrosoftLogging.LogWarning:
                    {
                        var type = methodCall.GetTypeSymbol(semanticModel);

                        if (type.Name == Constants.MicrosoftLogging.TypeName && type.ContainingNamespace.FullyQualifiedName() == Constants.MicrosoftLogging.NamespaceName)
                        {
                            var argumentSymbol = a.GetTypeSymbol(semanticModel);

                            return argumentSymbol.IsString();
                        }

                        return false;
                    }
                }
            }

            return false;
        }

        private void AnalyzeInterpolatedString(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InterpolatedStringExpressionSyntax node && IsLoggingCall(node, context.SemanticModel))
            {
                ReportDiagnostics(context, Issue(node.StringStartToken));
            }
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax node && IsLoggingCall(node, context.SemanticModel))
            {
                ReportDiagnostics(context, Issue(node));
            }
        }
    }
}