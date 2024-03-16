using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6024_CodeFixProvider)), Shared]
    public sealed class MiKo_6024_CodeFixProvider : SurroundedByBlankLinesCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6024_ObjectDisposedExceptionThrowIfStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override string Title => Resources.MiKo_6024_CodeFixTitle;
    }
}