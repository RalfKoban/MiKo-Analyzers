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
                                                                   };

        private static readonly Dictionary<string, string> CommandReplacementMap = CreateCommandReplacementMapEntries().OrderByDescending(_ => _.Key.Length)
                                                                                                                       .ThenBy(_ => _.Key)
                                                                                                                       .ToDictionary(_ => _.Key, _ => _.Value);

        public override string FixableDiagnosticId => MiKo_2038_CommandTypeSummaryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2038_CodeFixTitle;

        internal static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var c = Comment((XmlElementSyntax)syntax, CommandReplacementMap.Keys, CommandReplacementMap, FirstWordHandling.MakeLowerCase);

            return CommentStartingWith(c, Constants.Comments.CommandSummaryStartingPhrase, FirstWordHandling.MakeLowerCase | FirstWordHandling.MakeInfinite);
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntax(syntax);

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
                                      "can be used to",
                                      "is used to",
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

            return results;
        }
    }
}