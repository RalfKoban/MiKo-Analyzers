using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    /// <inheritdoc />
    /// <seealso cref="MiKo_2018_CodeFixProvider"/>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2073_CodeFixProvider)), Shared]
    public sealed class MiKo_2073_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private const string StartingPhrase = Constants.Comments.DeterminesWhetherPhrase;
        private const string AsyncStartingPhrase = Constants.Comments.AsynchronouslyStartingPhrase;

        private static readonly string FixedAsyncStartingPhrase = AsyncStartingPhrase + StartingPhrase.ToLowerCaseAt(0);

        public override string FixableDiagnosticId => "MiKo_2073";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return updatedSyntax;
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var comment = (XmlElementSyntax)syntax;

            var startText = comment.Content.ToString().AsCachedBuilder().Without('/').WithoutNewLines().TrimStart(Constants.Comments.Delimiters).ToStringAndRelease();

            if (startText.IsNullOrWhiteSpace())
            {
                return Comment(comment, StartingPhrase);
            }

            var firstWord = startText.FirstWord();

            var map = new Pair[3];

            if (firstWord is Constants.Comments.Asynchronously)
            {
                firstWord = startText.SecondWord();

                map[0] = new Pair(AsyncStartingPhrase + firstWord, FixedAsyncStartingPhrase);
            }
            else
            {
                map[0] = new Pair(firstWord, StartingPhrase);
            }

            // fix the wrong replacements (such as "Determines if " which was replaced into "Determines whether if " due to only first word was replaced)
            map[1] = new Pair("whether if", "whether");
            map[2] = new Pair("whether whether", "whether");

            return Comment(comment, new[] { firstWord }, map);
        }
    }
}