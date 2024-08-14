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
        private const string StartingPhrase = Constants.Comments.DeterminesWhetherPhrase;
        private const string AsyncStartingPhrase = Constants.Comments.AsynchronouslyStartingPhrase;

        private static readonly string FixedAsyncStartingPhrase = AsyncStartingPhrase + StartingPhrase.ToLowerCaseAt(0);

        public override string FixableDiagnosticId => "MiKo_2018";

        protected override string Title => Resources.MiKo_2018_CodeFixTitle.FormatWith(StartingPhrase);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;

            var startText = comment.Content.ToString().Without("/").TrimStart(Constants.Comments.Delimiters);

            if (startText.IsNullOrWhiteSpace())
            {
                return Comment(comment, StartingPhrase);
            }

            var firstWord = startText.FirstWord();

            var map = new Dictionary<string, string>();

            if (firstWord == Constants.Comments.Asynchronously)
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
            map.Add("whether that", "whether");
            map.Add("whether whether", "whether");

            return Comment(comment, new[] { firstWord }, map);
        }
    }
}