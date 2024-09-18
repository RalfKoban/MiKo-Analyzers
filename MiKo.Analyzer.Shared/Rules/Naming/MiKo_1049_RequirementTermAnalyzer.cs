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

        private static readonly Pair[] ReplacementMap = CreateReplacementMapEntries().ToArray();

        public MiKo_1049_RequirementTermAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.IsConst is false && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => true;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        private static IEnumerable<Pair> CreateReplacementMapEntries()
        {
            foreach (var term in Constants.Markers.Requirements)
            {
                var lowerTerm = term.ToLowerCase();

                yield return new Pair(term + "Be", "Is");
                yield return new Pair(term + "_Be", "Is");
                yield return new Pair(term + "Call", "Calls");
                yield return new Pair(term + "_Call", "Calls");
                yield return new Pair(term + "Create", "Creates");
                yield return new Pair(term + "_Create", "Creates");
                yield return new Pair(term + "Fail", "Fails");
                yield return new Pair(term + "Have", "Have");
                yield return new Pair(term + "NotHave", "DoNotHave");
                yield return new Pair(term + "NtHave", "DoNotHave");
                yield return new Pair(term + "ntHave", "DoNotHave");
                yield return new Pair(term + "NotBe", "IsNot");
                yield return new Pair(term + "NtBe", "IsNot");
                yield return new Pair(term + "ntBe", "IsNot");
                yield return new Pair(term + "Returns", "Returns");
                yield return new Pair(term + "Return", "Returns");
                yield return new Pair(term + "_Returns", "Returns");
                yield return new Pair(term + "_Return", "Returns");
                yield return new Pair(term + "Throw", "Throws");
                yield return new Pair(term + "_Throw", "Throws");
                yield return new Pair(term, "Does");

                yield return new Pair("_" + lowerTerm + "_be_", "_is_");
                yield return new Pair("_" + lowerTerm + "_call", "_calls");
                yield return new Pair("_" + lowerTerm + "_create", "_creates");
                yield return new Pair("_" + lowerTerm + "_fail", "_fails");
                yield return new Pair("_" + lowerTerm + "_have_", "_has_");
                yield return new Pair("_" + lowerTerm + "_not_have_", "_does_not_have_");
                yield return new Pair("_" + lowerTerm + "_not_be_", "_is_not_");
                yield return new Pair("_" + lowerTerm + "_return_", "_returns_");
                yield return new Pair("_" + lowerTerm + "_returns_", "_returns_");
                yield return new Pair("_" + lowerTerm + "_throw_", "_throws_");
                yield return new Pair("_" + lowerTerm + "_", "_does_");
            }
        }

        private static string FindBetterName(string symbolName)
        {
            var result = new StringBuilder(symbolName);

            foreach (var pair in ReplacementMap)
            {
                result.ReplaceWithCheck(pair.Key, pair.Value);
            }

            return result.ToString();
        }

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol)
        {
            var symbolName = new StringBuilder(symbol.Name).ReplaceWithCheck("efresh", "#") // filter 'refresh' and 'Refresh'
                                                           .ReplaceWithCheck("hallow", "#") // filter 'shallow' and 'Shallow'
                                                           .ReplaceWithCheck("icenseNeed", "#") // filter 'licenseNeed' and 'LicenseNeed'
                                                           .ReplaceWithCheck("eeded", "#") // filter 'needed' and 'Needed'
                                                           .ReplaceWithCheck("eeds", "#") // filter 'needs' and 'Needs'
                                                           .ToString();

            return Constants.Markers.Requirements
                            .Where(_ => symbolName.Contains(_, StringComparison.OrdinalIgnoreCase))
                            .Select(_ => Issue(symbol, _, CreateBetterNameProposal(FindBetterName(symbol.Name))));
        }
    }
}