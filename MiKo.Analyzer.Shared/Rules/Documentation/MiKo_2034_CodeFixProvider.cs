using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2034_CodeFixProvider)), Shared]
    public sealed class MiKo_2034_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        private static readonly string[] Parts = Constants.Comments.EnumTaskReturnTypeStartingPhraseTemplate.FormatWith("task", "|").Split('|');

        public override string FixableDiagnosticId => MiKo_2034_EnumReturnTypeDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2034_CodeFixTitle;

        protected override XmlElementSyntax GenericComment(Document document, XmlElementSyntax comment, GenericNameSyntax returnType)
        {
            return Comment(comment, Parts[0], SeeCrefTaskResult(), Parts[1], RemoveStartingWord(comment));
        }

        protected override XmlElementSyntax NonGenericComment(Document document, XmlElementSyntax comment, TypeSyntax returnType)
        {
            return Comment(comment, Constants.Comments.EnumReturnTypeStartingPhrase, RemoveStartingWord(comment));
        }

        private static SyntaxList<XmlNodeSyntax> RemoveStartingWord(XmlElementSyntax comment) => RemoveStartingWord(comment.WithoutFirstXmlNewLine(), Constants.Comments.ParameterStartingCodefixPhrases);

        private static SyntaxList<XmlNodeSyntax> RemoveStartingWord(XmlElementSyntax comment, params string[] words)
        {
            var contents = comment.Content;

            if (contents.First() is XmlTextSyntax t)
            {
                var token = t.TextTokens.First();

                var text = token.ValueText.WithoutFirstWords(words);

                return comment.ReplaceToken(token, token.WithText(text)).Content;
            }

            return contents;
        }
    }
}