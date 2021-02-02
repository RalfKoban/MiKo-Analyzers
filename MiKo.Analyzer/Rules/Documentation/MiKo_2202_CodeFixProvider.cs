using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2202_CodeFixProvider)), Shared]
    public sealed class MiKo_2202_CodeFixProvider : DocumentationCodeFixProvider
    {
        private const string Replacement = "identifier";

        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer.Id;

        protected override string Title => "Change '" + MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer.Term + "' into '" + Replacement + "'";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var comment = (DocumentationCommentTriviaSyntax)syntax;
            return Comment(comment, MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer.Terms, ReplacementMap);
        }

        private static Dictionary<string, string> CreateReplacementMap()
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var term in MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer.Terms)
            {
                var replacement = term.Replace(MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer.Term, Replacement);
                dictionary.Add(term, replacement);

                var alternative = term.Replace('i', 'I');
                dictionary.Add(alternative, replacement);

                // also consider the upper case as its a commonly used abbreviation
                dictionary.Add(alternative.ToUpperInvariant(), replacement);
            }

            return dictionary;
        }
    }
}