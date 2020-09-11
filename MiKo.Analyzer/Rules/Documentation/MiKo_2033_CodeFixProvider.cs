using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2033_CodeFixProvider)), Shared]
    public sealed class MiKo_2033_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2033_StringReturnTypeDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2033_CodeFixTitle;

        protected override SyntaxNode GenericComment(XmlElementSyntax comment, GenericNameSyntax returnType)
        {
            var parts = string.Format(Constants.Comments.StringTaskReturnTypeStartingPhraseTemplate, "|", "|", "contains").Split('|');

            return Comment(comment, parts[0], SeeCrefTaskResult(), parts[1], SeeCref("string"), parts[2] + comment.Content);
        }

        protected override XmlElementSyntax NonGenericComment(XmlElementSyntax comment, TypeSyntax returnType)
        {
            var parts = string.Format(Constants.Comments.StringReturnTypeStartingPhraseTemplate, "|", "contains").Split('|');

            return Comment(comment, parts[0], SeeCref("string"), parts[1] + comment.Content);
        }
    }
}