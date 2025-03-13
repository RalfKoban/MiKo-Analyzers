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
        public override string FixableDiagnosticId => "MiKo_6005";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault(IsReturnStatement);

        private static bool IsReturnStatement(SyntaxNode node)
        {
            switch (node.RawKind)
            {
                case (int)SyntaxKind.ReturnStatement:
                case (int)SyntaxKind.YieldReturnStatement:
                    return true;

                default:
                    return false;
            }
        }
    }
}