using System;
using System.Collections.Generic;
using System.Composition;
using System.Globalization;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2040_CodeFixProvider)), Shared]
    public sealed class MiKo_2040_CodeFixProvider : DocumentationCodeFixProvider
    {
        private static readonly string[] Phrases = MiKo_2040_LangwordAnalyzer.Phrases;

        public override string FixableDiagnosticId => MiKo_2040_LangwordAnalyzer.Id;

        protected override string Title => Resources.MiKo_2040_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var comment = (DocumentationCommentTriviaSyntax)syntax;

            comment = ReplaceCode(comment);
            comment = ReplaceValue(comment);
            comment = ReplaceWrongEmptySeeOrSeeAlso(comment);
            comment = ReplaceWrongNonEmptySeeOrSeeAlso(comment);
            comment = ReplaceText(comment);

            return comment;
        }

        private static DocumentationCommentTriviaSyntax ReplaceWrongEmptySeeOrSeeAlso(DocumentationCommentTriviaSyntax comment)
        {
            var nodes = comment.DescendantNodes().OfType<XmlEmptyElementSyntax>().Where(_ => _.IsEmptySee(MiKo_2040_LangwordAnalyzer.WrongAttributes) || _.IsEmptySeeAlso(MiKo_2040_LangwordAnalyzer.WrongAttributes)).ToList();

            return comment.ReplaceNodes(nodes, (original, rewritten) =>
                                                   {
                                                       var attribute = rewritten.Attributes.First() as XmlTextAttributeSyntax;
                                                       var textToken = attribute?.TextTokens[0];

                                                       return SeeLangword(textToken?.ValueText.ToLowerCase());
                                                   });
        }

        private static DocumentationCommentTriviaSyntax ReplaceWrongNonEmptySeeOrSeeAlso(DocumentationCommentTriviaSyntax comment)
        {
            var nodes = comment.DescendantNodes().OfType<XmlElementSyntax>().Where(_ => _.IsNonEmptySee(MiKo_2040_LangwordAnalyzer.WrongAttributes) || _.IsNonEmptySeeAlso(MiKo_2040_LangwordAnalyzer.WrongAttributes)).ToList();

            return comment.ReplaceNodes(nodes, (original, rewritten) =>
                                                   {
                                                       var attribute = rewritten.StartTag.Attributes.First() as XmlTextAttributeSyntax;
                                                       var textToken = attribute?.TextTokens[0];

                                                       return SeeLangword(textToken?.ValueText.ToLowerCase());
                                                   });
        }

        private static DocumentationCommentTriviaSyntax ReplaceCode(DocumentationCommentTriviaSyntax comment)
        {
            // replace all '<c>true</c>', '<c>false</c>' and '<c>null</c>', but ignore <code>
            var nodes = comment.DescendantNodes().OfType<XmlElementSyntax>().Where(_ => _.IsCBool() || _.IsCNull()).ToList();

            return comment.ReplaceNodes(nodes, (original, rewritten) => SeeLangword(rewritten.Content.ToString().ToLowerCase()));
        }

        private static DocumentationCommentTriviaSyntax ReplaceValue(DocumentationCommentTriviaSyntax comment)
        {
            // replace all '<value>true</value>', '<value>false</value>' and '<value>null</value>'
            var nodes = comment.DescendantNodes().OfType<XmlElementSyntax>().Where(_ => _.IsValueBool() || _.IsValueNull()).ToList();

            return comment.ReplaceNodes(nodes, (original, rewritten) => SeeLangword(rewritten.Content.ToString().ToLowerCase()));
        }

        private static DocumentationCommentTriviaSyntax ReplaceText(DocumentationCommentTriviaSyntax comment)
        {
            var nodes = new List<XmlTextSyntax>();
            foreach (var node in comment.DescendantNodes().OfType<XmlTextSyntax>())
            {
                if (node.Parent is XmlElementSyntax parent && parent.GetName() == Constants.XmlTag.Code)
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
            // by following algorithm:
            // 1. Create a dictionary with SyntaxAnnotations and replacement nodes for the node to annotate (new SyntaxAnnotation)
            // 2. Annotate the node to keep track (node.WithAdditionalAnnotations())
            // 3. Loop over all annotated nodes and replace them with the replacement nodes (document.GetAnnotatedNodes(annotation))
            var annotation = new SyntaxAnnotation();

            var newComment = comment.ReplaceNodes(nodes, (original, rewritten) => original.WithAdditionalAnnotations(annotation));

            while (true)
            {
                var oldNode = newComment.GetAnnotatedNodes(annotation).OfType<XmlTextSyntax>().FirstOrDefault();
                if (oldNode is null)
                {
                    // nothing left
                    return newComment;
                }

                // create replacement nodes, based on tokens
                newComment = newComment.ReplaceNode(oldNode, GetReplacements(oldNode));
            }
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
                    result.Add(SyntaxFactory.XmlText(string.Empty).WithLeadingXmlComment());

                    // we do not need to inspect further
                    newLineTokenJustSkipped = true;
                    continue;
                }

                var text = textToken.ValueText;

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
                        result.Add(SyntaxFactory.XmlText(part));
                    }
                }
            }

            return result;
        }
    }
}
