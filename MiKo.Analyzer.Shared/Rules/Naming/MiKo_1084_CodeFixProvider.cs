using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1084_CodeFixProvider)), Shared]
    public sealed class MiKo_1084_CodeFixProvider : NamingLocalVariableCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_1084";

        protected override string Title => Resources.MiKo_1084_CodeFixTitle;

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => MiKo_1084_VariablesWithNumberSuffixAnalyzer.FindBetterName(symbol);
    }
}