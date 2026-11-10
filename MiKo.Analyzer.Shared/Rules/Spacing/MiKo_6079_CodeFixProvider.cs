using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6079_CodeFixProvider)), Shared]
    public sealed class MiKo_6079_CodeFixProvider : IndentedSpacingCodeFixProvider<BlockSyntax>
    {
        public override string FixableDiagnosticId => "MiKo_6079";

        protected override TSyntaxNode GetUpdatedSyntax<TSyntaxNode>(TSyntaxNode syntax, in int leadingSpaces) => GetUpdatedBlock(syntax as BlockSyntax, leadingSpaces) as TSyntaxNode;

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            if (syntax is BlockSyntax node)
            {
                var openBrace = node.OpenBraceToken;
                var previousToken = openBrace.GetPreviousToken();

                if (openBrace.IsOnSameLineAsEndOf(previousToken))
                {
                    var updatedRoot = root.ReplaceToken(previousToken, previousToken.WithForcedEndOfLine());

                    return base.GetUpdatedSyntaxRootAsync(document, updatedRoot, syntax, annotationOfSyntax, issue, cancellationToken);
                }
            }

            return base.GetUpdatedSyntaxRootAsync(document, root, syntax, annotationOfSyntax, issue, cancellationToken);
        }
    }
}