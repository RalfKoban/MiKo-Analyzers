using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    // TODO: RKN [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2012_FireMethodsAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2012";

        public MiKo_2012_FireMethodsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }
    }
}