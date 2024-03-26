using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2303_CodeFixProvider)), Shared]
    public sealed class MiKo_2303_CodeFixProvider : CommentCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2303";

        protected override string Title => Resources.MiKo_2303_CodeFixTitle;

        protected override SyntaxTrivia ComputeReplacementTrivia(SyntaxTrivia original, SyntaxTrivia rewritten)
        {
            var comment = original.ToString().AsSpan().TrimEnd();

            if (DocumentationComment.EndsWithPeriod(comment))
            {
                // ensure that there is no empty space at the end, after removing trailing dots
                return SyntaxFactory.Comment(comment.TrimEnd('.').TrimEnd().ToString());
            }

            return original;
        }
    }
}