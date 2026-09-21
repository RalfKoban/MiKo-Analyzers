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

        internal static bool CommentContainsSeparator(in ReadOnlySpan<char> comment) => comment.ContainsAny(Separators);

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.CompilationUnit);

        private void AnalyzeComment(SyntaxNodeAnalysisContext context) => ReportDiagnostics(context, AnalyzeComment(context.Node));

        private IReadOnlyList<Diagnostic> AnalyzeComment(SyntaxNode node)
        {
            List<Diagnostic> issues = null;

            foreach (var trivia in node.DescendantTrivia())
            {
                // we use 'RawKind' for performance reasons as most likely, we have single line comments
                // Note that the method 'IsSingleLineComment' got inlined here for performance reasons as invoking the method would have some remarkably costly overhead
                if (trivia.RawKind != (uint)SyntaxKind.SingleLineCommentTrivia)
                {
                    continue;
                }

                if (CommentContainsSeparator(trivia.ToString().AsSpan()))
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(); // use default capacity here as it may not be the only separator in the compilation unit
                    }

                    issues.Add(Issue(trivia));
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }
    }
}