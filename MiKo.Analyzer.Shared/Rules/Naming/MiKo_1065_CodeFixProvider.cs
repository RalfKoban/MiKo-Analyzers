using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1065_CodeFixProvider)), Shared]
    public sealed class MiKo_1065_CodeFixProvider : ParameterNamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1065_OperatorParameterNameAnalyzer.Id;

        protected override string Title => Resources.MiKo_1065_CodeFixTitle;

        protected override string FindBetterName(IParameterSymbol symbol) => MiKo_1065_OperatorParameterNameAnalyzer.FindBetterName(symbol);
    }
}