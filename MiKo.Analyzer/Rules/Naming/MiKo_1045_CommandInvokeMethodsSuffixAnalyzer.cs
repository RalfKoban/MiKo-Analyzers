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

        internal const string Suffix = "Command";

        public MiKo_1045_CommandInvokeMethodsSuffixAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        internal static string FindBetterName(ISymbol symbol) => symbol.Name.WithoutSuffix(Suffix);

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);

        private void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var node = (ObjectCreationExpressionSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            if (node.Type.IsCommand(semanticModel))
            {
                var issues = AnalyzeCommandCreation(node, semanticModel);

                ReportDiagnostics(context, issues);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeCommandCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var argumentList = node.ArgumentList;
            if (argumentList is null)
            {
                yield break;
            }

            var arguments = argumentList.Arguments;
            if (arguments.Count == 0)
            {
                yield break;
            }

            foreach (var argument in arguments)
            {
                yield return AnalyzeSuffix(argument, semanticModel);
            }
        }

        private Diagnostic AnalyzeSuffix(ArgumentSyntax argument, SemanticModel semanticModel)
        {
            var argumentName = argument.ToString();
            if (argumentName.EndsWith(Suffix, StringComparison.Ordinal))
            {
                var location = argument.GetLocation();
                var symbol = semanticModel.LookupSymbols(location.SourceSpan.Start, name: argumentName).FirstOrDefault();
                if (symbol != null)
                {
                    return Issue(symbol, argumentName.WithoutSuffix(Suffix));
                }
            }

            return null;
        }
    }
}