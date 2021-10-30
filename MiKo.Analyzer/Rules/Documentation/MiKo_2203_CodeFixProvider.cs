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
        private const string Replacement = "unique identifier";

        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer.Id;

        protected override string Title => "Change 'GUID' into '" + Replacement + "'";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return Comment(syntax, MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer.Guids, ReplacementMap);
        }

        private static Dictionary<string, string> CreateReplacementMap()
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var term in MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer.Guids)
            {
                var replacement = term.ToUpperInvariant().Replace("GUID", Replacement);
                dictionary.Add(term, replacement);
            }

            return dictionary;
        }
    }
}