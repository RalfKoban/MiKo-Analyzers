using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Extensions;

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
            return Comment(comment, TaskParts[0], SeeCrefTaskResult(), TaskParts[1], SeeCref("string"), TaskParts[2] + CommentStartingWith(comment.Content, string.Empty));
        }

        protected override XmlElementSyntax NonGenericComment(XmlElementSyntax comment, TypeSyntax returnType)
        {
            var contents = comment.Content;
            if (contents.Count > 3)
            {
                // we might have an almost complete string
                if (contents[0] is XmlTextSyntax startText && IsSeeCref(contents[1], "string") && contents[2] is XmlTextSyntax continueText)
                {
                    if (startText.TextTokens.Any(_ => _.ValueText.TrimStart() == StringParts[0]))
                    {
                        foreach (var token in continueText.TextTokens)
                        {
                            var text = token.ValueText;

                            if (text.Contains(" containing "))
                            {
                                var newToken = token.WithText(text.Replace(" containing ", StringParts[1]));

                                return comment.ReplaceToken(token, newToken);
                            }
                        }
                    }
                }
            }

            // TODO RKN: Fix XML escaping caused by string conversion

            // we have to replace the XmlText if it is part of the first item of context
            return Comment(comment, StringParts[0], SeeCref("string"), StringParts[1] + CommentStartingWith(contents, string.Empty));
        }
    }
}