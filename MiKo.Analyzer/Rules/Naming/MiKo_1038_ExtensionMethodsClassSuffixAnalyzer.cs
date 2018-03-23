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

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => symbol.TypeKind == TypeKind.Class
                                                                                        && symbol.IsStatic
                                                                                        && symbol.GetMembers().OfType<IMethodSymbol>().Any(_ => _.IsExtensionMethod)
                                                                                        && !symbol.Name.EndsWith(Suffix, StringComparison.Ordinal)
                                                                                               ? new[] { ReportIssue(symbol, Suffix) }
                                                                                               : Enumerable.Empty<Diagnostic>();
    }
}