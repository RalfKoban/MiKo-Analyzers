using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2223_CodeFixProvider)), Shared]
    public sealed class MiKo_2223_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2223";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue) => syntax;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            if (syntax is XmlTextSyntax xmlText && issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.TextReplacementKey, out var replacement) && replacement != null)
            {
                var token = xmlText.FindToken(issue);

                var span = issue.Location.SourceSpan;
                var offset = token.SpanStart;

                var valueText = token.ValueText;
                var partBefore = valueText.Substring(0, span.Start - offset);
                var partAfter = valueText.Substring(span.End - offset);

                // we need to split at the specific token and place all other tokens into the new XML text
                var textTokens = xmlText.TextTokens;
                var tokenIndex = textTokens.IndexOf(token);

                var firstPart = textTokens.Take(tokenIndex).ToTokenList().Add(token.WithText(partBefore));

                var updatedXmlText = xmlText.WithTextTokens(firstPart);
                var seeCref = SeeCref(SyntaxFactory.ParseName(replacement));

                var newNodes = new List<SyntaxNode> { updatedXmlText, seeCref };

                if (partAfter.Length is 0)
                {
                    var lastPart = textTokens.Skip(tokenIndex + 1).ToTokenList();

                    if (lastPart.Count > 0)
                    {
                        newNodes.Add(XmlText(lastPart));
                    }
                }
                else
                {
                    var lastPart = textTokens.Replace(token, partAfter.AsToken()) // replace the original text token with a completely new text token, to avoid any preceding '///' characters
                                             .Skip(tokenIndex)
                                             .ToTokenList();

                    newNodes.Add(XmlText(lastPart));
                }

                return root.ReplaceNode(xmlText, newNodes);
            }

            return base.GetUpdatedSyntaxRoot(document, root, syntax, annotationOfSyntax, issue);
        }
    }
}