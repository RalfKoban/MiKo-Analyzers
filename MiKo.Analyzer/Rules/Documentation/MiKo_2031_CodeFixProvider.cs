using System;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2031_CodeFixProvider)), Shared]
    public sealed class MiKo_2031_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        private static readonly string[] Parts = string.Format(Constants.Comments.GenericTaskReturnTypeStartingPhraseTemplate, "task", "|").Split('|');

        private static readonly string[] ContinueTextParts =
            {
                "containing",
                "that contains",
                "which contains",
                "that represents the asynchronous operation.",
            };

        private static readonly string[] TextParts = (from x in new[] { "A result", "A task", "The task" }
                                                      from y in ContinueTextParts
                                                      select x + " " + y).ToArray();

        public override string FixableDiagnosticId => MiKo_2031_TaskReturnTypeDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2031_CodeFixTitle;

        protected override XmlElementSyntax GenericComment(Document document, XmlElementSyntax comment, GenericNameSyntax returnType)
        {
            comment = PrepareComment(comment);

            // we have to replace the XmlText if it is part of the first item of context
            return Comment(comment, Parts[0], SeeCrefTaskResult(), Parts[1], comment.Content);
        }

        protected override XmlElementSyntax NonGenericComment(Document document, XmlElementSyntax comment, TypeSyntax returnType) => Comment(comment, Constants.Comments.NonGenericTaskReturnTypePhrase);

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment)
        {
            var contents = comment.Content;

            if (contents.FirstOrDefault() is XmlTextSyntax startText)
            {
                if (contents.Count >= 3)
                {
                    var taskRef = contents[1];

                    if (IsSeeCrefTask(taskRef) && contents[2] is XmlTextSyntax continueText)
                    {
                        // might be an almost complete text
                        var text = GetText(continueText);

                        if (text.StartsWithAny(ContinueTextParts))
                        {
                            var newText = text.Without(ContinueTextParts).Trim();
                            if (newText.EndsWith('.') is false)
                            {
                                newText += " "; // add extra space so that next XML syntax node is placed well
                            }

                            var newContents = contents.Replace(continueText, XmlText(newText));

                            // remove the beginning node and the <see cref="Task"/> node
                            return comment.WithContent(newContents.RemoveAt(0).RemoveAt(0));
                        }
                    }
                }
                else
                {
                    return ReplaceText(comment, startText, TextParts, string.Empty);
                }
            }

            return comment;
        }
    }
}