using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1001_CodeFixProvider)), Shared]
    public sealed class MiKo_1001_CodeFixProvider : ParameterNamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1001_EventArgsParameterAnalyzer.Id;

        protected override string Title => "Rename event argument";

        protected override string FindBetterName(IParameterSymbol symbol) => MiKo_1001_EventArgsParameterAnalyzer.FindBetterName(symbol);
    }
}