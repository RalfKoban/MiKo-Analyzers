using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2038_CodeFixProvider)), Shared]
    public sealed class MiKo_2038_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        internal static readonly string[] CommandStartingPhrases =
            {
                "A command ",
                "Command ",
                "command ",
            };

        private static readonly Dictionary<string, string> CommandReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => MiKo_2038_CommandTypeSummaryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2038_CodeFixTitle;

        internal static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var c = Comment((XmlElementSyntax)syntax, CommandReplacementMap.Keys, CommandReplacementMap);

            return CommentStartingWith(c, Constants.Comments.CommandSummaryStartingPhrase, FirstWordHandling.MakeInfinite);
        }

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntax(syntax);

        private static Dictionary<string, string> CreateReplacementMap()
        {
            var entries = CreateCommandReplacementMapEntries().OrderBy(_ => _.Key[0]).ToList(); // sort by first character

            var result = new Dictionary<string, string>(entries.Count);

            foreach (var entry in entries)
            {
                if (result.ContainsKey(entry.Key) is false)
                {
                    result[entry.Key] = entry.Value;
                }
            }

            return result;
        }

        private static IEnumerable<KeyValuePair<string, string>> CreateCommandReplacementMapEntries()
        {
            var middleParts = new[]
                                  {
                                      "that can",
                                      "that will",
                                      "that",
                                      "which can",
                                      "which will",
                                      "which",
                                      "will",
                                      "to",
                                      "for",
                                  };

            foreach (var start in CommandStartingPhrases.Select(_ => _.Trim()))
            {
                foreach (var middle in middleParts)
                {
                    yield return new KeyValuePair<string, string>($"{start} {middle} ", string.Empty);
                }
            }
        }
    }
}