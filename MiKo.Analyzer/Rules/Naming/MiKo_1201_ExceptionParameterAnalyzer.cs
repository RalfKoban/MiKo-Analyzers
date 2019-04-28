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

        private const string ExceptionIdentifier1 = "ex";
        private const string ExceptionIdentifier2 = "exception";
        private const string InnerExceptionIdentifier = "inner";
        private const string InnerExceptionIdentifier2 = "innerException";

        public MiKo_1201_ExceptionParameterAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol) => symbol.Type.IsException();

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol)
        {
            switch (symbol.Name)
            {
                case ExceptionIdentifier1:
                case ExceptionIdentifier2:
                    return Enumerable.Empty<Diagnostic>();

                case InnerExceptionIdentifier:
                case InnerExceptionIdentifier2:
                    if (symbol.ContainingSymbol.IsConstructor() && symbol.ContainingType.IsException())
                        return Enumerable.Empty<Diagnostic>();

                    break;
            }

            return new[] { Issue(symbol, ExceptionIdentifier1, ExceptionIdentifier2) };
        }
    }
}