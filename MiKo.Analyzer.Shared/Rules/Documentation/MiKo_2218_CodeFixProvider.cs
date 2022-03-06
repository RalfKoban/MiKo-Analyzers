using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2218_CodeFixProvider)), Shared]
    public sealed class MiKo_2218_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2218_DocumentationShouldNotContainUsedToAnalyzer.Id;

        protected override string Title => Resources.MiKo_2218_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(CodeFixContext context, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var affectedTokens = syntax.DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens);

            return syntax.ReplaceTokens(affectedTokens, (original, rewritten) => original.WithText(MiKo_2218_DocumentationShouldNotContainUsedToAnalyzer.GetBetterText(original.Text)));
        }
    }
}