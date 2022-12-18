using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1097_ParameterNameFollowsFieldNameSchemeAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1097";

        private const string FoundPrefix = "FoundPrefix";

        private static readonly string[] WrongPrefixes = Constants.Markers.FieldPrefixes.Where(_ => _.Length > 0).ToArray();
        private static readonly char[] WrongPrefixChars = Constants.Markers.FieldPrefixes.Where(_ => _.Length > 1).Select(_ => _[0]).ToArray(); // we are not interested in '_' character

        public MiKo_1097_ParameterNameFollowsFieldNameSchemeAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        internal static string FindBetterName(IParameterSymbol symbol, Diagnostic issue)
        {
            var prefix = issue.Properties[FoundPrefix];

            var betterName = symbol.Name.Substring(prefix.Length).ToLowerCaseAt(0);

            return betterName;
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        private static string FindWrongPrefix(ISymbol symbol)
        {
            var symbolName = symbol.Name.AsSpan();

            const int MinimalPrefixLength = 2;

            if (symbolName.Length >= MinimalPrefixLength)
            {
                foreach (var wrongPrefix in WrongPrefixes)
                {
                    if (symbolName.StartsWith(wrongPrefix))
                    {
                        return wrongPrefix;
                    }
                }

                var prefix = symbolName.Slice(0, MinimalPrefixLength);

                if (prefix[1].IsUpperCase())
                {
                    foreach (var wrongPrefixChar in WrongPrefixChars)
                    {
                        if (prefix[0] == wrongPrefixChar)
                        {
                            return wrongPrefixChar.ToString();
                        }
                    }
                }
            }

            return null;
        }

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol)
        {
            var foundPrefix = FindWrongPrefix(symbol);

            if (foundPrefix != null)
            {
                return new[]
                           {
                               Issue(symbol, foundPrefix, new Dictionary<string, string> { { FoundPrefix, foundPrefix } })
                           };
            }

            return Enumerable.Empty<Diagnostic>();

        }
    }
}