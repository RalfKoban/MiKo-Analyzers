using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1063_AbbreviationsInNameAnalyzer : OverallNameAnalyzer
    {
        public const string Id = "MiKo_1063";

        public MiKo_1063_AbbreviationsInNameAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            switch (symbol.Name)
            {
                case "paramName": // used in exceptions
                case "lParam": // used by Windows C++ API
                case "wParam": // used by Windows C++ API
                    return Array.Empty<Diagnostic>();

                default:
                    return base.AnalyzeName(symbol, compilation);
            }
        }

        protected override Diagnostic[] AnalyzeName(string symbolName, ISymbol symbol, string prefix = "")
        {
            var findings = AbbreviationFinder.Find(symbolName);
            var findingsLength = findings.Length;

            if (findingsLength is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var betterName = AbbreviationFinder.ReplaceAllAbbreviations(symbolName, findings);
            var betterNameProposal = CreateBetterNameProposal(prefix.Length > 0 ? prefix + betterName : betterName);

            var issues = new Diagnostic[findingsLength];

            for (var index = 0; index < findingsLength; index++)
            {
                var pair = findings[index];

                issues[index] = Issue(symbol, pair.Key, pair.Value, betterNameProposal);
            }

            return issues;
        }
    }
}