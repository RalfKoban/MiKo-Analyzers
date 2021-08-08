using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2032_CodeFixProvider)), Shared]
    public sealed class MiKo_2032_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        private static readonly string[] NonGenericParts = string.Format(Constants.Comments.BooleanReturnTypeStartingPhraseTemplate, "|").Split('|');
        private static readonly string[] GenericParts = string.Format(Constants.Comments.BooleanTaskReturnTypeStartingPhraseTemplate, "|").Split('|');

        public override string FixableDiagnosticId => MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2032_CodeFixTitle;

        protected override SyntaxNode GenericComment(XmlElementSyntax comment, GenericNameSyntax returnType) => Comment(comment, GenericParts[0], SeeLangword_True(), GenericParts[1] + comment.Content);

        protected override XmlElementSyntax NonGenericComment(XmlElementSyntax comment, TypeSyntax returnType) => Comment(comment, NonGenericParts[0], SeeLangword_True(), NonGenericParts[1] + comment.Content);
    }
}