using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2213_CodeFixProvider)), Shared]
    public sealed class MiKo_2213_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2213_DocumentationContainsNtContradictionAnalyzer.Id;

        protected override string Title => Resources.MiKo_2213_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(CodeFixContext context, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var affectedTokens = syntax.DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens).Where(_ => _.ValueText.Contains("n't")).ToList();

            return syntax.ReplaceTokens(affectedTokens, (original, rewritten) => original.WithText(GetFixedText(original.Text)));
        }

        private static string GetFixedText(string text) => text
                                                           .Replace("an't", "annot")
                                                           .Replace("won't", "will not")
                                                           .Replace("Won't", "Will not")
                                                           .Replace("n't", " not");
    }
}