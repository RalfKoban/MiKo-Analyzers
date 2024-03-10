using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1053_DelegateFieldNameSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1053";

        private static readonly string[] WrongNames = { "Action", "Delegate", "Func" };

        private static readonly string[] WrongSuffixes =
                                                         {
                                                             "action",
                                                             "delegate",
                                                             "func",
                                                             "Action",
                                                             "Delegate",
                                                             "Func",
                                                         };

        public MiKo_1053_DelegateFieldNameSuffixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation)
        {
            var symbolType = symbol.Type;

            switch (symbolType.TypeKind)
            {
                case TypeKind.Delegate:
                case TypeKind.Class when symbolType.ToString() == TypeNames.Delegate:
                {
                    if (symbol.Name.EndsWithAny(WrongNames))
                    {
                        var betterName = FindBetterName(symbol);

                        return new[] { Issue(symbol, CreateBetterNameProposal(betterName)) };
                    }

                    return Enumerable.Empty<Diagnostic>();
                }

                default:
                    return Enumerable.Empty<Diagnostic>();
            }
        }

        private static string FindBetterName(IFieldSymbol symbol)
        {
            var symbolName = symbol.Name;

            foreach (var suffix in WrongSuffixes.Where(symbolName.EndsWith))
            {
                var newName = symbolName.Without(suffix);
                var upperCase = newName.Length > 0 && symbolName[newName.Length].IsUpperCase();
                var correctSuffix = upperCase ? "Callback" : "callback";

                return newName + correctSuffix;
            }

            return null;
        }
    }
}