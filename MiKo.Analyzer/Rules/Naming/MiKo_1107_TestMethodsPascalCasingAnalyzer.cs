using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1107_TestMethodsPascalCasingAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1107";

        private const string PascalCasingRegex = "[a-z]+[A-Z]+";

        public MiKo_1107_TestMethodsPascalCasingAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        internal static string FindBetterName(ISymbol symbol)
        {
            var symbolName = symbol.Name;
            if (symbolName.Length < 2)
            {
                return symbolName;
            }

            var caseAlreadyFlipped = false;

            const int CharacterToStartWith = 1;

            var characters = new List<char>(symbolName);
            for (var index = CharacterToStartWith; index < characters.Count; index++)
            {
                var c = characters[index];

                if (c.IsUpperCase())
                {
                    if (index == CharacterToStartWith)
                    {
                        // multiple upper cases in a line at beginning of the name, so do not flip
                        caseAlreadyFlipped = true;
                    }

                    if (caseAlreadyFlipped is false)
                    {
                        var nextC = c.ToLowerCase();

                        var nextIndex = index + 1;
                        if (nextIndex >= characters.Count || (nextIndex < characters.Count && characters[nextIndex].IsUpperCase()))
                        {
                            // multiple upper cases in a line, so do not flip
                            nextC = c;
                        }

                        characters[index++] = '_';
                        characters.Insert(index, nextC);
                    }

                    caseAlreadyFlipped = true;
                }
                else
                {
                    if (caseAlreadyFlipped && characters[index - 1].IsUpperCase())
                    {
                        // we are behind multiple upper cases in a line, so add an underline
                        characters[index++] = '_';
                        characters.Insert(index, c);
                    }

                    caseAlreadyFlipped = false;
                }
            }

            return string.Intern(new string(characters.ToArray()));
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol)
        {
            var symbolName = symbol.Name;

            if (Regex.IsMatch(symbolName, PascalCasingRegex) is false)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            if (symbolName.Contains("_"))
            {
                var underlinesNr = symbolName.Count(_ => _ is '_');
                var upperCasesNr = symbolName.Count(_ => _.IsUpperCase());
                var diff = underlinesNr - upperCasesNr;
                if (diff >= 0)
                {
                    return Enumerable.Empty<Diagnostic>();
                }
            }

            return new[] { Issue(symbol) };
        }
    }
}