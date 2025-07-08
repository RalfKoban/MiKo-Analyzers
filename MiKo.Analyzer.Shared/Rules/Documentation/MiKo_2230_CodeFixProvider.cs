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

            if (token.Parent is XmlTextSyntax node)
            {
                var text = node.GetTextTrimmed();

                return syntax.ReplaceNode(node, UpdateText(text.AsSpan()));
            }

            return syntax;
        }

        private static XmlNodeSyntax[] UpdateText(ReadOnlySpan<char> text)
        {
            const string ValueMeaning = Constants.Comments.ValueMeaningPhrase;

            var index = text.IndexOf(ValueMeaning.AsSpan(), StringComparison.Ordinal);

            var xmlText = XmlText(text.Slice(0, index).Trim().ToString());
            var xmlTable = XmlTable(text.Slice(index + ValueMeaning.Length));

            return new XmlNodeSyntax[]
                       {
                           xmlText.WithLeadingXmlComment(),
                           xmlTable.WithTrailingXmlComment(),
                       };
        }

        private static XmlElementSyntax XmlTable(ReadOnlySpan<char> text)
        {
            // try to find all sentences by splitting the texts at the dots (that should indicate a sentence ending)
            var nodes = new List<XmlNodeSyntax>
                            {
                                XmlElement(Constants.XmlTag.ListHeader, Constants.Comments.ValuePhrase, Constants.Comments.MeaningPhrase),
                            };

            if (TryCreateItems(text, nodes) is false)
            {
                foreach (ReadOnlySpan<char> sentences in text.SplitBy(".".AsSpan()))
                {
                    var sentence = sentences.FirstSentence();
                    var remainingText = sentences.Slice(sentence.Length);

                    var term = sentence.Trim().ToString();
                    var description = remainingText.Trim().ConcatenatedWith('.'); // append a dot as we had split it by the dots which due to that gone missing

                    nodes.Add(XmlElement(Constants.XmlTag.Item, term, description));
                }
            }

            return XmlList(Constants.XmlTag.ListType.Table, nodes);
        }

        private static bool TryCreateItems(ReadOnlySpan<char> text, List<XmlNodeSyntax> nodes)
        {
            const string LessThanZero = Constants.Comments.LessThanZero;
            const string Zero = Constants.Comments.Zero;
            const string GreaterThanZero = Constants.Comments.GreaterThanZero;

            var lessThanZeroStart = text.IndexOf(LessThanZero.AsSpan());
            var zeroStart = text.IndexOf(Zero.AsSpan());
            var greaterThanZeroStart = text.IndexOf(GreaterThanZero.AsSpan());

            if (lessThanZeroStart < 0 || zeroStart <= lessThanZeroStart || greaterThanZeroStart <= zeroStart)
            {
                return false;
            }

            var lessThanZeroEnd = lessThanZeroStart + LessThanZero.Length;
            var zeroEnd = zeroStart + Zero.Length;
            var greaterThanZeroEnd = greaterThanZeroStart + GreaterThanZero.Length;

            var lessThanZeroText = text.Slice(lessThanZeroEnd, zeroStart - lessThanZeroEnd).Trim().ToString();
            var zeroText = text.Slice(zeroEnd, greaterThanZeroStart - zeroEnd).Trim().ToString();
            var greaterThanZeroText = text.Slice(greaterThanZeroEnd).Trim().ToString();

            nodes.Add(XmlElement(Constants.XmlTag.Item, LessThanZero, lessThanZeroText));
            nodes.Add(XmlElement(Constants.XmlTag.Item, Zero, zeroText));
            nodes.Add(XmlElement(Constants.XmlTag.Item, GreaterThanZero, greaterThanZeroText));

            return true;
        }

        private static XmlElementSyntax XmlElement(string tag, string term, string description) => XmlElement(tag, new[]
                                                                                                                       {
                                                                                                                           XmlElement(Constants.XmlTag.Term, XmlText(term)),
                                                                                                                           XmlElement(Constants.XmlTag.Description, XmlText(description)),
                                                                                                                       });
    }
}