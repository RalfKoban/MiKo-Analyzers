using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1091_CodeFixProvider)), Shared]
    public sealed class MiKo_1091_CodeFixProvider : NamingLocalVariableCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_1091";

        protected override string Title => Resources.MiKo_1091_CodeFixTitle;

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => MiKo_1091_VariableWrongSuffixedAnalyzer.FindBetterName(symbol);
    }
}