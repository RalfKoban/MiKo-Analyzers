using System;
using System.Composition;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2034_CodeFixProvider)), Shared]
    public sealed class MiKo_2034_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        private static readonly string[] Parts = Constants.Comments.EnumTaskReturnTypeStartingPhraseTemplate.FormatWith("task", "|").Split('|');

        private static readonly string[] UnwantedResultTexts = { ", the result will be", ", the result is" };

        public override string FixableDiagnosticId => "MiKo_2034";

        protected override XmlElementSyntax GenericComment(Document document, XmlElementSyntax comment, string memberName, GenericNameSyntax returnType)
        {
            var commentStart = Parts[0];
            var commentEnd = Parts[1];

            if (TryUpdateSpecialPhrase(comment, out var updatedComment))
            {
                return Comment(updatedComment, commentStart, SeeCrefTaskResult(), commentEnd.WithoutSuffix("the "), RemoveStartingWord(updatedComment));
            }

            return Comment(comment, commentStart, SeeCrefTaskResult(), commentEnd, RemoveStartingWord(comment));
        }

        protected override XmlElementSyntax NonGenericComment(Document document, XmlElementSyntax comment, string memberName, TypeSyntax returnType)
        {
            var text = Constants.Comments.EnumReturnTypeStartingPhrase[0];

            if (TryUpdateSpecialPhrase(comment, out var updatedComment))
            {
                return Comment(updatedComment, text.WithoutSuffix("the "), RemoveStartingWord(updatedComment));
            }

            return Comment(comment, text, RemoveStartingWord(comment));
        }

        private static bool TryUpdateSpecialPhrase(XmlElementSyntax comment, out XmlElementSyntax updatedSyntax)
        {
            updatedSyntax = null;

            var contents = comment.Content;

            if (contents.First() is XmlTextSyntax t)
            {
                var text = t.GetTextWithoutTrivia();

                if (text.StartsWith("If ", StringComparison.OrdinalIgnoreCase) && text.ContainsAny(UnwantedResultTexts))
                {
                    // get rid of the unwanted phrases and switch text parts
                    var parts = text.SplitBy(UnwantedResultTexts, options: StringSplitOptions.RemoveEmptyEntries);
                    var switchedText = parts.ConcatenatedWith(" ").AdjustFirstWord(FirstWordHandling.MakeLowerCase);
                    var startText = switchedText.AsBuilder().Insert(0, ' ').TrimEnd();

                    var updatedContents = contents.Remove(t);

                    if (updatedContents.Count <= 1)
                    {
                        updatedContents = updatedContents.Add(XmlText(startText));
                    }
                    else
                    {
                        if (updatedContents[1] is XmlTextSyntax t1)
                        {
                            updatedContents = updatedContents.Replace(t1, t1.WithStartText(startText));
                        }
                        else
                        {
                            // insert at position 1, so that it is positioned after the <see> that's probably there
                            updatedContents = updatedContents.Insert(1, XmlText(startText));
                        }
                    }

                    updatedSyntax = comment.WithContent(updatedContents);

                    return true;
                }
            }

            return false;
        }

        private static SyntaxList<XmlNodeSyntax> RemoveStartingWord(XmlElementSyntax comment) => RemoveStartingWord(comment.WithoutFirstXmlNewLine(), Constants.Comments.ParameterStartingCodefixPhrase);

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