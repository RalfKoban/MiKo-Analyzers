using System;
using System.Composition;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2312_CodeFixProvider)), Shared]
    public sealed class MiKo_2312_CodeFixProvider : CommentCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2312";

        protected override SyntaxTrivia ComputeReplacementTrivia(in SyntaxTrivia original, in SyntaxTrivia rewritten)
        {
            var comment = original.ToString();

            if (DocumentationComment.ContainsPhrases(Constants.Comments.WhichIsToTerms, comment.AsSpan().TrimStart()))
            {
                var text = comment.AsCachedBuilder().ReplaceAllWithProbe(Constants.Comments.WhichIsToTerms, Constants.Comments.ToTerm).ToStringAndRelease();

                return SyntaxFactory.Comment(text);
            }

            return original;
        }
    }
}