﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4003_DisposeMethodsOrderedAfterCtorsAndFinalizersAnalyzer : OrderingAnalyzer
    {
        public const string Id = "MiKo_4003";

        public MiKo_4003_DisposeMethodsOrderedAfterCtorsAndFinalizersAnalyzer() : base(Id)
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
            var ctors = GetMethodsOrderedByLocation(symbol, MethodKind.Constructor).Select(_ => _.GetStartingLine()).ToList();
            var finalizers = GetMethodsOrderedByLocation(symbol, MethodKind.Destructor).Select(_ => _.GetStartingLine()).ToList();

            var ordinaryMethods = GetMethodsOrderedByLocation(symbol);
            var interfaceImplementations = GetMethodsOrderedByLocation(symbol, MethodKind.ExplicitInterfaceImplementation);

            var ordinaryDisposeMethods = ordinaryMethods.Where(_ => _.DeclaredAccessibility == Accessibility.Public && _.Parameters.None() && _.Name == nameof(IDisposable.Dispose)).ToList();
            var interfaceDisposeMethods = interfaceImplementations.Where(_ => _.Parameters.None() && _.Name == nameof(System) + "." + nameof(IDisposable) + "." + nameof(IDisposable.Dispose)).ToList();

            var otherMethods = ordinaryMethods.Except(ordinaryDisposeMethods).Concat(interfaceImplementations.Except(interfaceDisposeMethods)).Select(_ => _.GetStartingLine()).ToList();

            var disposeMethods = ordinaryDisposeMethods.Concat(interfaceDisposeMethods);

            foreach (var disposeMethod in disposeMethods)
            {
                var startingLine = disposeMethod.GetStartingLine();

                if (ctors.Exists(_ => _ >= startingLine) || finalizers.Exists(_ => _ >= startingLine) || otherMethods.Exists(_ => _ <= startingLine))
                {
                    yield return Issue(disposeMethod);
                }
            }
        }
    }
}