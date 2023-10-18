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
        public override string FixableDiagnosticId => MiKo_2309_CommentContainsNtContradictionAnalyzer.Id;

        protected override string Title => Resources.MiKo_2309_CodeFixTitle;

        protected override SyntaxTrivia ComputeReplacementTrivia(SyntaxTrivia original, SyntaxTrivia rewritten)
        {
            var comment = original.ToString();

            if (MiKo_2213_DocumentationContainsNtContradictionAnalyzer.CommentHasIssue(comment.TrimEnd()))
            {
                return SyntaxFactory.Comment(GetFixedText(comment));
            }

            return original;
        }

        private static string GetFixedText(string text) => new StringBuilder(text).ReplaceWithCheck("adnt", "ad not")
                                                                                  .ReplaceWithCheck("an't", "annot")
                                                                                  .ReplaceWithCheck("arent", "are not")
                                                                                  .ReplaceWithCheck("Arent", "Are not")
                                                                                  .ReplaceWithCheck("asnt", "as not")
                                                                                  .ReplaceWithCheck("avent", "ave not")
                                                                                  .ReplaceWithCheck("cant", "cannot")
                                                                                  .ReplaceWithCheck("Cant", "Cannot")
                                                                                  .ReplaceWithCheck("dont", "do not")
                                                                                  .ReplaceWithCheck("Dont", "Do not")
                                                                                  .ReplaceWithCheck("eednt", "eed not")
                                                                                  .ReplaceWithCheck("erent", "ere not")
                                                                                  .ReplaceWithCheck("idnt", "id not")
                                                                                  .ReplaceWithCheck("oesnt", "oes not")
                                                                                  .ReplaceWithCheck("ouldnt", "ould not")
                                                                                  .ReplaceWithCheck("snt", "s not")
                                                                                  .ReplaceWithCheck("wont", "will not")
                                                                                  .ReplaceWithCheck("won't", "will not")
                                                                                  .ReplaceWithCheck("Wont", "Will not")
                                                                                  .ReplaceWithCheck("Won't", "Will not")
                                                                                  .ReplaceWithCheck("n't", " not")
                                                                                  .ToString();
    }
}