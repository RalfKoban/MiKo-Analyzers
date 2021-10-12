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
        private static readonly string[] TaskParts = string.Format(Constants.Comments.StringTaskReturnTypeStartingPhraseTemplate, "|", "|", "contains").Split('|');
        private static readonly string[] StringParts = string.Format(Constants.Comments.StringReturnTypeStartingPhraseTemplate, "|", "contains").Split('|');

        public override string FixableDiagnosticId => MiKo_2033_StringReturnTypeDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2033_CodeFixTitle;

        protected override SyntaxNode GenericComment(XmlElementSyntax comment, GenericNameSyntax returnType)
        {
            if (comment.Content.Count > 5)
            {
                // we might have an almost complete string
                if (comment.Content[0] is XmlTextSyntax startText && IsSeeCrefTaskResult(comment.Content[1]))
                {
                    var newComment = ReplaceText(comment, startText, "representing", "that represents");

                    if (newComment.Content[2] is XmlTextSyntax continueText1)
                    {
                        newComment = ReplaceText(newComment, continueText1, "returning", "that returns");
                    }

                    if (IsSeeCref(newComment.Content[3], "string") && newComment.Content[4] is XmlTextSyntax continueText2)
                    {
                        newComment = ReplaceText(newComment, continueText2, "containing", "that contains");
                    }

                    var first = (XmlTextSyntax)newComment.Content.First();
                    newComment = newComment.ReplaceNode(first, first.WithoutLeadingXmlComment().WithLeadingXmlComment());

                    var last = (XmlTextSyntax)newComment.Content.Last();
                    newComment = newComment.ReplaceNode(last, last.WithoutTrailingXmlComment().WithTrailingXmlComment());

                    return newComment;
                }
            }

            // we have to replace the XmlText if it is part of the first item of context
            return Comment(comment, TaskParts[0], SeeCrefTaskResult(), TaskParts[1], SeeCref("string"), TaskParts[2], comment.Content.ToArray());
        }

        protected override XmlElementSyntax NonGenericComment(XmlElementSyntax comment, TypeSyntax returnType)
        {
            var commentStart = StringParts[0];
            var commentEnd = StringParts[1];

            var contents = comment.Content;

            if (contents.Count > 3)
            {
                // we might have an almost complete string
                if (contents[0] is XmlTextSyntax startText && IsSeeCref(contents[1], "string") && contents[2] is XmlTextSyntax continueText)
                {
                    if (startText.TextTokens.Any(_ => _.ValueText.TrimStart() == commentStart))
                    {
                        var newComment = ReplaceText(comment, continueText, "containing", "that contains");

                        if (ReferenceEquals(comment, newComment) is false)
                        {
                            return newComment;
                        }
                    }
                }
            }

            // we have to replace the XmlText if it is part of the first item of context
            return Comment(comment, commentStart, SeeCref("string"), commentEnd, contents.ToArray());
        }

        private static XmlElementSyntax ReplaceText(XmlElementSyntax comment, XmlTextSyntax textSyntax, string phrase, string replacement)
        {
            foreach (var token in textSyntax.TextTokens)
            {
                var text = token.ValueText;

                if (text.Contains(phrase))
                {
                    var newToken = token.WithText(text.Replace(phrase, replacement));

                    return comment.ReplaceToken(token, newToken);
                }
            }

            return comment;
        }
    }
}