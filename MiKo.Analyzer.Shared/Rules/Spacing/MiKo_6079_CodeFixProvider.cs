using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6079_CodeFixProvider)), Shared]
    public sealed class MiKo_6079_CodeFixProvider : IndentedSpacingCodeFixProvider<BlockSyntax>
    {
        public override string FixableDiagnosticId => "MiKo_6079";

        protected override TSyntaxNode GetUpdatedSyntax<TSyntaxNode>(TSyntaxNode syntax, in int leadingSpaces) => syntax is BlockSyntax node
                                                                                                                   ? GetUpdatedBlock(node, leadingSpaces) as TSyntaxNode
                                                                                                                   : syntax;
    }
}