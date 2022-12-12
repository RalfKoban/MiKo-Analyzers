using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6011_CodeFixProvider)), Shared]
    public sealed class MiKo_6011_CodeFixProvider : SurroundedByBlankLinesCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6011_LockStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override string Title => Resources.MiKo_6011_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<LockStatementSyntax>().FirstOrDefault();
    }
}