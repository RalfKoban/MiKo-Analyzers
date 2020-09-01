using System;
using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2073_CodeFixProvider)), Shared]
    public sealed class MiKo_2073_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer.Id;

        protected override string Title => "Start summary with '" + MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer.StartingPhrase + "'";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var comment = (XmlElementSyntax)syntax;

            var startText = comment.Content.ToString().Without("/").TrimStart();
            var firstWord = startText.FirstWord();

            var map = new Dictionary<string, string>();

            if (firstWord == Constants.Comments.AsynchrounouslyStartingPhrase.TrimEnd())
            {
                firstWord = startText.SecondWord();

                var key = Constants.Comments.AsynchrounouslyStartingPhrase + firstWord;
                var value = Constants.Comments.AsynchrounouslyStartingPhrase + MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer.StartingPhrase.ToLowerCaseAt(0);
                map.Add(key, value);
            }
            else
            {
                map.Add(firstWord, MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer.StartingPhrase);
            }

            // fix the wrong replacements (such as "Determines if " which was replaced into "Determines whether if " due to only first word was replaced)
            map.Add("whether if", "whether");
            map.Add("whether whether", "whether");

            return Comment(comment, new[] { firstWord }, map);
        }
    }
}