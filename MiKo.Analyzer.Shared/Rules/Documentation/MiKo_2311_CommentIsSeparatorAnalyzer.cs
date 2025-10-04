using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2311_CommentIsSeparatorAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2311";

        private static readonly string[] Separators = { "----", "****", "====", "####" };

        public MiKo_2311_CommentIsSeparatorAnalyzer() : base(Id)
        {
        }

        internal static bool CommentContainsSeparator(ReadOnlySpan<char> comment) => comment.ContainsAny(Separators);

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.CompilationUnit);

        private void AnalyzeComment(SyntaxNodeAnalysisContext context) => ReportDiagnostics(context, AnalyzeComment(context.Node));

        private IEnumerable<Diagnostic> AnalyzeComment(SyntaxNode node)
        {
            foreach (var trivia in node.DescendantTrivia())
            {
                if (trivia.IsSingleLineComment() && CommentContainsSeparator(trivia.ToString().AsSpan()))
                {
                    yield return Issue(trivia);
                }
            }
        }
    }
}