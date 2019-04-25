
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2301_TestArrangeActAssertCommentAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2301";

        private static readonly string[] Phrases =
            {
                "arrange",
                "act",
                "assert",
                "prepare",
                "run",
                "set-up",
                "setup",
                "test",
                "verify",
            };

        public MiKo_2301_TestArrangeActAssertCommentAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);

        protected override bool ShallAnalyzeMethod(IMethodSymbol symbol) => symbol.IsTestMethod();

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var node = (MethodDeclarationSyntax)context.Node;

            if (ShallAnalyzeMethod(node.GetEnclosingMethod(context.SemanticModel)))
            {
                var issues = AnalyzeSingleLineCommentTrivias(node);
                foreach (var issue in issues)
                {
                    context.ReportDiagnostic(issue);
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeSingleLineCommentTrivias(MethodDeclarationSyntax node) => node.DescendantTrivia()
                                                                                                             .Where(_ => _.IsKind(SyntaxKind.SingleLineCommentTrivia))
                                                                                                             .Select(_ => AnalyzeSingleLineComment(_, node.Identifier.Text))
                                                                                                             .Where(_ => _ != null);

        private Diagnostic AnalyzeSingleLineComment(SyntaxTrivia trivia, string methodName)
        {
            var comment = trivia.ToFullString().Remove("//").TrimStart();

            return comment.StartsWithAny(Phrases, StringComparison.OrdinalIgnoreCase)
                       ? ReportIssue(methodName, trivia.GetLocation())
                       : null;
        }
    }
}