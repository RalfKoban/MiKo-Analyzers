using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2301_TestArrangeActAssertCommentAnalyzer : SingleLineCommentAnalyzer
    {
        public const string Id = "MiKo_2301";

        private static readonly string[] Phrases =
            {
                "arrange",
                "act",
                "assert",
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

        internal static bool CommentContainsArrangeActAssert(string comment) => comment.StartsWithAny(Phrases, StringComparison.OrdinalIgnoreCase);

        protected override bool ShallAnalyzeMethod(IMethodSymbol symbol) => symbol.IsTestMethod();

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel) => CommentContainsArrangeActAssert(comment);
    }
}