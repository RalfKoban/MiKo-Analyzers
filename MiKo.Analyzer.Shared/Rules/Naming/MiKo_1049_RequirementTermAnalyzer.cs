using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        internal static string FindBetterName(ISymbol symbol) => FindBetterName(symbol.Name);

        internal static string FindBetterName(string symbolName)
        {
            var result = new StringBuilder(symbolName);

            foreach (var term in Constants.Markers.Requirements)
            {
                var lowerTerm = term.ToLowerCase();

                result
                    .Replace(term + "Be", "Is")
                    .Replace(term + "_Be", "Is")
                    .Replace(term + "Call", "Calls")
                    .Replace(term + "_Call", "Calls")
                    .Replace(term + "Create", "Creates")
                    .Replace(term + "_Create", "Creates")
                    .Replace(term + "Fail", "Fails")
                    .Replace(term + "Have", "Have")
                    .Replace(term + "NotHave", "DoNotHave")
                    .Replace(term + "NtHave", "DoNotHave")
                    .Replace(term + "ntHave", "DoNotHave")
                    .Replace(term + "NotBe", "IsNot")
                    .Replace(term + "NtBe", "IsNot")
                    .Replace(term + "ntBe", "IsNot")
                    .Replace(term + "Returns", "Returns")
                    .Replace(term + "Return", "Returns")
                    .Replace(term + "_Returns", "Returns")
                    .Replace(term + "_Return", "Returns")
                    .Replace(term + "Throw", "Throws")
                    .Replace(term + "_Throw", "Throws")
                    .Replace(term, "Does")
                    .Replace("_" + lowerTerm + "_be_", "_is_")
                    .Replace("_" + lowerTerm + "_call", "_calls")
                    .Replace("_" + lowerTerm + "_create", "_creates")
                    .Replace("_" + lowerTerm + "_fail", "_fails")
                    .Replace("_" + lowerTerm + "_have_", "_has_")
                    .Replace("_" + lowerTerm + "_not_have_", "_does_not_have_")
                    .Replace("_" + lowerTerm + "_not_be_", "_is_not_")
                    .Replace("_" + lowerTerm + "_return_", "_returns_")
                    .Replace("_" + lowerTerm + "_returns_", "_returns_")
                    .Replace("_" + lowerTerm + "_throw_", "_throws_")
                    .Replace("_" + lowerTerm + "_", "_does_");
            }

            return result.ToString();
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
            var symbolName = new StringBuilder(symbol.Name).Replace("efresh", "#") // filter 'refresh' and 'Refresh'
                                                           .Replace("hallow", "#") // filter 'shallow' and 'Shallow'
                                                           .Replace("icenseNeed", "#") // filter 'licenseNeed' and 'LicenseNeed'
                                                           .Replace("eeded", "#") // filter 'needed' and 'Needed'
                                                           .ToString();

            return Constants.Markers.Requirements
                            .Where(_ => symbolName.Contains(_, StringComparison.OrdinalIgnoreCase))
                            .Select(_ => Issue(symbol, _));
        }
    }
}