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

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => symbol.IsFactory()
                                                                                               ? symbol.GetMembers().OfType<IMethodSymbol>().SelectMany(AnalyzeMethod).ToList()
                                                                                               : Enumerable.Empty<Diagnostic>();

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (method.MethodKind != MethodKind.Ordinary || method.IsOverride) return Enumerable.Empty<Diagnostic>();

            if (method.Name.StartsWith("Create", StringComparison.Ordinal)) return Enumerable.Empty<Diagnostic>();

            return new[] { ReportIssue(method) };
        }
    }
}