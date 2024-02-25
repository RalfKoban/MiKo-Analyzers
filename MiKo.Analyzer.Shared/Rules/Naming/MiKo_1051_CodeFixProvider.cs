using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1051_CodeFixProvider)), Shared]
    public sealed class MiKo_1051_CodeFixProvider : ParameterNamingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_1051";

        protected override string Title => Resources.MiKo_1051_CodeFixTitle;

        protected override string FindBetterName(IParameterSymbol symbol, Diagnostic diagnostic) => MiKo_1051_DelegateParameterNameSuffixAnalyzer.ExpectedName;
    }
}