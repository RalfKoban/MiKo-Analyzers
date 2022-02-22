using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2045_CodeFixProvider)), Shared]
    public sealed class MiKo_2045_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2045_InvalidParameterReferenceInSummaryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2045_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(CodeFixContext context, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var updatedSyntax = syntax.ReplaceNodes(
                                                    MiKo_2045_InvalidParameterReferenceInSummaryAnalyzer.GetIssues(syntax),
                                                    (original, rewritten) => XmlText(GetParameter(original)));

            return updatedSyntax;
        }

        private static string GetParameter(SyntaxNode original)
        {
            switch (original)
            {
                case XmlElementSyntax e:
                    return GetParameterName(e);

                case XmlEmptyElementSyntax ee:
                    return GetParameterName(ee);

                default:
                    return "TODO";
            }
        }
    }
}