using System;
using System.Collections.Generic;
using System.Linq;

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

        internal static bool CommentContainsSeparator(in ReadOnlySpan<char> comment) => comment.ContainsAny(Separators);

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.CompilationUnit);

        private void AnalyzeComment(SyntaxNodeAnalysisContext context) => ReportDiagnostics(context, AnalyzeComment(context.Node));

        private IEnumerable<Diagnostic> AnalyzeComment(SyntaxNode node)
        {
            // TODO RKN: Consider to not include all descendants, especially 'SyntaxKind.SingleLineDocumentationCommentTrivia' as inspecting documentation might take a lot of time
            foreach (var trivia in node.DescendantTrivia().Where(_ => _.IsSingleLineComment()))
            {
                if (CommentContainsSeparator(trivia.ToString().AsSpan()))
                {
                    yield return Issue(trivia);
                }
            }
        }
    }
}