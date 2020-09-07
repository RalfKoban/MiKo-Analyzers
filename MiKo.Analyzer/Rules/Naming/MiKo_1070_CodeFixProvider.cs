using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1070_CodeFixProvider)), Shared]
    public sealed class MiKo_1070_CodeFixProvider : NamingLocalVariableCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1070_CollectionLocalVariableAnalyzer.Id;

        protected override string Title => "Rename variable into plural";

        protected override string GetNewName(ISymbol symbol) => MiKo_1070_CollectionLocalVariableAnalyzer.FindBetterName(symbol);
    }
}