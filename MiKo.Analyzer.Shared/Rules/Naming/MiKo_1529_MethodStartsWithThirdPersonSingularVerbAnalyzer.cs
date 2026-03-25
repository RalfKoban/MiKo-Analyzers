using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1529_MethodStartsWithThirdPersonSingularVerbAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1529";

        private static readonly string[] AllowedFirstWords =
                                                             {
                                                                 "As",
                                                                 "Bytes",
                                                                 "Columns",
                                                                 "Devices",
                                                                 "Messages",
                                                                 "Modules",
                                                                 "Numbers",
                                                                 "Packages",
                                                                 "Parameters",
                                                                 "Projects",
                                                                 "Properties",
                                                                 "Windows",
                                                             };

        public MiKo_1529_MethodStartsWithThirdPersonSingularVerbAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (base.ShallAnalyze(symbol) is false)
            {
                return false;
            }

            if (symbol.ReturnType.IsBoolean())
            {
                return false;
            }

            if (symbol.IsTestMethod())
            {
                return false;
            }

            if (symbol.ContainingType.IsTestClass())
            {
                return false;
            }

            return true;
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;
            var firstWord = methodName.AsSpan().FirstWord();

            if (HasIssue(firstWord))
            {
                // only convert to string if needed, to avoid unnecessary allocations
                var word = firstWord.ToString();
                var infiniteVerb = Verbalizer.MakeInfiniteVerb(word);
                var betterName = infiniteVerb + methodName.Substring(firstWord.Length);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static bool HasIssue(in ReadOnlySpan<char> firstWord)
        {
            if (firstWord.EqualsAny(AllowedFirstWords))
            {
                return false;
            }

            return Verbalizer.IsThirdPersonSingularVerb(firstWord);
        }
    }
}