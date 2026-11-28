using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1543_FactoriesWithPropertiesOnlyAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1543";

        private static readonly HashSet<string> NamesToIgnore = new HashSet<string>(StringComparer.Ordinal)
                                                                    {
                                                                        nameof(Equals),
                                                                        nameof(GetHashCode),
                                                                        nameof(GetType),
                                                                        nameof(ToString),
                                                                    };

        public MiKo_1543_FactoriesWithPropertiesOnlyAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name.AsSpan();

            return symbolName.EndsWith("Factory")
                || symbolName.EndsWith("Builder");
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            var methods = symbol.GetMethods(MethodKind.Ordinary).SkipWhere(_ => NamesToIgnore.Contains(_.Name));

            if (methods.None())
            {
                var properties = symbol.GetProperties();

                if (properties.Count > 0)
                {
                    var betterName = symbol.Name
                                           .Replace("Factory", "Provider")
                                           .Replace("Builder", "Provider");

                    return new[] { Issue(symbol, betterName) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}