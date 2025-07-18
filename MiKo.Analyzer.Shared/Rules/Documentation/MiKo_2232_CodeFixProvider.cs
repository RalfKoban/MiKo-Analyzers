﻿using System;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2232_CodeFixProvider)), Shared]
    public sealed class MiKo_2232_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2232";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic issue)
        {
            var node = syntax.FindNode(issue.Location.SourceSpan, true, true);

            if (node is XmlElementSyntax element && element.Parent == syntax)
            {
                // we are directly within the DocumentationCommentTriviaSyntax
                var updatedContent = GetUpdatedXmlContent(syntax.Content, element);

                if (updatedContent.Count is 0)
                {
                    return null;
                }

                return syntax.WithContent(updatedContent.WithoutFirstXmlNewLine().WithLeadingXmlComment().WithIndentation());
            }

            return syntax;
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            SyntaxToken token;

            if (syntax is DocumentationCommentTriviaSyntax comment)
            {
                token = comment.ParentTrivia.Token;
            }
            else
            {
                // XML comment should be removed here, so adjust empty line on token
                // (we need to be aware of attributes, so we have to find the node first to get the attribute and it's first token)
                var node = root.FindNode(issue.Location.SourceSpan);

                token = node.FirstDescendantToken();
            }

            var updatedToken = token.WithLeadingEmptyLine();

            var updatedRoot = root.ReplaceToken(token, updatedToken);

            return updatedRoot;
        }

        private static SyntaxList<XmlNodeSyntax> GetUpdatedXmlContent(in SyntaxList<XmlNodeSyntax> originalContent, XmlElementSyntax issue)
        {
            var content = originalContent.ToList();
            var index = content.IndexOf(issue);

            if (index > 0)
            {
                if (content.Count > index + 2)
                {
                    if (content[index + 2] is XmlTextSyntax)
                    {
                        RemoveEmptyText(index + 1);
                    }
                }
                else
                {
                    RemoveEmptyText(index + 1);
                }

                for (var i = index - 1; i >= 0; i--)
                {
                    RemoveEmptyText(i);
                }

                content.Remove(issue);
            }

            return SyntaxFactory.List(content);

            void RemoveEmptyText(int i)
            {
                if (content.Count > i && content[i].IsWhiteSpaceOnlyText())
                {
                    content.RemoveAt(i);
                }
            }
        }
    }
}