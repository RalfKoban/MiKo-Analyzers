using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1060_UseNotFoundInsteadOfMissingAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1060";

        public MiKo_1060_UseNotFoundInsteadOfMissingAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsEnum() || symbol.IsException();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (symbol.IsEnum())
            {
                foreach (var member in symbol.GetFields())
                {
                    var diagnostic = AnalyzeName(member);

                    if (diagnostic != null)
                    {
                        yield return diagnostic;
                    }
                }
            }
            else
            {
                var diagnostic = AnalyzeName(symbol);

                if (diagnostic != null)
                {
                    yield return diagnostic;
                }
            }
        }

        private Diagnostic AnalyzeName(ISymbol symbol)
        {
            var name = symbol.Name;

            if (name.Contains("Missing"))
            {
                var betterName = name.Replace("Missing", "NotFound");

                return Issue(symbol, betterName, new Dictionary<string, string> { { Constants.BetterName, betterName } });
            }

            if (name.StartsWith("Get", StringComparison.InvariantCulture) && name.Contains("Failed"))
            {
                var betterName = name.Substring(3).Replace("Failed", "NotFound");

                return Issue(symbol, betterName, new Dictionary<string, string> { { Constants.BetterName, betterName } });
            }

            return null;
        }
    }
}