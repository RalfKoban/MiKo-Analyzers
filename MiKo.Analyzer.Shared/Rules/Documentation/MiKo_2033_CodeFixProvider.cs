using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2033_CodeFixProvider)), Shared]
    public sealed class MiKo_2033_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private const string SpecialAlmostCorrectTaskStartingPhraseIncomplete = "A task that represents the asynchronous operation. The ";
        private const string SpecialAlmostCorrectTaskStartingPhrase = "A task that represents the asynchronous operation. The value of the ";

        private static readonly string[] TaskParts = Constants.Comments.StringTaskReturnTypeStartingPhraseTemplate.FormatWith("|", "|", "contains").Split('|');
        private static readonly string[] StringParts = Constants.Comments.StringReturnTypeStartingPhraseTemplate.FormatWith("|", "that contains").Split('|');

        private static readonly string[] TextParts =
                                                     {
                                                         "containing",
                                                         "returning",
                                                         "representing",
                                                         "that contains",
                                                         "that represents",
                                                         "that returns",
                                                         "which contains",
                                                         "which represents",
                                                         "which returns",
                                                     };

        private static readonly Pair[] PreparationMap =
                                                        {
                                                            new Pair("new string value with", "#1#"),
                                                            new Pair("new string with", "#2#"),
                                                        };

        private static readonly Pair[] CleanupMap =
                                                    {
                                                        new Pair("#1#", "new string value with"),
                                                        new Pair("#2#", "new string with"),
                                                        new Pair("that contains a new string value", "that contains the original value"),
                                                        new Pair("that contains a new string", "that contains the original value"),
                                                        new Pair("that contains a new", "that contains the original value"),
                                                        new Pair("that contains the new string value", "that contains the original value"),
                                                        new Pair("that contains the new string", "that contains the original value"),
                                                        new Pair("that contains the new", "that contains the original value"),
                                                        new Pair("that contains a formatted string", "that contains the formatted result"),
                                                        new Pair("that contains the formatted string", "that contains the formatted result"),
                                                        new Pair("that contains a ", "that contains the "),
                                                        new Pair("that contains value ", "that contains the value "),
                                                    };

        private static readonly string[] CleanupMapKeys = CleanupMap.ToArray(_ => _.Key);

        private static readonly string[] ReplacementMapKeys = CreateReplacementMapKeys().OrderDescendingByLengthAndText();

        private static readonly Pair[] ReplacementMap = PreparationMap.Concat(ReplacementMapKeys.ToArray(_ => new Pair(_))).ToArray();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2033";

        protected override XmlElementSyntax GenericComment(Document document, XmlElementSyntax comment, string memberName, GenericNameSyntax returnType)
        {
            var content = comment.Content;

            if (content.Count > 5)
            {
                // we might have an almost complete string
                if (content[0] is XmlTextSyntax startText && content[1].IsSeeCrefTaskResult())
                {
                    var newComment = ReplaceText(comment, startText, "representing", "that represents");

                    if (newComment.Content[2] is XmlTextSyntax continueText1)
                    {
                        newComment = ReplaceText(newComment, continueText1, "returning", "that returns");
                    }

                    if (newComment.Content[3].IsSeeCref("string") && newComment.Content[4] is XmlTextSyntax continueText2)
                    {
                        newComment = ReplaceText(newComment, continueText2, TextParts, "that contains");
                    }

                    var first = (XmlTextSyntax)newComment.Content.First();
                    newComment = newComment.ReplaceNode(first, first.WithoutLeadingXmlComment().WithLeadingXmlComment());

                    var last = (XmlTextSyntax)newComment.Content.Last();
                    newComment = newComment.ReplaceNode(last, last.WithoutTrailingXmlComment().WithTrailingXmlComment());

                    return newComment;
                }
            }
            else if (content.Count is 5)
            {
                if (content[0] is XmlTextSyntax start && content[1].IsSeeCref() && content[2] is XmlTextSyntax middle && content[3].IsSeeCref() && content[4] is XmlTextSyntax)
                {
                    // seems like some almost correct text
                    return comment.ReplaceNodes(
                                            new[] { start, middle },
                                            (_, rewritten) => rewritten.ReplaceText(SpecialAlmostCorrectTaskStartingPhrase, SpecialAlmostCorrectTaskStartingPhraseIncomplete) // replace with incomplete one so that the correct one will not get broken
                                                                       .ReplaceText(SpecialAlmostCorrectTaskStartingPhraseIncomplete, SpecialAlmostCorrectTaskStartingPhrase) // now replace with correct one (the line before is needed here to not break the text)
                                                                       .ReplaceText("property on the task object", "parameter"));
                }
            }

            if (content.Count > 0)
            {
                if (content[0] is XmlTextSyntax startText)
                {
                    comment = ReplaceText(comment, startText, AlmostCorrectTaskReturnTypeStartingPhrases, string.Empty);
                }
            }

            // we have to replace the XmlText if it is part of the first item of context
            return Comment(comment, TaskParts[0], SeeCrefTaskResult(), TaskParts[1], SeeCref("string"), TaskParts[2], comment.Content.ToArray());
        }

        protected override XmlElementSyntax NonGenericComment(Document document, XmlElementSyntax comment, string memberName, TypeSyntax returnType)
        {
            if (memberName == nameof(ToString))
            {
                return Comment(comment, "A ", SeeCref("string"), " that represents the current object.");
            }

            var commentStart = StringParts[0];
            var commentEnd = StringParts[1];

            var contents = comment.Content;

            // we might have an almost complete string
            if (contents.Count >= 3 && contents[0] is XmlTextSyntax startText && contents[1].IsSeeCref("string") && contents[2] is XmlTextSyntax continueText)
            {
                if (startText.TextTokens.Any(_ => _.ValueText.AsSpan().TrimStart().Equals(commentStart, StringComparison.Ordinal)))
                {
                    var newComment = ReplaceText(comment, continueText, TextParts, "that contains");

                    if (ReferenceEquals(comment, newComment) is false)
                    {
                        return newComment;
                    }
                }
            }

            // fix start text
            contents = PrepareComment(comment).Content;

            // we have to replace the XmlText if it is part of the first item of context
            var updatedComment = Comment(comment, commentStart, SeeCref("string"), commentEnd, contents.ToArray());

            return CleanupComment(updatedComment);
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment) => Comment(comment, ReplacementMapKeys, ReplacementMap);

        private static XmlElementSyntax CleanupComment(XmlElementSyntax comment) => Comment(comment, CleanupMapKeys, CleanupMap);

