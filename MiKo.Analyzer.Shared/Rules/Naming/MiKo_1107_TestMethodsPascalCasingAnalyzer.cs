﻿using System;
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

        internal static string FindBetterName(ISymbol symbol) => FindBetterName(symbol.Name);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => false;

        protected override IEnumerable<Diagnostic> AnalyzeLocalFunctions(IMethodSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>(); // don't consider local functions at all

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (Regex.IsMatch(symbolName, PascalCasingRegex))
            {
                if (symbolName.Contains("_"))
                {
                    var underlinesNr = symbolName.Count(_ => _ is '_');
                    var upperCasesNr = symbolName.Count(_ => _.IsUpperCase());
                    var diff = underlinesNr - upperCasesNr;
                    if (diff >= 0)
                    {
                        yield break;
                    }
                }

                yield return Issue(symbol);
            }
        }

        private static string FindBetterName(string symbolName)
        {
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

                if (c == '_')
                {
                    // keep the existing underline
                    continue;
                }

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

                        if (characters[index - 1] == '_')
                        {
                            characters[index] = nextC;
                        }
                        else
                        {
                            // only add an underline if we not already have one
                            characters[index] = '_';
                            index++;
                            characters.Insert(index, nextC);
                        }
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

            // fix some corrections, such as for known exceptions
            var result = new string(characters.ToArray())
                         .Replace("argument_null_exception", nameof(ArgumentNullException))
                         .Replace("argument_exception", nameof(ArgumentException))
                         .Replace("argument_out_of_range_exception", nameof(ArgumentOutOfRangeException))
                         .Replace("invalid_operation_exception", nameof(InvalidOperationException))
                         .Replace("object_disposed_exception", nameof(ObjectDisposedException));

            return string.Intern(result);
        }
    }
}