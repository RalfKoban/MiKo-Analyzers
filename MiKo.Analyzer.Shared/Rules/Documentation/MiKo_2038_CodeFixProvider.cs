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
//// ncrunch: rdi off

        private static readonly string[] CommandStartingPhrases =
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

        private static readonly Pair[] CommandReplacementMap = CreateCommandReplacementMapEntries().OrderByDescending(_ => _.Key.Length)
                                                                                                   .ThenBy(_ => _.Key)
                                                                                                   .ToArray();

        private static readonly string[] CommandReplacementMapKeys = CommandReplacementMap.ToArray(_ => _.Key);

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2038";

        internal static bool CanFix(in ReadOnlySpan<char> text) => text.StartsWithAny(CommandStartingPhrases, StringComparison.OrdinalIgnoreCase);

        internal static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is XmlElementSyntax element)
            {
                var comment = Comment(element, CommandReplacementMapKeys, CommandReplacementMap, FirstWordAdjustment.StartLowerCase);

                return CommentStartingWith(comment, Constants.Comments.CommandSummaryStartingPhrase, FirstWordAdjustment.StartLowerCase | FirstWordAdjustment.MakeInfinite);
            }

            return syntax;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntax(syntax);

//// ncrunch: rdi off

        private static List<Pair> CreateCommandReplacementMapEntries()
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

            var results = new List<Pair>();

            foreach (var phrase in CommandStartingPhrases)
            {
                var start = phrase.AsSpan().Trim().ToString();

                foreach (var middle in middleParts)
                {
                    results.Add(new Pair(string.Concat(start, " ", middle, " ")));
                }

                results.Add(new Pair(string.Concat(start, " ")));
            }

            results.Add(new Pair(string.Concat("Offers to", " ")));
            results.Add(new Pair(string.Concat("Tries to", " ")));

            return results;
        }
    }

    //// ncrunch: rdi default
}