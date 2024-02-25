using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1112_CodeFixProvider)), Shared]
    public sealed class MiKo_1112_CodeFixProvider : NamingLocalVariableCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_1112";

        protected override string Title => Resources.MiKo_1112_CodeFixTitle;

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => MiKo_1112_TestsShouldNotUseArbitraryIdentifiersAnalyzer.FindBetterName(symbol);
    }
}