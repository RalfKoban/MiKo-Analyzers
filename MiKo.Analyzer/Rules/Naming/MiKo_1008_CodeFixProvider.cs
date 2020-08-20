using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1008_CodeFixProvider)), Shared]
    public sealed class MiKo_1008_CodeFixProvider : ParameterNamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1008_DependencyPropertyEventHandlingMethodParametersAnalyzer.Id;

        protected override string Title => "Rename DependencyProperty event handler argument";

        protected override string FindBetterName(IParameterSymbol symbol) => MiKo_1008_DependencyPropertyEventHandlingMethodParametersAnalyzer.FindBetterName(symbol);
    }
}