//// ncrunch: rdi off

        private static IEnumerable<string> CreateReplacementMapKeys()
        {
            var starts = new[]
                             {
                                 "A ", "A new ", "A single ", "A concatenated ",
                                 "a ", "a new ", "a single ", "a concatenated ",
                                 "The ", "The new ", "The single ", "The concatenated ",
                                 string.Empty,
                             };
            var middles = new[] { "string", "String", "string value", "String value" };

            foreach (var start in starts)
            {
                foreach (var middle in middles)
                {
                    var beginning = string.Concat(start, middle, " ");

                    foreach (var text in TextParts)
                    {
                        var beginningText = string.Concat(beginning, text, " ");

                        yield return beginningText;

                        if (beginningText[0].IsLowerCase())
                        {
                            yield return "Returns " + beginningText;
                            yield return "Return " + beginningText;
                            yield return "returns " + beginningText;
                            yield return "return " + beginningText;
                        }
                    }

                    yield return beginning + "of ";

                    if (start.Contains(" new ") is false)
                    {
                        yield return beginning + "with ";
                    }
                }
            }

            yield return "Contains ";
            yield return "Contain ";
            yield return "Returns ";
            yield return "Return ";
            yield return "returns ";
            yield return "return ";

            foreach (var phrase in AlmostCorrectTaskReturnTypeStartingPhrases)
            {
                yield return phrase;
            }
        }

//// ncrunch: rdi default
    }
}