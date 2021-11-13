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

        private static readonly string[] TaskParts = string.Format(Constants.Comments.GenericTaskReturnTypeStartingPhraseTemplate, "task", '|').Split('|');

        public override string FixableDiagnosticId => MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2035_CodeFixTitle;

        protected override XmlElementSyntax GenericComment(XmlElementSyntax comment, GenericNameSyntax returnType)
        {
            var preparedComment = PrepareComment(comment);

            // it's either a task or a generic collection
            if (returnType.Identifier.ValueText == nameof(Task))
            {
                // it is a task, so inspect the typ argument to check if it is an array type
                var middlePart = GetGenericCommentMiddlePart(returnType);

                return CommentStartingWith(preparedComment, TaskParts[0], SeeCrefTaskResult(), TaskParts[1] + middlePart);
            }

            return CommentStartingWith(preparedComment, Constants.Comments.EnumerableReturnTypeStartingPhrase[0]);
        }

        protected override XmlElementSyntax NonGenericComment(XmlElementSyntax comment, TypeSyntax returnType)
        {
            var phrases = GetNonGenericCommentPhrases(returnType);
            var preparedComment = PrepareComment(comment);

            return CommentStartingWith(preparedComment, phrases[0]);
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

        private static string[] GetNonGenericCommentPhrases(TypeSyntax returnType)
        {
            if (returnType is ArrayTypeSyntax arrayType)
            {
                return arrayType.ElementType.IsByte()
                              ? Constants.Comments.ByteArrayReturnTypeStartingPhrase
                              : Constants.Comments.ArrayReturnTypeStartingPhrase;
            }

            return Constants.Comments.EnumerableReturnTypeStartingPhrase;
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment) => Comment(comment, ReplacementMap.Keys, ReplacementMap);
    }
}