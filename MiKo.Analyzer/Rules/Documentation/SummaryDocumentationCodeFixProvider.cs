using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class SummaryDocumentationCodeFixProvider : DocumentationCodeFixProvider
    {
        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes, Constants.XmlTag.Summary).First();

        protected SyntaxNode BeginCommentWith(XmlElementSyntax comment, string phrase)
        {
            var content = comment.Content;

            // when necessary adjust beginning text
            // Note: when on new line, then the text is not the 1st one but the 2nd one
            var index = GetIndex(content);

            XmlTextSyntax newText;
            if (content[index] is XmlTextSyntax text)
            {
                // we have to remove the element as otherwise we duplicate the comment
                content = content.Remove(content[index]);
                newText = text.WithStartText(phrase);
            }
            else
            {
                newText = SyntaxFactory.XmlText(phrase);
            }

            return SyntaxFactory.XmlElement(
                                            comment.StartTag,
                                            content.Insert(index, newText),
                                            comment.EndTag);
        }

        private static int GetIndex(SyntaxList<XmlNodeSyntax> content)
        {
            var onlyWhitespaceText = content[0] is XmlTextSyntax t && GetText(t).IsNullOrWhiteSpace();
            return onlyWhitespaceText ? 1 : 0;
        }

        private static string GetText(XmlTextSyntax text)
        {
            return string.Concat(text.TextTokens.Select(_ => _.WithoutTrivia()));
        }
    }
}