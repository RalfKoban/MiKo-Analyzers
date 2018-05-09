using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1015_FactoryMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1015";

        public MiKo_1015_FactoryMethodsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsFactory();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol) => symbol.GetMembers().OfType<IMethodSymbol>().SelectMany(AnalyzeMethod).ToList();

        protected override IEnumerable<Diagnostic> AnalyzeOrdinaryMethod(IMethodSymbol method) => method.Name.StartsWith("Create", StringComparison.Ordinal)
                                                                                                      ? Enumerable.Empty<Diagnostic>()
                                                                                                      : new[] { ReportIssue(method) };
    }
}