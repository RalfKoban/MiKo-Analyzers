using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6005_CodeFixProvider)), Shared]
    public sealed class MiKo_6005_CodeFixProvider : SurroundedByBlankLinesCodeFixProvider
    {
        private static readonly SyntaxKind[] ReturnStatements = { SyntaxKind.ReturnStatement, SyntaxKind.YieldReturnStatement };

        public override string FixableDiagnosticId => "MiKo_6005";

        protected override string Title => Resources.MiKo_6005_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault(_ => _.IsAnyKind(ReturnStatements));
    }
}