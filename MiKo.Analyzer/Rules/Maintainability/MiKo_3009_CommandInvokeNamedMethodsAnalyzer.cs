using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3009_CommandInvokeNamedMethodsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3009";

        public MiKo_3009_CommandInvokeNamedMethodsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);

        private void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var node = (ObjectCreationExpressionSyntax)context.Node;
            if (!node.Type.IsCommand(context.SemanticModel)) return;

            var diagnostics = AnalyzeCommandCreation(node);
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeCommandCreation(ObjectCreationExpressionSyntax node)
        {
            var argumentList = node.ArgumentList;
            if (argumentList is null) return Enumerable.Empty<Diagnostic>();

            var arguments = argumentList.Arguments;
            if (arguments.Count == 0) return Enumerable.Empty<Diagnostic>();

            return arguments
                           .Where(_ => _.Expression is LambdaExpressionSyntax)
                           .Select(_ => ReportIssue(_.ToString(), _.GetLocation()))
                           .ToList();
        }
    }
}