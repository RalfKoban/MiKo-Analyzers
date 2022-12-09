using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1033_CodeFixProvider)), Shared]
    public sealed class MiKo_1033_CodeFixProvider : ParameterNamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1033_ParameterModelSuffixAnalyzer.Id;

        protected override string Title => Resources.MiKo_1033_CodeFixTitle;

        protected override string FindBetterName(IParameterSymbol symbol, Diagnostic diagnostic) => MiKo_1033_ParameterModelSuffixAnalyzer.FindBetterName(symbol);
    }
}