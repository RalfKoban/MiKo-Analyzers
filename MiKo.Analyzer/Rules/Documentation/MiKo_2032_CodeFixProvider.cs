﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2032_CodeFixProvider)), Shared]
    public sealed class MiKo_2032_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        private static readonly string[] NonGenericStartParts = string.Format(Constants.Comments.BooleanReturnTypeStartingPhraseTemplate, "|").Split('|');
        private static readonly string[] NonGenericEndParts = string.Format(Constants.Comments.BooleanReturnTypeEndingPhraseTemplate, "|").Split('|');
        private static readonly string[] GenericStartParts = string.Format(Constants.Comments.BooleanTaskReturnTypeStartingPhraseTemplate, "|").Split('|');
        private static readonly string[] GenericEndParts = string.Format(Constants.Comments.BooleanTaskReturnTypeEndingPhraseTemplate, "|").Split('|');
        private static readonly char[] TrailingSentenceMarkers = " .;:,?\t".ToCharArray();

        public override string FixableDiagnosticId => MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2032_CodeFixTitle;

        protected override SyntaxNode GenericComment(XmlElementSyntax comment, GenericNameSyntax returnType)
        {
            var middlePart = CreateMiddlePart(comment, GenericStartParts[1], GenericEndParts[0]);

            return Comment(comment, GenericStartParts[0], SeeLangword_True(), middlePart, SeeLangword_False(), GenericEndParts[1]);
        }

        protected override XmlElementSyntax NonGenericComment(XmlElementSyntax comment, TypeSyntax returnType)
        {
            var middlePart = CreateMiddlePart(comment, NonGenericStartParts[1], NonGenericEndParts[0]);

            return Comment(comment, NonGenericStartParts[0], SeeLangword_True(), middlePart, SeeLangword_False(), NonGenericEndParts[1]);
        }

        private static SyntaxList<XmlNodeSyntax> CreateMiddlePart(XmlElementSyntax comment, string startingPhrase, string endingPhrase)
        {
            // add starting text and ensure that first character of original text is now lower-case
            var nodes = comment.WithStartText(startingPhrase);

            if (nodes.Last() is XmlTextSyntax text)
            {
                // get rid of endings of . ; : , or ?
                nodes = nodes.Replace(text, text.WithoutTrailingCharacters(TrailingSentenceMarkers));
            }

            return nodes.Add(SyntaxFactory.XmlText(endingPhrase));
        }
    }
}