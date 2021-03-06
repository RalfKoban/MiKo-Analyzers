﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5010_EqualsAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5010";

        public MiKo_5010_EqualsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);

        private static bool IsObjectEqualsMethod(ISymbol method) => method != null && method.IsStatic && method.ContainingType.SpecialType == SpecialType.System_Object && method.Name == nameof(object.Equals);

        private static bool IsStruct(SemanticModel semanticModel, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            foreach (var argument in arguments)
            {
                if (argument.Expression.IsStruct(semanticModel))
                {
                    return true;
                }
            }

            return false;
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;

            var diagnostic = AnalyzeEqualsInvocation(node, context.SemanticModel);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
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
                    return Issue(node);
                }
            }

            return null;
        }
    }
}