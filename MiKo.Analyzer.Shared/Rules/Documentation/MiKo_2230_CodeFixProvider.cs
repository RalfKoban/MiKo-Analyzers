using System;
using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2230_CodeFixProvider)), Shared]
    public sealed class MiKo_2230_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2230";

        protected override string Title => Resources.MiKo_2230_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic issue)
        {
            var token = syntax.FindToken(issue);

            return syntax.ReplaceNode(token.Parent, UpdateText(token));
        }

        private static XmlNodeSyntax[] UpdateText(SyntaxToken token)
        {
            const string ValueMeaning = Constants.Comments.ValueMeaningPhrase;

            var text = token.Text.AsSpan();
            var index = text.IndexOf(ValueMeaning.AsSpan(), StringComparison.Ordinal);

            var xmlText = XmlText(text.Slice(0, index).Trim().ToString());
            var xmlTable = XmlTable(text.Slice(index + ValueMeaning.Length));

            return new XmlNodeSyntax[]
                   {
                       xmlText.WithLeadingXmlComment(),
                       xmlTable.WithTrailingXmlComment(),
                   };
        }

        private static XmlElementSyntax XmlTable(ReadOnlySpan<char> listTexts)
        {
            // try to find all sentences by splitting the texts at the dots (that should indicate a sentence ending)
            var items = new List<XmlNodeSyntax>
                        {
                            XmlElement(Constants.XmlTag.ListHeader, Constants.Comments.ValuePhrase, Constants.Comments.MeaningPhrase),
                        };

            foreach (ReadOnlySpan<char> sentences in listTexts.SplitBy(".".AsSpan()))
            {
                var sentence = sentences.FirstSentence();
                var remainingText = sentences.Slice(sentence.Length);

                var term = sentence.Trim().ToString();
                var description = remainingText.Trim().ConcatenatedWith('.'); // append a dot as we had split it by the dots which due to that gone missing

                items.Add(XmlElement(Constants.XmlTag.Item, term, description));
            }

            return XmlList(Constants.XmlTag.ListType.Table, items);
        }

        private static XmlElementSyntax XmlElement(string tag, string term, string description) => XmlElement(tag, new[]
                                                                                                                   {
                                                                                                                       XmlElement(Constants.XmlTag.Term, XmlText(term)),
                                                                                                                       XmlElement(Constants.XmlTag.Description, XmlText(description)),
                                                                                                                   });
    }
}