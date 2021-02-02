using System;
using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    //// <seealso cref="MiKo_2073_CodeFixProvider"/>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2018_CodeFixProvider)), Shared]
    public sealed class MiKo_2018_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private const string StartingPhrase = MiKo_2018_ChecksSummaryAnalyzer.StartingPhrase;
        private const string AsyncStartingPhrase = Constants.Comments.AsynchrounouslyStartingPhrase;

        private static readonly string FixedAsyncStartingPhrase = AsyncStartingPhrase + StartingPhrase.ToLowerCaseAt(0);

        public override string FixableDiagnosticId => MiKo_2018_ChecksSummaryAnalyzer.Id;

        protected override string Title => string.Format(Resources.MiKo_2018_CodeFixTitle, StartingPhrase);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var comment = (XmlElementSyntax)syntax;

            var startText = comment.Content.ToString().Without("/").TrimStart();
            var firstWord = startText.FirstWord();

            var map = new Dictionary<string, string>();

            if (firstWord == AsyncStartingPhrase.TrimEnd())
            {
                firstWord = startText.SecondWord();

                map.Add(AsyncStartingPhrase + firstWord, FixedAsyncStartingPhrase);
            }
            else
            {
                map.Add(firstWord, StartingPhrase);
            }

            // fix the wrong replacements (such as "Determines if " which was replaced into "Determines whether if " due to only first word was replaced)
            map.Add("whether if", "whether");
            map.Add("whether whether", "whether");

            return Comment(comment, new[] { firstWord }, map);
        }
    }
}