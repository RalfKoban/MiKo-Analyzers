using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            };

        private static readonly string[] ArgumentBlockMarkers =
            {
                "(",
                ")",
                "[",
                "]",
            };

        private static readonly string[] CodeConditionMarkers =
            {
                "??",
                "?.",
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

        private static readonly string[] CodeStartMarkers =
            {
                "var ",
                "public ",
                "internal ",
                "protected ",
                "protected ",
                "private ",
            };

        private readonly HashSet<string> m_knownTypeNames;
        private readonly HashSet<string> m_knownAssemblyNames;

        public MiKo_2302_CommentedOutCodeAnalyzer() : base(Id)
        {
            m_knownTypeNames = new HashSet<string>();
            m_knownAssemblyNames = new HashSet<string>();

            IgnoreMultipleLines = false;
        }

        protected override void AnalyzeMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax node)
        {
            // avoid multi-threading issues during add (contains will not have an issue because the contents are always added, never removed)
            lock (m_knownAssemblyNames)
            {
                var compilation = context.SemanticModel.Compilation;

                // to speed up the lookup, add known assemblies and their types only once
                foreach (var typeName in compilation.References
                                                    .Select(compilation.GetAssemblyOrModuleSymbol)
                                                    .OfType<IAssemblySymbol>()
                                                    .Where(_ => m_knownAssemblyNames.Add(_.FullyQualifiedName()))
                                                    .SelectMany(_ => _.TypeNames))
                {
                    m_knownTypeNames.Add(typeName);
                }
            }

            base.AnalyzeMethod(context, node);
        }

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel)
        {
            if (comment == "else")
                return true;

            if (comment.StartsWithAny(CodeStartMarkers, StringComparison.Ordinal))
                return true;

            if (comment.ContainsAny(CodeBlockMarkers, StringComparison.OrdinalIgnoreCase))
                return true;

            if (comment.ContainsAny(CodeConditionMarkers, StringComparison.Ordinal))
                return true;

            if (comment.Contains("//")) // comment in comment indicator
                return !comment.Contains("://"); // http indicator

            if (comment.EndsWith(";", StringComparison.Ordinal) || comment.Contains("="))
            {
                if (comment.Contains("."))
                    return true;

                if (comment.ContainsAny(Operators))
                    return true;

                if (comment.ContainsAny(ArgumentBlockMarkers, StringComparison.Ordinal))
                    return true;

                // attempt to find a type because it's likely commented out code if we find some
                var firstWord = comment.FirstWord();
                if (m_knownTypeNames.Contains(firstWord))
                    return true;
            }

            if (comment.Contains("case ") && comment.Contains(":"))
                return true;

            return false;
        }
    }
}