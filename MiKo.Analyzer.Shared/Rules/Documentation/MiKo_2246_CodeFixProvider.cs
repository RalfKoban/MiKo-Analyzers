using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2246_CodeFixProvider)), Shared]
    public sealed class MiKo_2246_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2246";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue)
        {
            var spaces = GetProposedSpaces(issue);

            var token = syntax.FindToken(issue);
            var text = token.ValueText.AsSpan(spaces);

            if (text.Trim().IsEmpty)
            {
                switch (syntax.NextSibling())
                {
                    case XmlEmptyElementSyntax _:
                    case XmlElementSyntax _:
                        break; // seems we have an XML element as next element, so only shorten text

                    default:
                    {
                        var textTokens = syntax.TextTokens;

                        if (textTokens.Count is 1)
                        {
                            return null; // seems it is the only one, so remove the complete text
                        }

                        return syntax.WithTextTokens(textTokens.Remove(token));
                    }
                }
            }

            return syntax.ReplaceToken(token, token.WithText(text));
        }
    }
}