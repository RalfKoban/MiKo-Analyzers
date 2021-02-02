using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3202_CodeFixProvider)), Shared]
    public class MiKo_3202_CodeFixProvider : SurroundedByBlankLinesCodeFixProvider
    {
        public sealed override string FixableDiagnosticId => MiKo_3202_AssertStatementSurroundedByBlankLinesAnalyzer.Id;

        protected sealed override string Title => Resources.MiKo_3202_CodeFixTitle;
    }
}