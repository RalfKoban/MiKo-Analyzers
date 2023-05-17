using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1201_ExceptionParameterAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1201";

        public MiKo_1201_ExceptionParameterAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol) => symbol.Type.IsException();

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            if (HasIssue(symbol))
            {
                yield return Issue(symbol, Constants.ExceptionIdentifier, "exception");
            }
        }

        private static bool HasIssue(IParameterSymbol symbol)
        {
            switch (symbol.Name)
            {
                case Constants.ExceptionIdentifier:
                case "exception":
                case "innerException":
                    return false;

                case Constants.InnerExceptionIdentifier:
                    var isConstructorInException = symbol.ContainingSymbol.IsConstructor() && symbol.ContainingType.IsException();

                    return isConstructorInException is false;

                default:
                    var isConstructorInProperty = symbol.ContainingSymbol.IsConstructor() && symbol.MatchesProperty();

                    return isConstructorInProperty is false;
            }
        }
    }
}