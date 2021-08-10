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

        public override string FixableDiagnosticId => MiKo_2031_TaskReturnTypeDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2031_CodeFixTitle;

        protected override SyntaxNode GenericComment(XmlElementSyntax comment, GenericNameSyntax returnType) => Comment(comment, Parts[0], SeeCrefTaskResult(), Parts[1] + comment.Content);

        protected override XmlElementSyntax NonGenericComment(XmlElementSyntax comment, TypeSyntax returnType) => Comment(comment, Constants.Comments.NonGenericTaskReturnTypePhrase);
    }
}