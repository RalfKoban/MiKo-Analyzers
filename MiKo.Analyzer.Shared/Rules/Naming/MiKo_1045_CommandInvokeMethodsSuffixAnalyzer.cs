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

        private const string IsPrefix = "Is";
        private static readonly string[] HasArePrefix = { "Has", "Are" };

        public MiKo_1045_CommandInvokeMethodsSuffixAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        internal static string FindBetterName(ISymbol symbol)
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

                    return "Can" + betterName.ToString();
                }

                // remove 'Can' at the beginning as the name already fits
                if (StartsWithCan(betterName))
                {
                    return betterName.Slice(3).ToString();
                }
            }

            return betterName.ToString();

            bool StartsWithCan(ReadOnlySpan<char> name) => name.Length > 4 && name[3].IsUpperCaseLetter() && name.StartsWith("Can", StringComparison.Ordinal);

            ReadOnlySpan<char> WithoutIsHasArePrefix(ReadOnlySpan<char> name)
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

                        if (nameLength > 3 && name[3].IsUpperCaseLetter() && name.Slice(0, 3).EqualsAny(HasArePrefix))
                        {
                            return name.Slice(3);
                        }

                        return name;
                    }
                }
            }
        }

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
                    return Issue(symbol, FindBetterName(symbol));
                }
            }

            return null;
        }
    }
}