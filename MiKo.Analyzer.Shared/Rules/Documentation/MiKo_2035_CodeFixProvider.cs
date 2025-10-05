using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
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

        private static readonly string[] TaskParts = Constants.Comments.GenericTaskReturnTypeStartingPhraseTemplate.FormatWith("task", "|").Split('|');

        public override string FixableDiagnosticId => "MiKo_2035";

        public static void LoadData() => GC.KeepAlive(MappedData.Value);

        protected override XmlElementSyntax GenericComment(Document document, XmlElementSyntax comment, string memberName, GenericNameSyntax returnType)
        {
            var preparedComment = PrepareComment(comment);

            var updatedComment = GenericComment(preparedComment, returnType);

            return CleanupComment(updatedComment);
        }

        protected override XmlElementSyntax NonGenericComment(Document document, XmlElementSyntax comment, string memberName, TypeSyntax returnType)
        {
            var preparedComment = returnType is ArrayTypeSyntax arrayType && arrayType.ElementType.IsByte()
                                  ? PrepareByteArrayComment(comment)
                                  : PrepareComment(comment);

            var startingPhrase = GetNonGenericMiddlePart(returnType);

            var updatedComment = CommentStartingWith(preparedComment, startingPhrase);

            return CleanupComment(updatedComment);
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment)
        {
            var adjustedComment = CommentWithContent(comment, comment.Content);

            adjustedComment = Comment(adjustedComment, MappedData.Value.PreparationMapKeys, MappedData.Value.PreparationMap);

            return Comment(adjustedComment, MappedData.Value.ReplacementMapKeys, MappedData.Value.ReplacementMap);
        }

        private static XmlElementSyntax PrepareByteArrayComment(XmlElementSyntax comment)
        {
            var preparedComment = Comment(comment, MappedData.Value.ByteArrayReplacementMapKeys, MappedData.Value.ByteArrayReplacementMap);

            var contents = preparedComment.Content;
            var count = contents.Count;

            if (count > 1 && contents[1].IsSeeCref("byte") && contents[0].IsWhiteSpaceOnlyText())
            {
                // inspect the continue text and - if necessary - clean it up
                if (count > 2 && contents[2] is XmlTextSyntax continueText)
                {
                    var token = continueText.TextTokens.First();
                    var text = token.ValueText;

                    if (text.StartsWithAny(MappedData.Value.ByteArrayContinueTexts, StringComparison.OrdinalIgnoreCase))
                    {
                        var fixedText = text.AsCachedBuilder().Without(MappedData.Value.ByteArrayContinueTexts).ToStringAndRelease();
                        var newContinueText = continueText.ReplaceToken(token, token.WithText(fixedText));

                        preparedComment = preparedComment.ReplaceNode(continueText, newContinueText);
                    }
                }

                // remove the <see cref="byte"/> element
                preparedComment = preparedComment.Without(preparedComment.Content[1]);
            }

            return preparedComment;
        }

        private static XmlElementSyntax CleanupComment(XmlElementSyntax comment)
        {
            return Comment(comment, MappedData.Value.CleanupMapKeys, MappedData.Value.CleanupMap);
        }

        private static XmlElementSyntax GenericComment(XmlElementSyntax preparedComment, GenericNameSyntax returnType)
        {
            // it's either a task or a generic collection
            var returnTypeValue = returnType.Identifier.ValueText;

            if (returnTypeValue == nameof(Task))
            {
                // it is a task, so inspect the typ argument to check if it is an array type
                var middlePart = GetGenericCommentMiddlePart(returnType);

                return CommentStartingWith(preparedComment, TaskParts[0], SeeCrefTaskResult(), TaskParts[1] + middlePart);
            }

            if (returnType.GetName() is "IEnumerable")
            {
                return CommentStartingWith(preparedComment, Constants.Comments.EnumerableReturnTypeStartingPhrase);
            }

            var startingPhrase = Constants.Comments.CollectionReturnTypeStartingPhrase;

            if (returnType.TypeArgumentList.Arguments.Count is 1)
            {
                var typeName = GetGenericTypeArgumentTypeName(returnType);

                if (WellKnownTypeNames.Contains(typeName) is false)
                {
                    var text = typeName.AsCachedBuilder()
                                       .AdjustFirstWord(FirstWordAdjustment.MakePlural)
                                       .SeparateWords(' ', FirstWordAdjustment.StartLowerCase)
                                       .ToStringAndRelease();

                    if (text.Length > 0)
                    {
                        startingPhrase = startingPhrase + text + " " + Constants.Comments.ThatContainsTerm + " ";
                    }
                }
            }

            return CommentStartingWith(preparedComment, startingPhrase);
        }

        private static string GetGenericTypeArgumentTypeName(GenericNameSyntax returnType)
        {
            var type = returnType.TypeArgumentList.Arguments[0];

            if (type is ArrayTypeSyntax arrayType)
            {
                type = arrayType.ElementType;
            }

            var name = type.GetName();

            if (returnType.Parent is MethodDeclarationSyntax m && m.ConstraintClauses is SyntaxList<TypeParameterConstraintClauseSyntax> clauses)
            {
                for (int index = 0, count = clauses.Count; index < count; index++)
                {
                    var clause = clauses[index];

                    if (name == clause.GetName())
                    {
                        var constraints = clause.Constraints;

                        if (constraints.OfType<TypeConstraintSyntax>().FirstOrDefault() is TypeConstraintSyntax constraint)
                        {
                            // seems we have a 'where T : xyz' clause
                            return constraint.Type.GetName();
                        }

                        if (constraints.OfType<ClassOrStructConstraintSyntax>().FirstOrDefault() is ClassOrStructConstraintSyntax classConstraint)
                        {
                            return classConstraint.IsKind(SyntaxKind.ClassConstraint)
                                   ? "object"
                                   : "value"; // it is a struct
                        }
                    }
                }
            }

            return name;
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

        private static string GetNonGenericMiddlePart(TypeSyntax returnType)
        {
            if (returnType is ArrayTypeSyntax arrayType)
            {
                return arrayType.ElementType.IsByte()
                       ? Constants.Comments.ByteArrayReturnTypeStartingPhrase[0]
                       : Constants.Comments.ArrayReturnTypeStartingPhrase[0];
            }

            return returnType.GetName() is "IEnumerable"
                   ? Constants.Comments.EnumerableReturnTypeStartingPhrase
                   : Constants.Comments.CollectionReturnTypeStartingPhrase;
        }

//// ncrunch: rdi off
//// ncrunch: no coverage start

        private sealed class MapData
        {
#pragma warning disable SA1401 // Fields should be private
            public readonly Pair[] ReplacementMap;
            public readonly string[] ReplacementMapKeys;
            public readonly Pair[] ByteArrayReplacementMap;
            public readonly string[] ByteArrayReplacementMapKeys;
            public readonly string[] ByteArrayContinueTexts;
            public readonly Pair[] PreparationMap;
            public readonly string[] PreparationMapKeys;
            public readonly Pair[] CleanupMap;
            public readonly string[] CleanupMapKeys;
#pragma warning restore SA1401 // Fields should be private

#pragma warning disable CA1861
            public MapData()
            {
                var phrases = CreatePhrases().ToHashSet(); // TODO RKN: Order by 'A', 'An ' and 'The '

                ReplacementMap = AlmostCorrectTaskReturnTypeStartingPhrases.ConcatenatedWith("A task that can be used to await.", "A task that can be used to await", "A task to await.", "A task to await", "An awaitable task.", "An awaitable task")
                                                                           .Concat(phrases)
                                                                           .Select(_ => new Pair(_))
                                                                           .OrderDescendingByLengthAndText(_ => _.Key);

                ReplacementMapKeys = GetTermsForQuickLookup(ReplacementMap.ToArray(_ => _.Key));

                ByteArrayReplacementMap = AlmostCorrectTaskReturnTypeStartingPhrases.ConcatenatedWith(
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
                                                                                                  "The array with ")
                                                                                    .Select(_ => new Pair(_))
                                                                                    .OrderDescendingByLengthAndText(_ => _.Key);

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

                PreparationMap = new[]
                                 {
                                     new Pair("trivia array", "#1#"),
                                 };

                PreparationMapKeys = PreparationMap.ToArray(_ => _.Key);

                CleanupMap = new[]
                                 {
                                     new Pair("#1#", "trivia"),
                                     new Pair("the modified set", "elements from the original set"),
                                 };

                CleanupMapKeys = CleanupMap.ToArray(_ => _.Key);
            }
#pragma warning restore CA1861

            private static IEnumerable<string> CreatePhrases()
            {
                var startingWords = new[] { "a", "an", "the" };
                var modifications = new[] { "readonly", "read-only", "read only", "filtered", "concurrent" };
                var collections = new[]
                                      {
                                          "array", "arraylist", "array list", "list",  "collection", "dictionary", "enumerable", "enumerable collection", "syntax list", "separated syntax list", "immutable array",
                                          "hash set", "hash table", "hashed set", "hashed table", "hashing set", "hashing table", "hashset", "hashSet", "hashtable", "hashTable",
                                          "map", "queue", "stack", "bag",
                                      };
                var prepositions = new[] { "of", "with", "that contains", "which contains", "that holds", "which holds", "containing", "holding" };

                foreach (var collection in collections)
                {
                    foreach (var preposition in prepositions)
                    {
                        var phrase = string.Concat(collection, " ", preposition, " ");

                        yield return phrase;
                        yield return phrase.ToUpperCaseAt(0);

                        foreach (var modification in modifications)
                        {
                            var modificationPhrase = string.Concat(modification, " ", phrase);

                            yield return modificationPhrase;
                            yield return modificationPhrase.ToUpperCaseAt(0);

                            foreach (var startingWord in startingWords)
                            {
                                var shortStartingPhrase = string.Concat(startingWord, " ", collection, " ");

                                yield return shortStartingPhrase;
                                yield return shortStartingPhrase.ToUpperCaseAt(0);

                                var modifiedStartingPhrase = string.Concat(startingWord, " ", modificationPhrase);

                                yield return modifiedStartingPhrase;
                                yield return modifiedStartingPhrase.ToUpperCaseAt(0);

                                var startingPhrase = string.Concat(startingWord, " ", phrase);

                                if (startingPhrase is Constants.Comments.CollectionReturnTypeStartingPhraseLowerCase)
                                {
                                    // we do not want to have that as that is the correct phrase which we shall not replace
                                }
                                else
                                {
                                    yield return startingPhrase;
                                    yield return startingPhrase.ToUpperCaseAt(0);
                                }
                            }
                        }
                    }
                }
            }
        }
//// ncrunch: no coverage end
//// ncrunch: rdi default
    }
}