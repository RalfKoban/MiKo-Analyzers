using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3004_EqualsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3004";

        public MiKo_3004_EqualsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;

            var diagnostic = AnalyzeEqualsInvocation(node, context.SemanticModel);
            if (diagnostic != null) context.ReportDiagnostic(diagnostic);
        }

        private Diagnostic AnalyzeEqualsInvocation(InvocationExpressionSyntax node, SemanticModel semanticModel)
        {
            var arguments = node.ArgumentList.Arguments;
            return arguments.Count == 2
                   ? AnalyzeMethod(node, semanticModel, arguments)
                   : null;
        }

        private Diagnostic AnalyzeMethod(ExpressionSyntax node, SemanticModel semanticModel, SeparatedSyntaxList<ArgumentSyntax> arguments) => IsEqualsMethod(semanticModel.GetSymbolInfo(node).Symbol)
                                                                                                                                            && IsStruct(semanticModel, arguments)
                                                                                                                                                    ? ReportIssue(node.ToString(), node.GetLocation())
                                                                                                                                                    : null;

        private static bool IsEqualsMethod(ISymbol method) => method != null && method.IsStatic && method.Name == nameof(object.Equals) && method.ContainingType.Name == nameof(System.Object);

        private static bool IsStruct(SemanticModel semanticModel, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            foreach (var argument in arguments)
            {
                var typeInfo = semanticModel.GetTypeInfo(argument.Expression);
                if (typeInfo.Type?.TypeKind == TypeKind.Struct) return true;
            }

            return false;
        }
    }
}