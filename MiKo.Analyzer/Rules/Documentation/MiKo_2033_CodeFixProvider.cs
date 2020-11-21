using System.Composition;

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
            return Comment(comment, TaskParts[0], SeeCrefTaskResult(), TaskParts[1], SeeCref("string"), TaskParts[2] + comment.Content);
        }

        protected override XmlElementSyntax NonGenericComment(XmlElementSyntax comment, TypeSyntax returnType)
        {
            return Comment(comment, StringParts[0], SeeCref("string"), StringParts[1] + comment.Content);
        }
    }
}