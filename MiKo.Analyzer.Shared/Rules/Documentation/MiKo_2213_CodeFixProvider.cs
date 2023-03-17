using System.Composition;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2213_CodeFixProvider)), Shared]
    public sealed class MiKo_2213_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2213_DocumentationContainsNtContradictionAnalyzer.Id;

        protected override string Title => Resources.MiKo_2213_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(CodeFixContext context, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var affectedTokens = syntax.GetXmlTextTokens()
                                       .Where(_ => _.GetLocation().Contains(diagnostic.Location))
                                       .Where(_ => _.ValueText.Contains("n't") || _.ValueText.Contains("nt"));

            return syntax.ReplaceTokens(affectedTokens, (original, rewritten) => original.WithText(GetFixedText(original.Text)));
        }

        private static StringBuilder GetFixedText(string text) => new StringBuilder(text).Replace("adnt", "ad not")
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
                                                                                         .Replace("n't", " not");

    }
}