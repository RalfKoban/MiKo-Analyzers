using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2040_CodeFixProvider)), Shared]
    public sealed class MiKo_2040_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly string[] Phrases = MiKo_2040_LangwordAnalyzer.Phrases;

        public override string FixableDiagnosticId => MiKo_2040_LangwordAnalyzer.Id;

        protected override string Title => Resources.MiKo_2040_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var comment = syntax;

            comment = ReplaceWrongTag(comment);
            comment = ReplaceWrongEmptySeeOrSeeAlso(comment);
            comment = ReplaceWrongNonEmptySeeOrSeeAlso(comment);
            comment = ReplaceText(comment);

            return comment;
        }

        private static DocumentationCommentTriviaSyntax ReplaceWrongEmptySeeOrSeeAlso(DocumentationCommentTriviaSyntax comment)
        {
            var nodes = comment.DescendantNodes<XmlEmptyElementSyntax>(_ => _.IsSee(MiKo_2040_LangwordAnalyzer.WrongAttributes) || _.IsSeeAlso(MiKo_2040_LangwordAnalyzer.WrongAttributes)).ToList();

            return comment.ReplaceNodes(nodes, (original, rewritten) =>
                                                                       {
                                                                           var attribute = rewritten.Attributes.First() as XmlTextAttributeSyntax;
                                                                           var text = attribute.GetTextWithoutTrivia();

                                                                           return SeeLangword(text.ToLowerCase());
                                                                       });
        }

        private static DocumentationCommentTriviaSyntax ReplaceWrongNonEmptySeeOrSeeAlso(DocumentationCommentTriviaSyntax comment)
        {
            var nodes = comment.DescendantNodes<XmlElementSyntax>(_ => _.IsSee(MiKo_2040_LangwordAnalyzer.WrongAttributes) || _.IsSeeAlso(MiKo_2040_LangwordAnalyzer.WrongAttributes)).ToList();

            return comment.ReplaceNodes(nodes, (original, rewritten) =>
                                                                       {
                                                                           var attribute = rewritten.StartTag.Attributes.First() as XmlTextAttributeSyntax;
                                                                           var text = attribute.GetTextWithoutTrivia();

                                                                           return SeeLangword(text.ToLowerCase());
                                                                       });
        }

        private static DocumentationCommentTriviaSyntax ReplaceWrongTag(DocumentationCommentTriviaSyntax comment)
        {
            // replace all '<b>true</b>', '<b>false</b>' and '<b>null</b>'
            // replace all '<c>true</c>', '<c>false</c>' and '<c>null</c>'
            // replace all '<value>true</value>', '<value>false</value>' and '<value>null</value>'
            var nodes = comment.DescendantNodes<XmlElementSyntax>(_ => _.IsWrongBooleanTag() || _.IsWrongNullTag()).ToList();

            return comment.ReplaceNodes(nodes, (original, rewritten) => SeeLangword(rewritten.Content.ToString().ToLowerCase()));
        }

        private static DocumentationCommentTriviaSyntax ReplaceText(DocumentationCommentTriviaSyntax comment)
        {
            var nodes = new List<XmlTextSyntax>();

            foreach (var node in comment.DescendantNodes<XmlTextSyntax>())
            {
                if (node.Parent.IsCode())
                {
                    // skip <code> samples
                    continue;
                }

                if (node.TextTokens.Any(__ => __.ValueText.ContainsAny(Phrases, StringComparison.OrdinalIgnoreCase)))
                {
                    nodes.Add(node);
                }
            }

            if (nodes.Count == 0)
            {
                return comment;
            }

            // replace all ' true ', ' true:', ' true,', ' true.', ' true)',  ' true!' or  ' true?' (same for false or null)
            return comment.ReplaceNodes(nodes, GetReplacements);
        }

        private static IEnumerable<SyntaxNode> GetReplacements(XmlTextSyntax node)
        {
            var textTokens = node.TextTokens;
            var result = new List<SyntaxNode>(textTokens.Count * 2);

            var newLineTokenJustSkipped = false;

            foreach (var textToken in textTokens)
            {
                // special handling of new lines
                if (textToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    // keep new line
                    result.Add(XmlText(string.Empty).WithLeadingXmlComment());

                    // we do not need to inspect further
                    newLineTokenJustSkipped = true;

                    continue;
                }

                var text = textToken.ValueText.AsSpan();

                // get rid of leading whitespace characters caused by '/// '
                if (newLineTokenJustSkipped)
                {
                    text = text.TrimStart();
                }

                newLineTokenJustSkipped = false;

                var parts = text.SplitBy(Phrases);

                foreach (var part in parts)
                {
                    if (Phrases.Any(_ => _.Equals(part, StringComparison.OrdinalIgnoreCase)))
                    {
                        result.Add(SeeLangword(part.ToLowerCase()));
                    }
                    else
                    {
                        result.Add(XmlText(part));
                    }
                }
            }

            return result;
        }
    }
}
