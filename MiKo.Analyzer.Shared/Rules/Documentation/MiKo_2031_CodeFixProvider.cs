﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2031_CodeFixProvider)), Shared]
    public sealed class MiKo_2031_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly string[] Parts = Constants.Comments.GenericTaskReturnTypeStartingPhraseTemplate.FormatWith("task", "|").Split('|');

        private static readonly string[] ContinueTextParts =
                                                             {
                                                                 "containing",
                                                                 "that contains",
                                                                 "which contains",
                                                                 "that represents the operation.",
                                                                 "that represents the asynchronous operation.",
                                                             };

        private static readonly string[] TextParts = CreateTextParts().OrderByDescending(_ => _.Length).ThenBy(_ => _).ToArray();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2031";

        protected override SyntaxNode Comment(Document document, XmlElementSyntax comment, MethodDeclarationSyntax method)
        {
            return HandleSpecialMethod(comment, method) ?? base.Comment(document, comment, method);
        }

        protected override XmlElementSyntax GenericComment(Document document, XmlElementSyntax comment, string memberName, GenericNameSyntax returnType)
        {
            comment = PrepareGenericComment(comment);

            // we have to replace the XmlText if it is part of the first item of context
            return Comment(comment, Parts[0], SeeCrefTaskResult(), Parts[1], comment.Content);
        }

        protected override XmlElementSyntax NonGenericComment(Document document, XmlElementSyntax comment, string memberName, TypeSyntax returnType)
        {
            return Comment(comment, Constants.Comments.NonGenericTaskReturnTypePhrase);
        }

        private static XmlElementSyntax HandleSpecialMethod(XmlElementSyntax comment, MethodDeclarationSyntax method)
        {
            switch (method.GetName())
            {
                case nameof(Task.FromCanceled): return Comment(comment, Constants.Comments.FromCanceledTaskReturnTypeStartingPhrase);
                case nameof(Task.FromException): return Comment(comment, Constants.Comments.FromExceptionTaskReturnTypeStartingPhrase);
                case nameof(Task.FromResult): return Comment(comment, Constants.Comments.FromResultTaskReturnTypeStartingPhrase);
                case nameof(Task.ContinueWith): return Comment(comment, Constants.Comments.ContinueWithTaskReturnTypeStartingPhrase);
                case nameof(Task.Run): return Comment(comment, Constants.Comments.RunTaskReturnTypeStartingPhrase);
                case nameof(Task.WhenAll): return Comment(comment, Constants.Comments.WhenAllTaskReturnTypeStartingPhrase);
                case nameof(Task.WhenAny):
                {
                    var parts = Constants.Comments.WhenAnyTaskReturnTypeStartingPhraseTemplate.FormatWith("task", "|").Split('|');

                    return Comment(comment, parts[0], SeeCrefTaskResult(), parts[1]);
                }

                default: return null;
            }
        }

        private static XmlElementSyntax PrepareGenericComment(XmlElementSyntax comment)
        {
            var contents = comment.Content;

            if (contents.FirstOrDefault() is XmlTextSyntax startText)
            {
                if (contents.Count >= 3)
                {
                    if (contents[1].IsSeeCrefTask() && contents[2] is XmlTextSyntax continueText)
                    {
                        // might be an almost complete text
                        var text = continueText.GetTextTrimmed();

                        if (text.StartsWithAny(ContinueTextParts))
                        {
                            var newText = text.Without(ContinueTextParts);

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

                return ReplaceText(comment, startText, TextParts, string.Empty);
            }

            return comment;
        }

//// ncrunch: rdi off

        private static HashSet<string> CreateTextParts()
        {
            var results = new HashSet<string>
                              {
                                  "An awaitable task.",
                                  "An awaitable task and",
                                  "An awaitable task",
                                  "A task to await.",
                                  "A task to await and",
                                  "A task to await",
                                  "A task that can be used to await.",
                                  "A task that can be used to await and",
                                  "A task that can be used to await",
                              };

            foreach (var phrase in AlmostCorrectTaskReturnTypeStartingPhrases)
            {
                results.Add(phrase);
            }

            foreach (var start in new[] { "A result", "A task", "The task" })
            {
                foreach (var end in ContinueTextParts)
                {
                    results.Add(start + " " + end);
                }
            }

            return results;
        }

//// ncrunch: rdi default
    }
}