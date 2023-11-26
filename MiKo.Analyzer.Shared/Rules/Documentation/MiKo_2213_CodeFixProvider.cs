using System.Collections.Generic;
using System.Composition;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2213_CodeFixProvider)), Shared]
    public sealed class MiKo_2213_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly KeyValuePair<string, string>[] ReplacementMap =
                                                                                {
                                                                                    new KeyValuePair<string, string>("adnt", "ad not"),
                                                                                    new KeyValuePair<string, string>("an't", "annot"),
                                                                                    new KeyValuePair<string, string>("arent", "are not"),
                                                                                    new KeyValuePair<string, string>("Arent", "Are not"),
                                                                                    new KeyValuePair<string, string>("asnt", "as not"),
                                                                                    new KeyValuePair<string, string>("avent", "ave not"),
                                                                                    new KeyValuePair<string, string>("cant", "cannot"),
                                                                                    new KeyValuePair<string, string>("Cant", "Cannot"),
                                                                                    new KeyValuePair<string, string>("dont", "do not"),
                                                                                    new KeyValuePair<string, string>("Dont", "Do not"),
                                                                                    new KeyValuePair<string, string>("eednt", "eed not"),
                                                                                    new KeyValuePair<string, string>("erent", "ere not"),
                                                                                    new KeyValuePair<string, string>("idnt", "id not"),
                                                                                    new KeyValuePair<string, string>("oesnt", "oes not"),
                                                                                    new KeyValuePair<string, string>("ouldnt", "ould not"),
                                                                                    new KeyValuePair<string, string>("snt", "s not"),
                                                                                    new KeyValuePair<string, string>("wont", "will not"),
                                                                                    new KeyValuePair<string, string>("won't", "will not"),
                                                                                    new KeyValuePair<string, string>("Wont", "Will not"),
                                                                                    new KeyValuePair<string, string>("Won't", "Will not"),
                                                                                    new KeyValuePair<string, string>("n't", " not"),
                                                                                };

        public override string FixableDiagnosticId => MiKo_2213_DocumentationContainsNtContradictionAnalyzer.Id;

        protected override string Title => Resources.MiKo_2213_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var token = syntax.FindToken(diagnostic);
            var text = new StringBuilder(token.ValueText).ReplaceAllWithCheck(ReplacementMap);

            return syntax.ReplaceToken(token, token.WithText(text));
        }
    }
}