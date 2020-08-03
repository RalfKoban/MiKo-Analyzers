using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1112_CodeFixProvider)), Shared]
    public sealed class MiKo_1112_CodeFixProvider : NamingLocalVariableCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1112_TestsShouldNotUseArbitraryIdentifiersAnalyzer.Id;

        protected override string Title => "Remove '" + MiKo_1112_TestsShouldNotUseArbitraryIdentifiersAnalyzer.Phrase + "' from name";

        protected override string GetNewName(ISymbol symbol) => MiKo_1112_TestsShouldNotUseArbitraryIdentifiersAnalyzer.FindBetterName(symbol);
    }
}