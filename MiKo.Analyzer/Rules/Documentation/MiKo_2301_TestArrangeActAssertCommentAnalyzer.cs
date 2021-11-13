﻿using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2301_TestArrangeActAssertCommentAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2301";

        private static readonly string[] Phrases =
            {
                "arrange",
                "act",
                "assert",
                "execute",
                "execution",
                "prep",
                "prepare",
                "preparation",
                "run",
                "set-up",
                "setup",
                "test",
                "verify",
                "verification",
            };

        public MiKo_2301_TestArrangeActAssertCommentAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        internal static bool CommentContainsArrangeActAssert(string comment) => comment.StartsWithAny(Phrases, StringComparison.OrdinalIgnoreCase);

        protected override bool ShallAnalyzeMethod(IMethodSymbol symbol) => symbol.IsTestMethod() || symbol.ContainingType.IsTestClass();

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel) => CommentContainsArrangeActAssert(comment);
    }
}