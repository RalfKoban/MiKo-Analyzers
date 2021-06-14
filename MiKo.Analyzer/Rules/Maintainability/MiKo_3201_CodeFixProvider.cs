using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3201_CodeFixProvider)), Shared]
    public sealed class MiKo_3201_CodeFixProvider : SurroundedByBlankLinesCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3201_LogStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override string Title => Resources.MiKo_3201_CodeFixTitle;
    }
}