using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2035_CodeFixProvider)), Shared]
    public sealed class MiKo_2035_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        private static readonly HashSet<string> WellKnownTypeNames = new HashSet<string>
                                                                         {
                                                                             "string",
                                                                             nameof(String),
                                                                             "short",
                                                                             nameof(Int16),
                                                                             "int",
                                                                             nameof(Int32),
                                                                             "long",
                                                                             nameof(Int64),
                                                                             "ushort",
                                                                             nameof(UInt16),
                                                                             "uint",
                                                                             nameof(UInt32),
                                                                             "ulong",
                                                                             nameof(UInt64),
                                                                             "bool",
                                                                             nameof(Boolean),
                                                                             "float",
                                                                             nameof(Single),
                                                                             "double",
                                                                             nameof(Double),
                                                                             "object",
                                                                             nameof(Object),
                                                                             "decimal",
                                                                             nameof(Decimal),
                                                                             "byte",
                                                                             nameof(Byte),
                                                                             "sbyte",
                                                                             nameof(SByte),
                                                                         };

        private static readonly Lazy<MapData> MappedData = new Lazy<MapData>();

        private static readonly string[] TaskParts = Constants.Comments.GenericTaskReturnTypeStartingPhraseTemplate.FormatWith("task", '|').Split('|');

#if !NCRUNCH // do not define a static ctor to speed up tests in NCrunch
        static MiKo_2035_CodeFixProvider() => LoadData(); // ensure that we have the object available
#endif

        public override string FixableDiagnosticId => "MiKo_2035";

        public static void LoadData() => GC.KeepAlive(MappedData.Value);

        protected override XmlElementSyntax GenericComment(Document document, XmlElementSyntax comment, string memberName, GenericNameSyntax returnType)
        {
            var preparedComment = PrepareComment(comment);

            // it's either a task or a generic collection
            var returnTypeValue = returnType.Identifier.ValueText;

            if (returnTypeValue == nameof(Task))
            {
                // it is a task, so inspect the typ argument to check if it is an array type
                var middlePart = GetGenericCommentMiddlePart(returnType);

                return CommentStartingWith(preparedComment, TaskParts[0], SeeCrefTaskResult(), TaskParts[1] + middlePart);
            }

            var startingPhrase = Constants.Comments.CollectionReturnTypeStartingPhrase;

            if (returnType.TypeArgumentList.Arguments.Count == 1)
            {
                var typeName = GetGenericTypeArgumentTypeName(returnType);

                if (WellKnownTypeNames.Contains(typeName) is false)
                {
                    var text = typeName.AsBuilder().AdjustFirstWord(FirstWordHandling.MakePlural).SeparateWords(' ', FirstWordHandling.MakeLowerCase).ToString();

                    if (text.Length > 0)
                    {
                        startingPhrase = startingPhrase + text + " " + Constants.Comments.ThatContainsTerm + " ";
                    }
                }
            }

            return CommentStartingWith(preparedComment, startingPhrase);
        }

        protected override XmlElementSyntax NonGenericComment(Document document, XmlElementSyntax comment, string memberName, TypeSyntax returnType)
        {
            if (returnType is ArrayTypeSyntax arrayType)
            {
                if (arrayType.ElementType.IsByte())
                {
                    return CommentStartingWith(PrepareByteArrayComment(comment), Constants.Comments.ByteArrayReturnTypeStartingPhrase);
                }

                return CommentStartingWith(PrepareComment(comment), Constants.Comments.ArrayReturnTypeStartingPhrase);
            }

            return CommentStartingWith(PrepareComment(comment), Constants.Comments.CollectionReturnTypeStartingPhrase);
        }

        private static string GetGenericTypeArgumentTypeName(GenericNameSyntax returnType)
        {
            var argument = returnType.TypeArgumentList.Arguments[0];

            return argument is ArrayTypeSyntax arrayType
                   ? arrayType.ElementType.GetName()
                   : argument.GetName();
        }

        private static string GetGenericCommentMiddlePart(GenericNameSyntax returnType)
        {
            var argument = returnType.TypeArgumentList.Arguments[0];

            if (argument is ArrayTypeSyntax arrayType)
            {
                return arrayType.ElementType.IsByte()
                       ? Constants.Comments.ByteArrayReturnTypeStartingPhraseALowerCase
                       : Constants.Comments.ArrayReturnTypeStartingPhraseALowerCase;
            }

            return Constants.Comments.CollectionReturnTypeStartingPhraseLowerCase;
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment)
        {
            var adjustedComment = CommentWithContent(comment, comment.Content);

            return Comment(adjustedComment, MappedData.Value.ReplacementMapKeys, MappedData.Value.ReplacementMap);
        }

        private static XmlElementSyntax PrepareByteArrayComment(XmlElementSyntax comment)
        {
            var preparedComment = Comment(comment, MappedData.Value.ByteArrayReplacementMapKeys, MappedData.Value.ByteArrayReplacementMap);

            var contents = preparedComment.Content;
            var count = contents.Count;

            if (count > 1 && IsSeeCref(contents[1], "byte") && contents[0].IsWhiteSpaceOnlyText())
            {
                // inspect the continue text and - if necessary - clean it up
                if (count > 2 && contents[2] is XmlTextSyntax continueText)
                {
                    var token = continueText.TextTokens.First();
                    var text = token.ValueText;

                    if (text.StartsWithAny(MappedData.Value.ByteArrayContinueTexts))
                    {
                        var newContinueText = continueText.ReplaceToken(token, token.WithText(text.AsBuilder().Without(MappedData.Value.ByteArrayContinueTexts)));

                        preparedComment = preparedComment.ReplaceNode(continueText, newContinueText);
                    }
                }

                // remove the <see cref="byte"/> element
                preparedComment = preparedComment.Without(preparedComment.Content[1]);
            }

            return preparedComment;
        }

