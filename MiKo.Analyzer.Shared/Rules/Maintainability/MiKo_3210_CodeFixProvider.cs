using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3210_CodeFixProvider)), Shared]
    public sealed class MiKo_3210_CodeFixProvider : SurroundedByBlankLinesCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3210_IfStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override string Title => Resources.MiKo_3210_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<IfStatementSyntax>().FirstOrDefault();
    }
}