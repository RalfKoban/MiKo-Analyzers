using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2034_CodeFixProvider)), Shared]
    public sealed class MiKo_2034_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2034_EnumReturnTypeDefaultPhraseAnalyzer.Id;

        protected override string Title => "Fix return comment";

        protected override SyntaxNode GenericComment(XmlElementSyntax comment, GenericNameSyntax returnType)
        {
            var parts = string.Format(Constants.Comments.EnumTaskReturnTypeStartingPhraseTemplate, "task", "|").Split('|');

            return Comment(comment, parts[0], SeeCrefTaskResult(), parts[1] + comment.Content);
        }

        protected override XmlElementSyntax NonGenericComment(XmlElementSyntax comment, TypeSyntax returnType)
        {
            return Comment(comment, Constants.Comments.EnumReturnTypeStartingPhrase, comment.Content.ToString());
        }
    }
}