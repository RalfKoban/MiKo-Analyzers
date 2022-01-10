using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2041_CodeFixProvider)), Shared]
    public sealed class MiKo_2041_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2041_InvalidXmlInSummaryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2041_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(CodeFixContext context, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var syntaxNodes = MiKo_2041_InvalidXmlInSummaryAnalyzer.GetIssues(syntax).ToList();
            var replacements = syntaxNodes.Select(_ => _.WithLeadingXmlCommentExterior().WithEndOfLine()).ToArray();

            var updatedSyntax = syntax.Without(syntaxNodes).AddContent(replacements);

            return updatedSyntax;
        }
    }
}