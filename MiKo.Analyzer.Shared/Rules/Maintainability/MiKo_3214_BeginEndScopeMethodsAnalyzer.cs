using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3214_BeginEndScopeMethodsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3214";

        private static readonly string[] ScopeIndicators = { "Begin", "End", "Enter", "Exit", "Leave" };

        public MiKo_3214_BeginEndScopeMethodsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.TypeKind == TypeKind.Interface;

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation)
        {
            foreach (var method in symbol.GetMethods())
            {
                var name = method.Name;
                var indicator = ScopeIndicators.Find(_ => name.StartsWith(_, StringComparison.OrdinalIgnoreCase));

                if (indicator != null)
                {
                    yield return Issue(method, indicator);
                }
            }
        }
    }
}