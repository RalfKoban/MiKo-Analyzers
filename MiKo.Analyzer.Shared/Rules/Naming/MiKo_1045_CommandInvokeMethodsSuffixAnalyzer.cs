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

        internal const string Suffix = Constants.Names.Command;

        private const string IsPrefix = "Is";
        private const string CanPrefix = "Can";
        private const string HasPrefix = "Has";

        private static readonly string[] HasArePrefix = { HasPrefix, "Are" };

        public MiKo_1045_CommandInvokeMethodsSuffixAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);

        private static string FindBetterName(ISymbol symbol)
        {
            // remove any starting 'Is' / 'Has' / 'Are'
            var betterName = WithoutIsHasArePrefix(symbol.Name.AsSpan().WithoutSuffix(Suffix));

            if (symbol is IMethodSymbol method)
            {
                if (method.ReturnType.IsBoolean())
                {
                    if (StartsWithCan(betterName))
                    {
                        // already starts correctly
                        return betterName.ToString();
                    }

                    return CanPrefix.ConcatenatedWith(betterName);
                }

                // remove 'Can' at the beginning as the name already fits
                if (StartsWithCan(betterName))
                {
                    return betterName.Slice(CanPrefix.Length).ToString();
                }
            }

            return betterName.ToString();

            bool StartsWithCan(in ReadOnlySpan<char> name) => name.Length > CanPrefix.Length + 1 && name[CanPrefix.Length].IsUpperCaseLetter() && name.StartsWith(CanPrefix);

            ReadOnlySpan<char> WithoutIsHasArePrefix(in ReadOnlySpan<char> name)
            {
                var nameLength = name.Length;

                switch (nameLength)
                {
                    case 0:
                    case 1:
                    case 2:
                        return name;

                    default:
                    {
                        if (name[IsPrefix.Length].IsUpperCaseLetter() && name.Slice(0, IsPrefix.Length).Equals(IsPrefix, StringComparison.Ordinal))
                        {
                            return name.Slice(IsPrefix.Length);
                        }

                        if (nameLength > HasPrefix.Length && name[HasPrefix.Length].IsUpperCaseLetter() && name.Slice(0, HasPrefix.Length).EqualsAny(HasArePrefix))
                        {
                            return name.Slice(HasPrefix.Length);
                        }

                        return name;
                    }
                }
            }
        }

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

            if (arguments.Count is 0)
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
                    var betterName = FindBetterName(symbol);

                    return Issue(symbol, betterName, CreateBetterNameProposal(betterName));
                }
            }

            return null;
        }
    }
}