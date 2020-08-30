using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1090_CodeFixProvider)), Shared]
    public sealed class MiKo_1090_CodeFixProvider : ParameterNamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1090_ParametersWrongSuffixedAnalyzer.Id;

        protected override string Title => "Rename parameter";

        protected override string FindBetterName(IParameterSymbol symbol) => MiKo_1090_ParametersWrongSuffixedAnalyzer.FindBetterName(symbol);
    }
}