using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1051_DelegateParameterNameSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1051";

        private static readonly string[] WrongNames = { "Action", "Delegate", "Func" };

        public MiKo_1051_DelegateParameterNameSuffixAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        // symbol.TypeKind == TypeKind.Delegate
        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol) => symbol.Type.TypeKind == TypeKind.Delegate
                                                                                                    ? AnalyzeName(symbol)
                                                                                                    : Enumerable.Empty<Diagnostic>();

        private IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol) => symbol.Name.EndsWithAny(StringComparison.OrdinalIgnoreCase, WrongNames)
                                                                                    ? new[] { ReportIssue(symbol) }
                                                                                    : Enumerable.Empty<Diagnostic>();
    }
}