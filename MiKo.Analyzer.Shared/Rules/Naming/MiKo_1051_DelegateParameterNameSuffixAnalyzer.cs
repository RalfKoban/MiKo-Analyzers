using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1051_DelegateParameterNameSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1051";

        private static readonly string[] WrongNames = { "Action", "Delegate", "Func" };

        public MiKo_1051_DelegateParameterNameSuffixAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol)
        {
            var type = symbol.Type;

            switch (type.TypeKind)
            {
                case TypeKind.Delegate:
                    return true;

                case TypeKind.Class when type.IsRecord:
                    return false;

                case TypeKind.Class:
                    return type.ToString() == TypeNames.Delegate;

                default:
                    return false;
            }
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            if (symbol.Name.EndsWithAny(WrongNames))
            {
                yield return Issue(symbol, CreateBetterNameProposal("callback"));
            }
        }
    }
}