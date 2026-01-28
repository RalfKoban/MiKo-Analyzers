using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1523_HelperMethodAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1523";

        private static readonly string[] Terms = { "Helper", "HelpingMethod" };

        public MiKo_1523_HelperMethodAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            for (int index = 0, count = Terms.Length; index < count; index++)
            {
                var term = Terms[index];

                if (symbolName.Contains(term, StringComparison.Ordinal))
                {
                    return new[] { Issue(symbol, term) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}