//// ncrunch: rdi off

        private sealed class MapData
        {
            public MapData()
            {
                var taskPhrases = new[]
                                      {
                                          "A task that can be used to await.",
                                          "A task that can be used to await",
                                          "A task to await.",
                                          "A task to await",
                                          "An awaitable task.",
                                          "An awaitable task",
                                      };

                var phrases = CreatePhrases().ToHashSet(_ => _ + " "); // TODO RKN: Order by 'A', 'An ' and 'The '

                ReplacementMap = AlmostCorrectTaskReturnTypeStartingPhrases.Concat(taskPhrases)
                                                                           .Concat(phrases)
                                                                           .OrderByDescending(_ => _.Length)
                                                                           .ThenBy(_ => _)
                                                                           .ToArray(_ => new Pair(_, string.Empty));

                ReplacementMapKeys = GetTermsForQuickLookup(ReplacementMap.ToArray(_ => _.Key));

                ByteArrayReplacementMap = AlmostCorrectTaskReturnTypeStartingPhrases.Concat(new[]
                                                                                                {
                                                                                                    "A array of byte containing ",
                                                                                                    "A array of byte that contains ",
                                                                                                    "A array of byte which contains ",
                                                                                                    "A array of bytes containing ",
                                                                                                    "A array of bytes that contains ",
                                                                                                    "A array of bytes which contains ",
                                                                                                    "A array of ",
                                                                                                    "A array with ",
                                                                                                    "An array of byte containing ",
                                                                                                    "An array of byte that contains ",
                                                                                                    "An array of byte which contains ",
                                                                                                    "An array of bytes containing ",
                                                                                                    "An array of bytes that contains ",
                                                                                                    "An array of bytes which contains ",
                                                                                                    "An array of ",
                                                                                                    "An array with ",
                                                                                                    "Array of ",
                                                                                                    "Array with ",
                                                                                                    "The array of byte containing ",
                                                                                                    "The array of byte that contains ",
                                                                                                    "The array of byte which contains ",
                                                                                                    "The array of bytes containing ",
                                                                                                    "The array of bytes that contains ",
                                                                                                    "The array of bytes which contains ",
                                                                                                    "The array of ",
                                                                                                    "The array with ",
                                                                                                })
                                                                                    .OrderByDescending(_ => _.Length)
                                                                                    .ThenBy(_ => _)
                                                                                    .ToArray(_ => new Pair(_, string.Empty));

                ByteArrayReplacementMapKeys = GetTermsForQuickLookup(ByteArrayReplacementMap.ToArray(_ => _.Key));

                ByteArrayContinueTexts = new[]
                                             {
                                                 "s containing ",
                                                 "s that contains ",
                                                 "s which contains ",
                                                 " containing ",
                                                 " that contains ",
                                                 " which contains ",
                                             };
            }

            public Pair[] ReplacementMap { get; }

            public string[] ReplacementMapKeys { get; }

            public Pair[] ByteArrayReplacementMap { get; }

            public string[] ByteArrayReplacementMapKeys { get; }

            public string[] ByteArrayContinueTexts { get; }

            private static IEnumerable<string> CreatePhrases()
            {
                var startingWords = new[] { "a", "an", "the" };
                var modifications = new[] { "readonly", "read-only", "read only" };
                var collections = new[] { "array", "list", "dictionary", "enumerable", "hash set", "hash table", "hashed set", "hashed table", "hashing set", "hashing table", "hashset", "hashSet", "hashtable", "hashTable", "map", "queue", "stack" };
                var prepositions = new[] { "of", "with", "that contains", "which contains", "that holds", "which holds", "containing", "holding" };

                foreach (var collection in collections)
                {
                    foreach (var preposition in prepositions)
                    {
                        var phrase = string.Concat(collection, " ", preposition);

                        yield return phrase.ToUpperCaseAt(0);
                        yield return phrase.ToLowerCaseAt(0);

                        foreach (var modification in modifications)
                        {
                            var modificationPhrase = string.Concat(modification, " ", phrase);

                            yield return modificationPhrase.ToUpperCaseAt(0);
                            yield return modificationPhrase.ToLowerCaseAt(0);

                            foreach (var startingWord in startingWords)
                            {
                                var shortStartingPhrase = string.Concat(startingWord, " ", collection);
                                var startingPhrase = string.Concat(startingWord, " ", phrase);
                                var modifiedStartingPhrase = string.Concat(startingWord, " ", modificationPhrase);

                                yield return shortStartingPhrase.ToUpperCaseAt(0);
                                yield return shortStartingPhrase.ToLowerCaseAt(0);

                                yield return startingPhrase.ToUpperCaseAt(0);
                                yield return startingPhrase.ToLowerCaseAt(0);

                                yield return modifiedStartingPhrase.ToUpperCaseAt(0);
                                yield return modifiedStartingPhrase.ToLowerCaseAt(0);
                            }
                        }
                    }
                }
            }
        }
    }

    //// ncrunch: rdi default
}