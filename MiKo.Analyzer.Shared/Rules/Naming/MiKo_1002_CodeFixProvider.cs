using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1002_CodeFixProvider)), Shared]
    public sealed class MiKo_1002_CodeFixProvider : ParameterNamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1002_EventHandlingMethodParametersAnalyzer.Id;

        protected override string Title => Resources.MiKo_1002_CodeFixTitle;

        protected override string FindBetterName(IParameterSymbol symbol, Diagnostic diagnostic) => MiKo_1002_EventHandlingMethodParametersAnalyzer.FindBetterName(symbol);
    }
}