using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2202_CodeFixProvider)), Shared]
    public sealed class MiKo_2202_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => "MiKo_2202";

        protected override string Title => Resources.MiKo_2202_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return Comment(syntax, MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer.Terms, ReplacementMap);
        }

        private static Dictionary<string, string> CreateReplacementMap()
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var term in MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer.Terms)
            {
                var alternative = term.Replace('i', 'I');

                var termWithA = "A " + term.TrimStart();
                var alternativeWithA = "A " + alternative.TrimStart();

                var replacement = term.Replace(MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer.Term, "identifier");
                var replacementWithA = "An " + replacement.TrimStart();

                dictionary.Add(termWithA, replacementWithA);
                dictionary.Add(term, replacement);

                dictionary.Add(alternativeWithA, replacementWithA);
                dictionary.Add(alternative, replacement);

                // also consider the upper case as its a commonly used abbreviation
                dictionary.Add(alternativeWithA.ToUpperInvariant(), replacementWithA);
                dictionary.Add(alternative.ToUpperInvariant(), replacement);
            }

            return dictionary;
        }
    }
}