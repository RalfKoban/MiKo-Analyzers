using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1039_CodeFixProvider)), Shared]
    public sealed class MiKo_1039_CodeFixProvider : ParameterNamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1039_ExtensionMethodsParameterAnalyzer.Id;

        protected override string Title => Resources.MiKo_1039_CodeFixTitle;

        protected override string FindBetterName(IParameterSymbol symbol) => MiKo_1039_ExtensionMethodsParameterAnalyzer.FindBetterName(symbol);
    }
}