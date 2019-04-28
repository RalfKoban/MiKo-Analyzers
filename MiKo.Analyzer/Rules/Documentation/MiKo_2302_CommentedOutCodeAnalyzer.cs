using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2302_CommentedOutCodeAnalyzer : SingleLineCommentAnalyzer
    {
        public const string Id = "MiKo_2302";

        private static readonly string[] CodeBlockMarkers =
            {
                "{",
                "}",
                "(",
                ")",
                "[",
                "]",
            };

        private static readonly string[] CodeConditionMarkers =
            {
                "??",
                "if(",
                "if (",
                "switch(",
                "switch (",
                "else if(",
                "else if (",
            };

        private static readonly string[] Operators =
            {
                "==",
                "!=",
                ">=",
                "<=",
                ">",
                "<",
                "++",
                "--",
                "+=",
                "-=",
                "*=",
                "/=",
                "=>", // lambda
            };

        public MiKo_2302_CommentedOutCodeAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel)
        {
            if (comment.ContainsAny(CodeBlockMarkers, StringComparison.OrdinalIgnoreCase))
                return true;

            if (comment.StartsWith("var ", StringComparison.Ordinal))
                return true;

            if (comment.ContainsAny(CodeConditionMarkers, StringComparison.Ordinal))
                return true;

            if (comment.Contains("case ") && comment.Contains(":"))
                return true;

            if (comment.ContainsAny(Operators) && comment.Contains(";"))
                return true;

            // attempt to find a type because it's likely commented out code if we find some
            var firstWord = comment.FirstWord();

            // TODO: RKN move into a place where it is not invoked *each* time (for performance reasons)
            var compilation = semanticModel.Compilation;
            var typeNames = compilation.References
                                       .Select(compilation.GetAssemblyOrModuleSymbol)
                                       .OfType<IAssemblySymbol>()
                                       .SelectMany(_ => _.TypeNames)
                                       .ToHashSet();

            if (typeNames.Contains(firstWord))
                return true;

            return false;
        }
    }
}