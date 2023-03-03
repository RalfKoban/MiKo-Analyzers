using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6021_CodeFixProvider)), Shared]
    public sealed class MiKo_6021_CodeFixProvider : SurroundedByBlankLinesCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6021_ArgumentNullExceptionThrowIfNullStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override string Title => Resources.MiKo_6021_CodeFixTitle;
    }
}