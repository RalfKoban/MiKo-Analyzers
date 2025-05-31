﻿using System.Collections;
using System.Collections.Generic;
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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeMethodDeclarationSyntax, SyntaxKind.MethodDeclaration);

        private static bool ReturnsEnumerable(MethodDeclarationSyntax method)
        {
            foreach (var child in method.ChildNodes())
            {
                switch (child)
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
            var methodName = method.Name;

            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    case BaseMethodDeclarationSyntax _:
                    case BaseTypeDeclarationSyntax _:
                    {
                        return false;
                    }

                    case ForEachStatementSyntax loop:
                    {
                        foreach (var invocation in loop.DescendantNodes<InvocationExpressionSyntax>())
                        {
                            var calledMethod = DetectCalledMethod(semanticModel, invocation, methodName);

                            if (calledMethod is null)
                            {
                                continue;
                            }

                            if (method.IsExtensionMethod && calledMethod.IsExtensionMethod)
                            {
                                if (method.ContainingType.Equals(calledMethod.ContainingType, SymbolEqualityComparer.IncludeNullability))
                                {
                                    return true;
                                }
                            }
                            else
                            {
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

        private static IMethodSymbol DetectCalledMethod(SemanticModel semanticModel, InvocationExpressionSyntax invocation, string methodName)
        {
            switch (invocation.Expression)
            {
                case IdentifierNameSyntax i when i.GetName() == methodName:
                    return DetectCalledMethod(semanticModel, invocation, i);

                case MemberAccessExpressionSyntax m when m.Name is IdentifierNameSyntax ins && ins.GetName() == methodName:
                    return DetectCalledMethod(semanticModel, invocation, ins);

                default:
                    return null;
            }
        }

        private static IMethodSymbol DetectCalledMethod(SemanticModel semanticModel, InvocationExpressionSyntax invocation, IdentifierNameSyntax identifier)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(identifier);

            if (symbolInfo.CandidateReason is CandidateReason.OverloadResolutionFailure)
            {
                var arguments = invocation.ArgumentList.Arguments;
                var argumentsCount = arguments.Count;

                // we might have multiple symbols, so we have to choose the right one (compare parameter types)
                foreach (var candidate in symbolInfo.CandidateSymbols.OfType<IMethodSymbol>()
                                                    .Where(_ => _.Parameters.Length == argumentsCount)
                                                    .Where(_ => ParametersHaveSameTypes(_.Parameters, arguments, semanticModel)))
                {
                    return candidate;
                }
            }

            return symbolInfo.Symbol as IMethodSymbol;
        }

        private static bool ParametersHaveSameTypes(in ImmutableArray<IParameterSymbol> candidateParameters, in SeparatedSyntaxList<ArgumentSyntax> arguments, SemanticModel semanticModel)
        {
            var candidateParametersLength = candidateParameters.Length;

            for (var index = 0; index < candidateParametersLength; index++)
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
            var issues = AnalyzeMethodDeclarationSyntax(context, method);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> AnalyzeMethodDeclarationSyntax(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
        {
            if (ReturnsEnumerable(method))
            {
                var semanticModel = context.SemanticModel;
                var methodSymbol = method.GetEnclosingMethod(semanticModel);

                // https://stackoverflow.com/questions/3969963/when-not-to-use-yield-return
                foreach (var yieldStatement in method.DescendantNodes<YieldStatementSyntax>())
                {
                    if (IsRecursiveYield(yieldStatement, semanticModel, methodSymbol))
                    {
                        yield return Issue(method.GetName(), yieldStatement);
                    }
                }
            }
        }
    }
}