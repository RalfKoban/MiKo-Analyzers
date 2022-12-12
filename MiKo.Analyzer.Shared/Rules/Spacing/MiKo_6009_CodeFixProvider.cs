using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6009_CodeFixProvider)), Shared]
    public sealed class MiKo_6009_CodeFixProvider : SurroundedByBlankLinesCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6009_TryStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override string Title => Resources.MiKo_6009_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<StatementSyntax>().FirstOrDefault();
    }
}