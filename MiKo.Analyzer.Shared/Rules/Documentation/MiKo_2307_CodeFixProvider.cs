using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2307_CodeFixProvider)), Shared]
    public sealed class MiKo_2307_CodeFixProvider : CommentCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2307";

        protected override SyntaxTrivia ComputeReplacementTrivia(SyntaxTrivia original, SyntaxTrivia rewritten)
        {
            var comment = original.ToString();

            const string Phrase = Constants.Comments.WasNotSuccessfulPhrase;

            if (DocumentationComment.ContainsPhrase(Phrase, comment.AsSpan().TrimEnd()))
            {
                return SyntaxFactory.Comment(comment.Replace(Phrase, "failed"));
            }

            return original;
        }
    }
}