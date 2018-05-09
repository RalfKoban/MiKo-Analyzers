using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1038_ExtensionMethodsClassSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1038";

        private const string Suffix = "Extensions";

        public MiKo_1038_ExtensionMethodsClassSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Class && symbol.IsStatic && symbol.GetMembers().OfType<IMethodSymbol>().Any(_ => _.IsExtensionMethod);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol) => symbol.Name.EndsWith(Suffix, StringComparison.Ordinal)
                                                                                               ? Enumerable.Empty<Diagnostic>()
                                                                                               : new[] { ReportIssue(symbol, Suffix) };
    }
}