using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2035_CodeFixProvider)), Shared]
    public sealed class MiKo_2035_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> ReplacementMap = new Dictionary<string, string>
                                                                                {
                                                                                    { "An enumerable of ", string.Empty },
                                                                                    { "An enumerable with ", string.Empty },
                                                                                    { "A list of ", string.Empty },
                                                                                    { "A list with ", string.Empty },
                                                                                    { "A read-only collection of ", string.Empty },
                                                                                    { "A read-only collection with ", string.Empty },
                                                                                    { "A readonly collection of ", string.Empty },
                                                                                    { "A readonly collection with ", string.Empty },
                                                                                    { "The enumerable of ", string.Empty },
                                                                                    { "The enumerable with ", string.Empty },
                                                                                    { "The list of ", string.Empty },
                                                                                    { "The list with ", string.Empty },
                                                                                    { "The array of ", string.Empty },
                                                                                    { "The array with ", string.Empty },
                                                                                    { "The collection of ", string.Empty },
                                                                                    { "The collection with ", string.Empty },
                                                                                    { "The read-only collection of ", string.Empty },
                                                                                    { "The read-only collection with ", string.Empty },
                                                                                    { "The readonly collection of ", string.Empty },
                                                                                    { "The readonly collection with ", string.Empty },
                                                                                };

        private static readonly Dictionary<string, string> ByteArrayReplacementMap = new Dictionary<string, string>
                                                                                         {
                                                                                             { "An array of bytes which contains ", string.Empty },
                                                                                             { "An array of bytes that contains ", string.Empty },
                                                                                             { "An array of bytes containing ", string.Empty },
                                                                                             { "An array of byte which contains ", string.Empty },
                                                                                             { "An array of byte that contains ", string.Empty },
                                                                                             { "An array of byte containing ", string.Empty },
                                                                                             { "The array of bytes which contains ", string.Empty },
                                                                                             { "The array of bytes that contains ", string.Empty },
                                                                                             { "The array of bytes containing ", string.Empty },
                                                                                             { "The array of byte which contains ", string.Empty },
                                                                                             { "The array of byte that contains ", string.Empty },
                                                                                             { "The array of byte containing ", string.Empty },
                                                                                             { "An array of ", string.Empty },
                                                                                             { "The array of ", string.Empty },
                                                                                         };

        private static readonly string[] ByteArrayContinueTexts =
            {
                "s containing ",
                "s that contains ",
                "s which contains ",
                " containing ",
                " that contains ",
                " which contains ",
            };

        private static readonly string[] TaskParts = string.Format(Constants.Comments.GenericTaskReturnTypeStartingPhraseTemplate, "task", '|').Split('|');

        public override string FixableDiagnosticId => MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2035_CodeFixTitle;

        protected override XmlElementSyntax GenericComment(Document document, XmlElementSyntax comment, GenericNameSyntax returnType)
        {
            var preparedComment = PrepareComment(comment);

            // it's either a task or a generic collection
            if (returnType.Identifier.ValueText == nameof(Task))
            {
                // it is a task, so inspect the typ argument to check if it is an array type
                var middlePart = GetGenericCommentMiddlePart(returnType);

                return CommentStartingWith(preparedComment, TaskParts[0], SeeCrefTaskResult(), TaskParts[1] + middlePart);
            }

            return CommentStartingWith(preparedComment, Constants.Comments.EnumerableReturnTypeStartingPhrase);
        }

        protected override XmlElementSyntax NonGenericComment(Document document, XmlElementSyntax comment, TypeSyntax returnType)
        {
            if (returnType is ArrayTypeSyntax arrayType)
            {
                if (arrayType.ElementType.IsByte())
                {
                    return CommentStartingWith(PrepareByteArrayComment(comment), Constants.Comments.ByteArrayReturnTypeStartingPhrase);
                }

                return CommentStartingWith(PrepareComment(comment), Constants.Comments.ArrayReturnTypeStartingPhrase);
            }

            return CommentStartingWith(PrepareComment(comment), Constants.Comments.EnumerableReturnTypeStartingPhrase);
        }

        private static string GetGenericCommentMiddlePart(GenericNameSyntax returnType)
        {
            var argument = returnType.TypeArgumentList.Arguments[0];

            if (argument is ArrayTypeSyntax arrayType)
            {
                return arrayType.ElementType.IsByte()
                           ? "a byte array containing "
                           : "an array of ";
            }

            return "a collection of ";
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment) => Comment(comment, ReplacementMap.Keys, ReplacementMap);

        private static XmlElementSyntax PrepareByteArrayComment(XmlElementSyntax comment)
        {
            var preparedComment = Comment(comment, ByteArrayReplacementMap.Keys, ByteArrayReplacementMap);

            var contents = preparedComment.Content;

            if (contents.Count > 1 && IsSeeCref(contents[1], "byte") && contents[0] is XmlTextSyntax startText && IsWhiteSpaceOnlyText(startText))
            {
                // inspect the continue text and - if necessary - clean it up
                if (contents.Count > 2 && contents[2] is XmlTextSyntax continueText)
                {
                    var token = continueText.TextTokens.First();
                    var text = token.ValueText;

                    if (text.StartsWithAny(ByteArrayContinueTexts))
                    {
                        var newContinueText = continueText.ReplaceToken(token, token.WithText(text.Without(ByteArrayContinueTexts)));

                        preparedComment = preparedComment.ReplaceNode(continueText, newContinueText);
                    }
                }

                // remove the <see cref="byte"/> element
                preparedComment = preparedComment.Without(preparedComment.Content[1]);
            }

            return preparedComment;
        }
    }
}