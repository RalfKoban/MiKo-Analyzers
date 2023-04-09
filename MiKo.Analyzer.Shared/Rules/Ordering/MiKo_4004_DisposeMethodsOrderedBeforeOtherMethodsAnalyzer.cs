using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4004_DisposeMethodsOrderedBeforeOtherMethodsAnalyzer : OrderingAnalyzer
    {
        public const string Id = "MiKo_4004";

        private static readonly Accessibility[] Accessibilities =
            {
                Accessibility.Public,
                Accessibility.Internal,
                Accessibility.Protected,
                Accessibility.Private,
            };

        public MiKo_4004_DisposeMethodsOrderedBeforeOtherMethodsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (symbol.IsDisposable())
            {
                foreach (var diagnostic in AnalyzeTypeCore(symbol))
                {
                    yield return diagnostic;
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeTypeCore(INamedTypeSymbol symbol)
        {
            var ordinaryMethods = GetMethodsOrderedByLocation(symbol).ToList();

            foreach (var accessibility in Accessibilities)
            {
                var methods = ordinaryMethods.Where(_ => _.DeclaredAccessibility == accessibility).ToList();
                var disposeMethods = methods.Where(_ => _.Name == nameof(IDisposable.Dispose)).ToList();

                if (accessibility == Accessibility.Public && disposeMethods.Count == 0)
                {
                    var interfaceImplementations = GetMethodsOrderedByLocation(symbol, MethodKind.ExplicitInterfaceImplementation).ToList();
                    disposeMethods = interfaceImplementations.Where(_ => _.Parameters.None() && _.Name == nameof(System) + "." + nameof(IDisposable) + "." + nameof(IDisposable.Dispose)).ToList();
                }

                if (disposeMethods.Count == 0)
                {
                    continue;
                }

                var otherMethodStartingLines = methods.Except(disposeMethods).Where(_ => _.IsStatic is false).Select(_ => _.GetStartingLine()).ToList();

                foreach (var disposeMethod in disposeMethods)
                {
                    var startingLine = disposeMethod.GetStartingLine();

                    if (otherMethodStartingLines.Any(_ => _ <= startingLine))
                    {
                        yield return Issue(disposeMethod);
                    }
                }
            }
        }
    }
}