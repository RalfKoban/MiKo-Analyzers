﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4101_TestSetUpMethodOrderingAnalyzer : TestMethodsOrderingAnalyzer
    {
        public const string Id = "MiKo_4101";

        public MiKo_4101_TestSetUpMethodOrderingAnalyzer() : base(Id)
        {
        }

        protected override IMethodSymbol GetMethod(INamedTypeSymbol symbol) => symbol.GetMembers()
                                                                                     .OfType<IMethodSymbol>()
                                                                                     .Where(_ => _.MethodKind == MethodKind.Ordinary)
                                                                                     .FirstOrDefault(_ => _.IsTestSetUpMethod());

        protected override int GetExpectedMethodIndex(IEnumerable<IMethodSymbol> methods)
        {
            var index = 0;

            if (methods.Any(_ => _.IsTestOneTimeSetUpMethod()))
            {
                index++;
            }

            if (methods.Any(_ => _.IsTestOneTimeTearDownMethod()))
            {
                index++;
            }

            return index;
        }
    }
}