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

        private static readonly string[] WrongPrefixes = Constants.Markers.FieldPrefixes.Where(_ => _.Length > 0).ToArray();
        private static readonly char[] WrongPrefixChars = Constants.Markers.FieldPrefixes.Where(_ => _.Length > 1).Select(_ => _[0]).ToArray(); // we are not interested in '_' character

        public MiKo_1097_ParameterNameFollowsFieldNameSchemeAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        private static string FindWrongPrefix(ReadOnlySpan<char> symbolName)
        {
            const int MinimalPrefixLength = 2;

            if (symbolName.Length >= MinimalPrefixLength)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var index = 0; index < WrongPrefixes.Length; index++)
                {
                    var wrongPrefix = WrongPrefixes[index];

                    if (symbolName.StartsWith(wrongPrefix))
                    {
                        return wrongPrefix;
                    }
                }

                var prefix = symbolName.Slice(0, MinimalPrefixLength);

                if (prefix[1].IsUpperCase())
                {
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var index = 0; index < WrongPrefixChars.Length; index++)
                    {
                        var wrongPrefixChar = WrongPrefixChars[index];

                        if (prefix[0] == wrongPrefixChar)
                        {
                            return wrongPrefixChar.ToString();
                        }
                    }
                }
            }

            return null;
        }

        private static string FindBetterName(ReadOnlySpan<char> symbolName, string prefix) => symbolName.Slice(prefix.Length).ToLowerCaseAt(0);

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol)
        {
            var symbolName = symbol.Name.AsSpan();

            var foundPrefix = FindWrongPrefix(symbolName);

            if (foundPrefix != null)
            {
                var proposal = FindBetterName(symbolName, foundPrefix);

                return new[] { Issue(symbol, foundPrefix, CreateBetterNameProposal(proposal)) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}