﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2209_DocumentationDoesNotUseDoublePeriodsAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2209";

        private static readonly HashSet<char> AllowedChars = new HashSet<char>
                                                                 {
                                                                     '.',
                                                                     '/',
                                                                     '\\',
                                                                 };

        public MiKo_2209_DocumentationDoesNotUseDoublePeriodsAnalyzer() : base(Id)
        {
        }

        internal static bool CommentHasIssue(string comment) => comment.Contains("..", _ => AllowedChars.Contains(_) is false, StringComparison.OrdinalIgnoreCase);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var text in comment.DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens))
            {
                if (CommentHasIssue(text.ValueText))
                {
                    // TODO RKN: report correct location of problematic text
                    yield return Issue(symbol);

                    // for the moment, stop after the first text
                    yield break;
                }
            }
        }
    }
}