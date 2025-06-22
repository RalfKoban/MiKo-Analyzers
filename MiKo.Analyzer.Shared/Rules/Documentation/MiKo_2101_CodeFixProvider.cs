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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2101_CodeFixProvider)), Shared]
    public sealed class MiKo_2101_CodeFixProvider : ExampleDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2101";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var example = (XmlElementSyntax)syntax;

            var list = new List<XmlTextSyntax>();

            var contents = example.Content;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            for (int index = 0, count = contents.Count; index < count; index++)
            {
                var node = contents[index];

                if (node.IsCode())
                {
                    // ignore code blocks
                    continue;
                }

                if (node is XmlTextSyntax text)
                {
                    list.Add(text);
                }
            }

            return example.ReplaceNodes(list, GetReplacements);
        }

        private static IEnumerable<SyntaxNode> GetReplacements(XmlTextSyntax text)
        {
            var result = new List<SyntaxNode>();

            var commentedOutCode = new List<SyntaxToken>();

            var normalText = new List<SyntaxToken>();

            // we have some text, so let's see if this is some commented out code
            var textTokens = text.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            for (int index = 0, textTokensCount = textTokens.Count; index < textTokensCount; index++)
            {
                var token = textTokens[index];

                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    // skip empty lines in commented out code, but keep normal lines
                    if (commentedOutCode.Count is 0)
                    {
                        normalText.Add(token);
                    }

                    continue;
                }

                var valueText = token.ValueText.AsSpan().Trim();

                if (CodeDetector.IsCommentedOutCodeLine(valueText))
                {
                    // we already have some text
                    AddXmlText(result, normalText);

                    normalText.Clear();

                    commentedOutCode.Add(token.WithText(valueText).WithLeadingXmlComment());
                }
                else
                {
                    // we found some code
                    AddCode(result, commentedOutCode);

                    if (valueText.IsNullOrWhiteSpace() is false)
                    {
                        // we found some text
                        normalText.Add(token);
                    }

                    commentedOutCode.Clear();
                }
            }

            // we found some code at the end
            AddCode(result, commentedOutCode);

            // we found some normal code at the end
            AddXmlText(result, normalText);

            if (result.Count is 0)
            {
                // nothing to replace, so use original code
                result.Add(text);
            }

            result[result.Count - 1] = result[result.Count - 1].WithTrailingXmlComment();

            return result;
        }

        private static void AddXmlText(List<SyntaxNode> result, List<SyntaxToken> text)
        {
            if (text.Any(_ => _.ValueText.IsNullOrWhiteSpace() is false))
            {
                // remove last new line token so that we do not have empty lines
                var last = text.Last();

                if (last.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    text.Remove(last);
                }

                if (text.Count > 0)
                {
                    result.Add(XmlText(text));
                }
            }
        }

        private static void AddCode(List<SyntaxNode> result, List<SyntaxToken> commentedOutCode)
        {
            if (commentedOutCode.Count > 0)
            {
                // we found some code at the end
                var code = GetAsCode(commentedOutCode);

                result.Add(code);
            }
        }

        private static XmlElementSyntax GetAsCode(IEnumerable<SyntaxToken> commentedOutCode)
        {
            var comment = XmlText(commentedOutCode).WithTrailingXmlComment();

            return XmlElement(Constants.XmlTag.Code, comment).WithLeadingXmlComment()
                                                             .WithTrailingNewLine();
        }
    }
}