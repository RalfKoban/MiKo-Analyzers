using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class XmlTextDocumentationCodeFixProvider : DocumentationCodeFixProvider
    {
        protected static XmlTextSyntax GetUpdatedXmlText(XmlTextSyntax xmlText, in ReadOnlySpan<string> terms, string endingTerm, in ReadOnlySpan<Pair> replacementMap, in string endingReplacement)
        {
            var updatedXmlText = Comment(xmlText, terms, replacementMap);

            var ending = ' '.ConcatenatedWith(endingTerm);

            foreach (var textToken in updatedXmlText.GetXmlTextTokens())
            {
                var text = textToken.ValueText;

                if (text.EndsWith(ending, StringComparison.OrdinalIgnoreCase))
                {
                    var replacement = text.AsSpan(0, text.Length - ending.Length + 1) // 1 as length of the ' ' character
                                          .ConcatenatedWith(endingReplacement);

                    updatedXmlText = updatedXmlText.ReplaceText(text, replacement);
                }
            }

            return updatedXmlText;
        }

        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<XmlTextSyntax>().FirstOrDefault();

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => syntax is XmlTextSyntax text
                                                                                                                         ? GetUpdatedSyntax(document, text, issue)
                                                                                                                         : syntax;

        protected abstract XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue);
    }
}