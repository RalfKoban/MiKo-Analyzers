﻿using System.Collections;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5012_RecursiveCallUsesYieldAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5012";

        public MiKo_5012_RecursiveCallUsesYieldAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeMethodDeclarationSyntax, SyntaxKind.MethodDeclaration);

        private static bool ReturnsEnumerable(MethodDeclarationSyntax method)
        {
            foreach (var unknown in method.ChildNodes())
            {
                switch (unknown)
                {
                    case GenericNameSyntax g when g.GetName() == nameof(IEnumerable):
                    case IdentifierNameSyntax i when i.GetName() == nameof(IEnumerable):
                        return true;
                }
            }

            return false;
        }

        private static bool IsRecursiveYield(YieldStatementSyntax node, SemanticModel semanticModel, IMethodSymbol method)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    case MethodDeclarationSyntax _:
                    {
                        return false;
                    }

                    case ForEachStatementSyntax loop:
                    {
                        foreach (var invocation in loop.DescendantNodes().OfType<InvocationExpressionSyntax>())
                        {
                            if (invocation.Expression is IdentifierNameSyntax i && i.GetName() == method.Name)
                            {
                                var calledMethod = DetectCalledMethod(semanticModel, i, invocation);

                                if (method.Equals(calledMethod, SymbolEqualityComparer.IncludeNullability))
                                {
                                    return true;
                                }
                            }
                        }

                        return false;
                    }
                }
            }

            return false;
        }

        private static ISymbol DetectCalledMethod(SemanticModel semanticModel, IdentifierNameSyntax identifier, InvocationExpressionSyntax invocation)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(identifier);

            if (symbolInfo.CandidateReason == CandidateReason.OverloadResolutionFailure)
            {
                var arguments = invocation.ArgumentList.Arguments;

                // we might have multiple symbols, so we have to choose the right one (compare parameter types)
                foreach (var candidate in symbolInfo.CandidateSymbols.OfType<IMethodSymbol>()
                                                    .Where(_ => _.Parameters.Length == arguments.Count)
                                                    .Where(_ => ParametersHaveSameTypes(_.Parameters, arguments, semanticModel)))
                {
                    return candidate;
                }
            }

            return symbolInfo.Symbol;
        }

        private static bool ParametersHaveSameTypes(ImmutableArray<IParameterSymbol> candidateParameters, SeparatedSyntaxList<ArgumentSyntax> arguments, SemanticModel semanticModel)
        {
            for (var index = 0; index < candidateParameters.Length; index++)
            {
                var c = candidateParameters[index];

                var argument = arguments[index];
                var argumentType = argument.GetTypeSymbol(semanticModel);

                if (c.Type.Equals(argumentType, SymbolEqualityComparer.Default) is false)
                {
                    return false;
                }
            }

            return true;
        }

        private void AnalyzeMethodDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;

            if (ReturnsEnumerable(method))
            {
                var semanticModel = context.SemanticModel;
                var methodSymbol = method.GetEnclosingMethod(semanticModel);

                // https://stackoverflow.com/questions/3969963/when-not-to-use-yield-return
                foreach (var yieldStatement in method.DescendantNodes().OfType<YieldStatementSyntax>())
                {
                    if (IsRecursiveYield(yieldStatement, semanticModel, methodSymbol))
                    {
                        var issue = Issue(method.GetName(), yieldStatement);
                        context.ReportDiagnostic(issue);
                    }
                }
            }
        }
    }
}