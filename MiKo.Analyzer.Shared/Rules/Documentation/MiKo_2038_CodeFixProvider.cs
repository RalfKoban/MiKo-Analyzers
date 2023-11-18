﻿using System;
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
            var c = Comment((XmlElementSyntax)syntax, CommandReplacementMap.Keys, CommandReplacementMap, FirstWordHandling.MakeLowerCase);

            return CommentStartingWith(c, Constants.Comments.CommandSummaryStartingPhrase, FirstWordHandling.MakeLowerCase | FirstWordHandling.MakeInfinite);
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntax(syntax);

        private static Dictionary<string, string> CreateReplacementMap()
        {
            var entries = CreateCommandReplacementMapEntries().ToArray(_ => _.Key, AscendingStringComparer.Default); // sort by first character

            var result = new Dictionary<string, string>(entries.Length);

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

            var results = new List<KeyValuePair<string, string>>();

            foreach (var phrase in CommandStartingPhrases)
            {
                var start = phrase.AsSpan().Trim();

                foreach (var middle in middleParts)
                {
                    results.Add(new KeyValuePair<string, string>(string.Concat(start.ToString(), " ", middle, " "), string.Empty));
                }
            }

            return results;
        }
    }
}