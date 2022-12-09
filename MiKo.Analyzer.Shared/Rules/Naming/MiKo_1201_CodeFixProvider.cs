using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1201_CodeFixProvider)), Shared]
    public sealed class MiKo_1201_CodeFixProvider : ParameterNamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1201_ExceptionParameterAnalyzer.Id;

        protected override string Title => Resources.MiKo_1201_CodeFixTitle;

        protected override string FindBetterName(IParameterSymbol symbol, Diagnostic diagnostic) => Constants.ExceptionIdentifier;
    }
}