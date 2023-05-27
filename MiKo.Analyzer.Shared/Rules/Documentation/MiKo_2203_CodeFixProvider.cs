using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2203_CodeFixProvider)), Shared]
    public sealed class MiKo_2203_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer.Id;

        protected override string Title => Resources.MiKo_2203_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return Comment(syntax, MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer.Phrases, ReplacementMap);
        }

        private static Dictionary<string, string> CreateReplacementMap()
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var term in MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer.Phrases)
            {
                var termWithA = "A " + term.TrimStart();

                var replacement = term.ToUpperInvariant().Replace("GUID", "unique identifier");
                var replacementWithA = "An " + replacement.TrimStart();

                dictionary.Add(termWithA, replacementWithA);
                dictionary.Add(term, replacement);
            }

            return dictionary;
        }
    }
}