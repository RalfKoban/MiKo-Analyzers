using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1049_RequirementTermAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1049";

        public MiKo_1049_RequirementTermAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        internal static string FindBetterName(ISymbol symbol)
        {
            var symbolName = symbol.Name;

            foreach (var term in Constants.Markers.Requirements)
            {
                var lowerTerm = term.ToLowerCase();

                symbolName = symbolName.Replace(term + "Have", "Have")
                                       .Replace(term + "NotHave", "DoNotHave")
                                       .Replace(term + "NtHave", "DoNotHave")
                                       .Replace(term + "ntHave", "DoNotHave")
                                       .Replace(term + "Be", "Is")
                                       .Replace(term + "NotBe", "IsNot")
                                       .Replace(term + "NtBe", "IsNot")
                                       .Replace(term + "ntBe", "IsNot")
                                       .Replace("_" + lowerTerm + "_have_", "_has_")
                                       .Replace("_" + lowerTerm + "_not_have_", "_does_not_have_")
                                       .Replace("_" + lowerTerm + "_not_be_", "_is_not_")
                                       .Replace("_" + lowerTerm + "_be_", "_is_");
            }

            foreach (var term in Constants.Markers.Requirements)
            {
                symbolName = symbolName.Replace(term, "Does");
            }

            return symbolName;
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.IsConst is false && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol)
        {
            var symbolName = symbol.Name;

            return Constants.Markers.Requirements
                            .Where(_ => symbolName.Contains(_, StringComparison.OrdinalIgnoreCase))
                            .Select(_ => Issue(symbol, _));
        }
    }
}