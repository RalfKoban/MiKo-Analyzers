using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2033_CodeFixProvider)), Shared]
    public sealed class MiKo_2033_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
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
                                                         "with",
                                                     };

        private static readonly IReadOnlyCollection<string> ReplacementMapKeys = CreateReplacementMapKeys().Distinct().ToArray();

        private static readonly IReadOnlyCollection<KeyValuePair<string, string>> ReplacementMap = ReplacementMapKeys.Select(_ => new KeyValuePair<string, string>(_, string.Empty))
                                                                                                                     .ToArray(_ => _.Key, AscendingStringComparer.Default);

        public override string FixableDiagnosticId => "MiKo_2033";

        protected override string Title => Resources.MiKo_2033_CodeFixTitle;

        protected override XmlElementSyntax GenericComment(Document document, XmlElementSyntax comment, string memberName, GenericNameSyntax returnType)
        {
            var content = comment.Content;

            if (content.Count > 5)
            {
                // we might have an almost complete string
                if (content[0] is XmlTextSyntax startText && IsSeeCrefTaskResult(content[1]))
                {
                    var newComment = ReplaceText(comment, startText, "representing", "that represents");

                    if (newComment.Content[2] is XmlTextSyntax continueText1)
                    {
                        newComment = ReplaceText(newComment, continueText1, "returning", "that returns");
                    }

                    if (IsSeeCref(newComment.Content[3], "string") && newComment.Content[4] is XmlTextSyntax continueText2)
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
            else if (content.Count == 5)
            {
                if (content[0] is XmlTextSyntax start && IsSeeCref(content[1]) && content[2] is XmlTextSyntax middle && IsSeeCref(content[3]) && content[4] is XmlTextSyntax)
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
            var commentStart = StringParts[0];
            var commentEnd = StringParts[1];

            var contents = comment.Content;

            if (memberName == nameof(ToString))
            {
                return Comment(comment, "A ", SeeCref("string"), " that represents the current object.");
            }

            // we might have an almost complete string
            if (contents.Count >= 3 && contents[0] is XmlTextSyntax startText && IsSeeCref(contents[1], "string") && contents[2] is XmlTextSyntax continueText)
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
            return Comment(comment, commentStart, SeeCref("string"), commentEnd, contents.ToArray());
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment) => Comment(comment, ReplacementMapKeys, ReplacementMap);

        private static IEnumerable<string> CreateReplacementMapKeys()
        {
            var starts = new[] { "a ", "A ", string.Empty };
            var middles = new[] { "string", "String" };

            foreach (var start in starts)
            {
                foreach (var middle in middles)
                {
                    foreach (var text in TextParts)
                    {
                        yield return string.Concat(start, middle, " ", text);
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
    }
}