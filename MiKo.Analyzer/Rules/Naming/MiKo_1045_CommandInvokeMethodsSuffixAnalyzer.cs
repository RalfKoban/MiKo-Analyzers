using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1045_CommandInvokeMethodsSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1045";

        private const string Suffix = "Command";

        public MiKo_1045_CommandInvokeMethodsSuffixAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);

        private void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var node = (ObjectCreationExpressionSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            if (node.Type.IsCommand(semanticModel))
            {
                var diagnostics = AnalyzeCommandCreation(node, semanticModel);
                foreach (var diagnostic in diagnostics)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeCommandCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var argumentList = node.ArgumentList;
            if (argumentList is null)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var arguments = argumentList.Arguments;
            if (arguments.Count == 0)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var results = new List<Diagnostic>();
            foreach (var argument in arguments)
            {
                AnalyzeSuffix(argument, semanticModel, results);
            }

            return results;
        }

        private void AnalyzeSuffix(ArgumentSyntax argument, SemanticModel semanticModel, ICollection<Diagnostic> list)
        {
            var argumentName = argument.ToString();
            if (argumentName.EndsWith(Suffix, StringComparison.Ordinal))
            {
                var location = argument.GetLocation();
                var symbol = semanticModel.LookupSymbols(location.SourceSpan.Start, name: argumentName).FirstOrDefault();
                if (symbol != null)
                {
                    list.Add(Issue(symbol, argumentName.WithoutSuffix(Suffix)));
                }
            }
        }
    }
}