using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1526_DelegatePropertyNameSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1526";

        private static readonly string[] WrongSuffixes =
                                                         {
                                                             "Action",
                                                             "Delegate",
                                                             "Func",
                                                         };

        public MiKo_1526_DelegatePropertyNameSuffixAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation)
        {
            if (symbol.Type.IsDelegate())
            {
                var symbolName = symbol.Name.AsSpan();

                if (symbolName.EndsWith(Constants.Names.Callback) is false)
                {
                    var betterName = FindBetterName(symbolName);

                    return new[] { Issue(symbol, CreateBetterNameProposal(betterName)) };
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(in ReadOnlySpan<char> symbolName)
        {
            var nameWithoutSuffix = symbolName.WithoutSuffixes(WrongSuffixes);

            return nameWithoutSuffix.ConcatenatedWith(Constants.Names.Callback);
        }
    }
}