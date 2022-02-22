using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1061_CodeFixProvider)), Shared]
    public sealed class MiKo_1061_CodeFixProvider : ParameterNamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1061_TryMethodOutParameterNameAnalyzer.Id;

        protected override string Title => Resources.MiKo_1061_CodeFixTitle;

        protected override string FindBetterName(IParameterSymbol symbol) => MiKo_1061_TryMethodOutParameterNameAnalyzer.FindBetterName(symbol.GetEnclosingMethod());
    }
}