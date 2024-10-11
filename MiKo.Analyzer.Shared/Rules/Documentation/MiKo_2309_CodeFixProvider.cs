using System;
using System.Composition;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2309_CodeFixProvider)), Shared]
    public sealed class MiKo_2309_CodeFixProvider : CommentCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2309";

        protected override SyntaxTrivia ComputeReplacementTrivia(SyntaxTrivia original, SyntaxTrivia rewritten)
        {
            var comment = original.ToString();

            if (DocumentationComment.ContainsPhrases(Constants.Comments.NotContradictionPhrase, comment.AsSpan().TrimEnd()))
            {
                var text = comment.AsBuilder()
                                  .ReplaceAllWithCheck(Constants.Comments.NotContradictionReplacementMap.AsSpan())
                                  .ToString();

                return SyntaxFactory.Comment(text);
            }

            return original;
        }
    }
}