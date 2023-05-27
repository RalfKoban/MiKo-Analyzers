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

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var affectedTokens = syntax.GetXmlTextTokens()
                                       .Where(_ => _.GetLocation().Contains(diagnostic.Location))
                                       .Where(_ => _.ValueText.Contains("n't") || _.ValueText.Contains("nt"));

            return syntax.ReplaceTokens(affectedTokens, (original, rewritten) => original.WithText(GetFixedText(original.Text)));
        }

        private static StringBuilder GetFixedText(string text) => new StringBuilder(text).ReplaceWithCheck("adnt", "ad not")
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
                                                                                         .ReplaceWithCheck("n't", " not");
    }
}