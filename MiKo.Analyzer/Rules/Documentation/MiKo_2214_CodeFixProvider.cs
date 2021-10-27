﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2214_CodeFixProvider)), Shared]
    public sealed class MiKo_2214_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2214_DocumentationContainsEmptyLinesAnalyzer.Id;

        protected override string Title => Resources.MiKo_2214_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var textSyntaxes = syntax.DescendantNodes().OfType<XmlTextSyntax>().Where(HasIssue).ToList();

            var updatedSyntax = syntax.ReplaceNodes(textSyntaxes, GetReplacements);

            return updatedSyntax;
        }

        private static bool HasIssue(XmlTextSyntax text)
        {
            var tokens = text.TextTokens;

            for (var i = 0; i < tokens.Count - 1; i++)
            {
                var token = tokens[i];
                var nextToken = tokens[i + 1];

                if (token.IsKind(SyntaxKind.XmlTextLiteralToken) && nextToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    if (token.ValueText.IsNullOrWhiteSpace())
                    {
                        // that's an issue, so add the already collected text as a new XML text, then add a <para/> tag
                        return true;
                    }
                }
            }

            return false;
        }

        private static IEnumerable<SyntaxNode> GetReplacements(XmlTextSyntax text)
        {
            var tokens = text.TextTokens;

            var replacements = new List<SyntaxNode>();

            var tokensForTexts = new List<SyntaxToken>();

            int i;
            var tokensCount = tokens.Count;

            var noXmlTagOnCommentStart = false;

            for (i = 0; i < tokensCount - 1; i++)
            {
                var token = tokens[i];
                var nextToken = tokens[i + 1];

                tokensForTexts.Add(token);

                if (token.IsKind(SyntaxKind.XmlTextLiteralToken) && nextToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken) && token.ValueText.IsNullOrWhiteSpace())
                {
                    tokensForTexts.Remove(token);

                    if (i == 0)
                    {
                        // skip first line as that shall not be replaced with a <para/> tag
                        // (this happens in case the comment does not start with any XML tag)
                        noXmlTagOnCommentStart = true;
                    }
                    else if (i == 1)
                    {
                        // skip second line as that shall not be replaced with a <para/> tag
                        // (this is the next line eg. after a <summary> tag)
                    }
                    else if (i == tokensCount - 3)
                    {
                        // skip second last empty line as that shall not be replaced with a <para/> tag
                        // (this is the line immediately before eg. a </summary> tag)
                        var last = tokensForTexts.Last();
                        if (last.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                        {
                            tokensForTexts.Remove(last);
                        }

                        replacements.Add(XmlText(tokensForTexts));
                    }
                    else if (i == tokensCount - 2 && noXmlTagOnCommentStart)
                    {
                        // skip last empty line as that shall not be replaced with a <para/> tag
                        // (this is the last line if the comment does not start with any XML tag)
                        var syntaxToken = tokensForTexts.Last();
                        if (syntaxToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                        {
                            tokensForTexts.Remove(syntaxToken);
                        }

                        replacements.Add(XmlText(tokensForTexts));
                    }
                    else
                    {
                        var consecutiveLines = false;

                        // skip first new line of a normal text as that would remain and lead to an additional empty line
                        var syntaxToken = noXmlTagOnCommentStart
                                              ? tokensForTexts.First()
                                              : tokensForTexts.Last();

                        if (syntaxToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                        {
                            if (noXmlTagOnCommentStart && i == 4)
                            {
                                tokensForTexts.Remove(syntaxToken);
                            }

                            if (tokensForTexts.Count <= 1)
                            {
                                // consecutive <para> tags
                                consecutiveLines = true;
                            }
                        }

                        if (consecutiveLines is false)
                        {
                            // that's an issue, so add the already collected text as a new XML text
                            replacements.Add(XmlText(tokensForTexts));

                            // now add a <para/> tag
                            replacements.Add(Para().WithLeadingXmlCommentExterior());
                        }
                    }

                    tokensForTexts.Clear();
                }
            }

            // add remaining tokens
            for (; i < tokensCount; i++)
            {
                tokensForTexts.Add(tokens[i]);
            }

            if (tokensForTexts.Any())
            {
                replacements.Add(XmlText(tokensForTexts));
            }

            // get rid of all texts that do not have any contents as they would cause a NullReferenceException inside Roslyn
            replacements.RemoveAll(_ => _ is XmlTextSyntax t && t.TextTokens.Count == 0);

            if (replacements.Count == 0)
            {
                // nothing to replace
                return new[] { text };
            }

            // TODO RKN Remove duplicate <para> tags
            return replacements;
        }
    }
}