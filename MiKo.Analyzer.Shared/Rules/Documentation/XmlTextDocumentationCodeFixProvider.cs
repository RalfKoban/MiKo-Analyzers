using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class XmlTextDocumentationCodeFixProvider : DocumentationCodeFixProvider
    {
        protected static XmlTextSyntax GetUpdatedSyntax(XmlTextSyntax syntax, Diagnostic issue, in ReadOnlySpan<Pair> replacementPairs, string endingTerm = null, string endingReplacement = null)
        {
            var token = syntax.FindToken(issue);
            var text = token.ValueText.AsCachedBuilder().ReplaceAllWithProbe(replacementPairs).ToStringAndRelease();

            if (endingTerm != null && text.EndsWith(endingTerm, StringComparison.OrdinalIgnoreCase))
            {
                var ending = ' '.ConcatenatedWith(endingTerm);

                if (text.EndsWith(ending, StringComparison.OrdinalIgnoreCase))
                {
                    text = text.AsSpan(0, text.Length - ending.Length + 1) // 1 as length of the ' ' character
                               .ConcatenatedWith(endingReplacement);
                }
            }

            return syntax.ReplaceToken(token, token.WithText(text));
        }

        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<XmlTextSyntax>().FirstOrDefault();

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => syntax is XmlTextSyntax text
                                                                                                                         ? GetUpdatedSyntax(document, text, issue)
                                                                                                                         : syntax;

        protected abstract XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue);
    }
}