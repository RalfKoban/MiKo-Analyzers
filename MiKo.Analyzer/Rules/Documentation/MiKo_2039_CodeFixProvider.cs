using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2039_CodeFixProvider)), Shared]
    public sealed class MiKo_2039_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2039_ExtensionMethodsClassSummaryAnalyzer.Id;

        protected override string Title => "Apply default extension methods comment to class";

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var parts = string.Format(Constants.Comments.ExtensionMethodClassStartingPhraseTemplate, '|').Split('|');

            return StartCommentWith((XmlElementSyntax)syntax, parts[0], SeeLangword("static"), parts[1]);
        }
    }
}