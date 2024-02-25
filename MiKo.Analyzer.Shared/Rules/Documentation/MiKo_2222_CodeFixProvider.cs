using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2222_CodeFixProvider)), Shared]
    public sealed class MiKo_2222_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => "MiKo_2222";

        protected override string Title => Resources.MiKo_2222_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return Comment(syntax, MiKo_2222_DocumentationUsesIdentificationInsteadOfIdentAnalyzer.Terms, ReplacementMap);
        }

        private static Dictionary<string, string> CreateReplacementMap()
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var term in MiKo_2222_DocumentationUsesIdentificationInsteadOfIdentAnalyzer.Terms)
            {
                var replacement = term.Replace(MiKo_2222_DocumentationUsesIdentificationInsteadOfIdentAnalyzer.Term, "identification");
                dictionary.Add(term, replacement);

                // alternative 1
                dictionary.Add(term.Replace('i', 'I'), replacement);

                // alternative 2
                dictionary.Add(term.ToUpper(), replacement);
            }

            return dictionary;
        }
    }
}