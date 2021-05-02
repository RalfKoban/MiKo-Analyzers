using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1043_CodeFixProvider)), Shared]
    public sealed class MiKo_1043_CodeFixProvider : NamingLocalVariableCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1043_CancellationTokenLocalVariableAnalyzer.Id;

        protected override string Title => "Name it '" + MiKo_1043_CancellationTokenLocalVariableAnalyzer.ExpectedName + "'";

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => MiKo_1043_CancellationTokenLocalVariableAnalyzer.ExpectedName;
    }
}