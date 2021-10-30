using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2210_CodeFixProvider)), Shared]
    public sealed class MiKo_2210_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private const string Replacement = "information";

        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer.Id;

        protected override string Title => "Change '" + MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer.Term + "' into '" + Replacement + "'";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return Comment(syntax, MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer.Terms, ReplacementMap);
        }

        private static Dictionary<string, string> CreateReplacementMap()
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var term in MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer.Terms)
            {
                var replacement = term.Replace(MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer.Term, Replacement);
                dictionary.Add(term, replacement);

                var alternative = term.Replace('i', 'I');
                dictionary.Add(alternative, replacement);
            }

            return dictionary;
        }
    }
}