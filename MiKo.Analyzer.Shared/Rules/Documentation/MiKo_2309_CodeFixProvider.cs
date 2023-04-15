using System.Composition;
using System.Text;

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

        protected override SyntaxTrivia ComputeReplacementTrivia(SyntaxTrivia original, SyntaxTrivia rewritten)
        {
            var comment = original.ToString();

            if (MiKo_2213_DocumentationContainsNtContradictionAnalyzer.CommentHasIssue(comment.TrimEnd()))
            {
                return SyntaxFactory.Comment(GetFixedText(comment));
            }

            return original;
        }

        private static string GetFixedText(string text) => new StringBuilder(text).Replace("adnt", "ad not")
                                                                                  .Replace("an't", "annot")
                                                                                  .Replace("arent", "are not")
                                                                                  .Replace("Arent", "Are not")
                                                                                  .Replace("asnt", "as not")
                                                                                  .Replace("avent", "ave not")
                                                                                  .Replace("cant", "cannot")
                                                                                  .Replace("Cant", "Cannot")
                                                                                  .Replace("dont", "do not")
                                                                                  .Replace("Dont", "Do not")
                                                                                  .Replace("eednt", "eed not")
                                                                                  .Replace("erent", "ere not")
                                                                                  .Replace("idnt", "id not")
                                                                                  .Replace("oesnt", "oes not")
                                                                                  .Replace("ouldnt", "ould not")
                                                                                  .Replace("snt", "s not")
                                                                                  .Replace("wont", "will not")
                                                                                  .Replace("won't", "will not")
                                                                                  .Replace("Wont", "Will not")
                                                                                  .Replace("Won't", "Will not")
                                                                                  .Replace("n't", " not")
                                                                                  .ToString();
    }
}