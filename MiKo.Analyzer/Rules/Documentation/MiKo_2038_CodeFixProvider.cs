using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2038_CodeFixProvider)), Shared]
    public sealed class MiKo_2038_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2038_CommandTypeSummaryAnalyzer.Id;

        protected override string Title => "Apply default comment to command";

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax) => StartCommentWith((XmlElementSyntax)syntax, Constants.Comments.CommandSummaryStartingPhrase);
    }
}