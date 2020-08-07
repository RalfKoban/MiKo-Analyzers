using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1050_CodeFixProvider)), Shared]
    public sealed class MiKo_1050_CodeFixProvider : NamingLocalVariableCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1050_ReturnValueLocalVariableAnalyzer.Id;

        protected override string Title => "Rename return value";

        protected override string GetNewName(ISymbol symbol) => MiKo_1050_ReturnValueLocalVariableAnalyzer.FindBetterName(symbol);
    }
}