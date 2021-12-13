using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2031_CodeFixProvider)), Shared]
    public sealed class MiKo_2031_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        private static readonly string[] Parts = string.Format(Constants.Comments.GenericTaskReturnTypeStartingPhraseTemplate, "task", "|").Split('|');

        private static readonly string[] TextParts =
            {
                "A result containing",
                "A result that contains",
                "A result which contains",
                "A task containing",
                "A task that contains",
                "A task which contains",
                "The task containing",
                "The task that contains",
                "The task which contains",
            };

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
            if (comment.Content.FirstOrDefault() is XmlTextSyntax startText)
            {
                return ReplaceText(comment, startText, TextParts, string.Empty);
            }

            return comment;
        }
    }
}