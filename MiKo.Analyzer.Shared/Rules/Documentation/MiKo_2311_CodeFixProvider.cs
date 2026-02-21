using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2311_CodeFixProvider)), Shared]
    public sealed class MiKo_2311_CodeFixProvider : CommentCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2311";

        protected override SyntaxTrivia ComputeReplacementTrivia(in SyntaxTrivia original, in SyntaxTrivia rewritten) => rewritten;

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, in SyntaxTrivia trivia, Diagnostic issue, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntaxRoot(root, trivia);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root, in SyntaxTrivia trivia) => root.ReplaceTrivia(new[] { trivia }, (original, rewritten) => SyntaxFactory.ElasticCarriageReturnLineFeed);
    }
}