using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2034_CodeFixProvider)), Shared]
    public sealed class MiKo_2034_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        private static readonly string[] Parts = string.Format(Constants.Comments.EnumTaskReturnTypeStartingPhraseTemplate, "task", "|").Split('|');

        public override string FixableDiagnosticId => MiKo_2034_EnumReturnTypeDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2034_CodeFixTitle;

        protected override SyntaxNode GenericComment(XmlElementSyntax comment, GenericNameSyntax returnType)
        {
            return Comment(comment, Parts[0], SeeCrefTaskResult(), Parts[1] + comment.Content);
        }

        protected override XmlElementSyntax NonGenericComment(XmlElementSyntax comment, TypeSyntax returnType)
        {
            return Comment(comment, Constants.Comments.EnumReturnTypeStartingPhrase, comment.Content.ToString());
        }
    }
}