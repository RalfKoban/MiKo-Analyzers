using System;
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
//// ncrunch: rdi off

        internal static readonly string[] CommandStartingPhrases =
                                                                   {
                                                                       "A command ",
                                                                       "A standard command ",
                                                                       "A toggle command ",
                                                                       "The command ",
                                                                       "The standard command ",
                                                                       "The toggle command ",
                                                                       "This command ",
                                                                       "This standard command ",
                                                                       "This toggle command ",
                                                                       "Command ",
                                                                       "command ",
                                                                       "A class ",
                                                                       "The class ",
                                                                       "This class ",
                                                                   };

        private static readonly KeyValuePair<string, string>[] CommandReplacementMap = CreateCommandReplacementMapEntries().OrderByDescending(_ => _.Key.Length)
                                                                                                                           .ThenBy(_ => _.Key)
                                                                                                                           .ToArray();

        private static readonly string[] CommandReplacementMapKeys = CommandReplacementMap.Select(_ => _.Key).ToArray();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2038";

        internal static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is XmlElementSyntax element)
            {
                var comment = Comment(element, CommandReplacementMapKeys, CommandReplacementMap, FirstWordHandling.MakeLowerCase);

                return CommentStartingWith(comment, Constants.Comments.CommandSummaryStartingPhrase, FirstWordHandling.MakeLowerCase | FirstWordHandling.MakeInfinite);
            }

            return syntax;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntax(syntax);

//// ncrunch: rdi off

        private static IEnumerable<KeyValuePair<string, string>> CreateCommandReplacementMapEntries()
        {
            var middleParts = new[]
                                  {
                                      "that can",
                                      "that will",
                                      "that offers to",
                                      "that tries to",
                                      "that",
                                      "which can",
                                      "which will",
                                      "which offers to",
                                      "which tries to",
                                      "which",
                                      "will",
                                      "to",
                                      "for",
                                      "can be used to",
                                      "is used to",
                                      "offers to",
                                      "tries to",
                                  };

            var results = new List<KeyValuePair<string, string>>();

            foreach (var phrase in CommandStartingPhrases)
            {
                var start = phrase.AsSpan().Trim().ToString();

                foreach (var middle in middleParts)
                {
                    results.Add(new KeyValuePair<string, string>(string.Concat(start, " ", middle, " "), string.Empty));
                }

                results.Add(new KeyValuePair<string, string>(string.Concat(start, " "), string.Empty));
            }

            results.Add(new KeyValuePair<string, string>(string.Concat("Offers to", " "), string.Empty));
            results.Add(new KeyValuePair<string, string>(string.Concat("Tries to", " "), string.Empty));

            return results;
        }
    }

    //// ncrunch: rdi default
}