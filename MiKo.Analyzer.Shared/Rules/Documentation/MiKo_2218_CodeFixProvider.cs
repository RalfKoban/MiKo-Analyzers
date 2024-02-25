using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2218_CodeFixProvider)), Shared]
    public sealed class MiKo_2218_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2218";

        protected override string Title => Resources.MiKo_2218_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var affectedNodes = syntax.DescendantNodes<XmlTextSyntax>(_ => _.GetLocation().Contains(diagnostic.Location));

            return syntax.ReplaceNodes(affectedNodes, (_, rewritten) => MiKo_2218_DocumentationShouldNotContainUsedToAnalyzer.GetBetterText(rewritten, diagnostic));
        }
    }
}