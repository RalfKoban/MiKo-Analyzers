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

        public MiKo_1201_ExceptionParameterAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol) => symbol.Type.IsException()
                                                                                                    ? AnalyzeName(symbol)
                                                                                                    : Enumerable.Empty<Diagnostic>();

        private IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol)
        {
            switch (symbol.Name)
            {
                case ExceptionIdentifier1:
                case ExceptionIdentifier2:
                    return Enumerable.Empty<Diagnostic>();

                default:
                    return new[] { ReportIssue(symbol, ExceptionIdentifier1, ExceptionIdentifier2) };
            }
        }
    }
}