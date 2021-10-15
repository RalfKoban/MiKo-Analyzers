using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2309_CodeFixProvider)), Shared]
    public sealed class MiKo_2309_CodeFixProvider : SingleLineCommentCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2309_CommentContainsNtContradictionAnalyzer.Id;

        protected override string Title => Resources.MiKo_2309_CodeFixTitle;

        protected override bool IsTrivia => true;

        protected override SyntaxTrivia ComputeReplacementTrivia(SyntaxTrivia original, SyntaxTrivia rewritten)
        {
            var comment = original.ToString();

            if (MiKo_2213_DocumentationContainsNtContradictionAnalyzer.CommentHasIssue(comment.TrimEnd()))
            {
                return SyntaxFactory.Comment(GetFixedText(comment));
            }

            return original;
        }

        private static string GetFixedText(string text) => text
                                                           .Replace("an't", "annot")
                                                           .Replace("won't", "will not")
                                                           .Replace("Won't", "Will not")
                                                           .Replace("n't", " not");
    }
}