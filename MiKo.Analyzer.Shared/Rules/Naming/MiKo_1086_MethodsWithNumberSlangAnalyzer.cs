using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1086_MethodsWithNumberSlangAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1086";

        private static readonly char[] SlangNumbers = { '2', '4' };

        private static readonly string[] AllowedNumbers = { "2nd", "4th" };

        public MiKo_1086_MethodsWithNumberSlangAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => HasSlangNumber(symbol.Name)
                                                                                                                 ? new[] { Issue(symbol) }
                                                                                                                 : Enumerable.Empty<Diagnostic>();

        private static bool HasSlangNumber(string symbolName)
        {
            var name = symbolName;

            while (true)
            {
                var index = name.IndexOfAny(SlangNumbers);

                // ignore first character as that is never a number
                var hasSlangNumber = index > 0 && index < (name.Length - 1) && name[index + 1].IsLetter();

                if (hasSlangNumber)
                {
                    // we have to ignore "2nd" and "4th"
                    var withoutNumbers = name.Without(AllowedNumbers);

                    if (withoutNumbers != name)
                    {
                        name = withoutNumbers;

                        continue;
                    }
                }

                return hasSlangNumber;
            }
        }
    }
}