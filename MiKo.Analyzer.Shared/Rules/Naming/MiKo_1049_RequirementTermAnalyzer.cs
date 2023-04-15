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

        private static readonly KeyValuePair<string, string>[] ReplacementMap = CreateReplacementMapEntries().ToArray();

        public MiKo_1049_RequirementTermAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        internal static string FindBetterName(ISymbol symbol) => FindBetterName(symbol.Name);

        internal static string FindBetterName(string symbolName)
        {
            var result = new StringBuilder(symbolName);

            foreach (var pair in ReplacementMap)
            {
                result.Replace(pair.Key, pair.Value);
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

        private static IEnumerable<KeyValuePair<string, string>> CreateReplacementMapEntries()
        {
            foreach (var term in Constants.Markers.Requirements)
            {
                var lowerTerm = term.ToLowerCase();

                yield return new KeyValuePair<string, string>(term + "Be", "Is");
                yield return new KeyValuePair<string, string>(term + "_Be", "Is");
                yield return new KeyValuePair<string, string>(term + "Call", "Calls");
                yield return new KeyValuePair<string, string>(term + "_Call", "Calls");
                yield return new KeyValuePair<string, string>(term + "Create", "Creates");
                yield return new KeyValuePair<string, string>(term + "_Create", "Creates");
                yield return new KeyValuePair<string, string>(term + "Fail", "Fails");
                yield return new KeyValuePair<string, string>(term + "Have", "Have");
                yield return new KeyValuePair<string, string>(term + "NotHave", "DoNotHave");
                yield return new KeyValuePair<string, string>(term + "NtHave", "DoNotHave");
                yield return new KeyValuePair<string, string>(term + "ntHave", "DoNotHave");
                yield return new KeyValuePair<string, string>(term + "NotBe", "IsNot");
                yield return new KeyValuePair<string, string>(term + "NtBe", "IsNot");
                yield return new KeyValuePair<string, string>(term + "ntBe", "IsNot");
                yield return new KeyValuePair<string, string>(term + "Returns", "Returns");
                yield return new KeyValuePair<string, string>(term + "Return", "Returns");
                yield return new KeyValuePair<string, string>(term + "_Returns", "Returns");
                yield return new KeyValuePair<string, string>(term + "_Return", "Returns");
                yield return new KeyValuePair<string, string>(term + "Throw", "Throws");
                yield return new KeyValuePair<string, string>(term + "_Throw", "Throws");
                yield return new KeyValuePair<string, string>(term, "Does");

                yield return new KeyValuePair<string, string>("_" + lowerTerm + "_be_", "_is_");
                yield return new KeyValuePair<string, string>("_" + lowerTerm + "_call", "_calls");
                yield return new KeyValuePair<string, string>("_" + lowerTerm + "_create", "_creates");
                yield return new KeyValuePair<string, string>("_" + lowerTerm + "_fail", "_fails");
                yield return new KeyValuePair<string, string>("_" + lowerTerm + "_have_", "_has_");
                yield return new KeyValuePair<string, string>("_" + lowerTerm + "_not_have_", "_does_not_have_");
                yield return new KeyValuePair<string, string>("_" + lowerTerm + "_not_be_", "_is_not_");
                yield return new KeyValuePair<string, string>("_" + lowerTerm + "_return_", "_returns_");
                yield return new KeyValuePair<string, string>("_" + lowerTerm + "_returns_", "_returns_");
                yield return new KeyValuePair<string, string>("_" + lowerTerm + "_throw_", "_throws_");
                yield return new KeyValuePair<string, string>("_" + lowerTerm + "_", "_does_");
            }
        }

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