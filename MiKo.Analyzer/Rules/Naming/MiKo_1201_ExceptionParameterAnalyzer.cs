using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1201_ExceptionParameterAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1201";

        internal const string ExpectedName = "ex";

        public MiKo_1201_ExceptionParameterAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol) => symbol.Type.IsException();

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            switch (symbol.Name)
            {
                case ExpectedName:
                case "exception":
                case "innerException":
                    return Enumerable.Empty<Diagnostic>();

                case "inner":
                    if (symbol.ContainingSymbol.IsConstructor() && symbol.ContainingType.IsException())
                    {
                        return Enumerable.Empty<Diagnostic>();
                    }

                    break;

                default:
                    if (symbol.ContainingSymbol.IsConstructor() && symbol.MatchesProperty())
                    {
                        return Enumerable.Empty<Diagnostic>();
                    }

                    break;
            }

            return new[] { Issue(symbol, ExpectedName, "exception") };
        }
    }
}