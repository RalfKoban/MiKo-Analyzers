
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3030_EqualsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3030";

        public MiKo_3030_EqualsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);

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

        private Diagnostic AnalyzeMethod(ExpressionSyntax node, SemanticModel semanticModel, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            var isEquals = IsObjectEqualsMethod(semanticModel.GetSymbolInfo(node).Symbol) && IsStruct(semanticModel, arguments);
            if (isEquals)
            {
                var method = node.GetEnclosingMethod(semanticModel);
                if (method.MethodKind != MethodKind.UserDefinedOperator)
                {
                    return Issue(node.ToString(), node.GetLocation());
                }
            }

            return null;
        }

        private static bool IsObjectEqualsMethod(ISymbol method) => method != null && method.IsStatic && method.ContainingType.SpecialType == SpecialType.System_Object && method.Name == nameof(object.Equals);

        private static bool IsStruct(SemanticModel semanticModel, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            foreach (var argument in arguments)
            {
                if (argument.Expression.IsStruct(semanticModel))
                    return true;
            }

            return false;
        }
    }
}