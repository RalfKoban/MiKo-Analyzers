using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class SingleLineCommentAnalyzer : DocumentationAnalyzer
    {
        protected SingleLineCommentAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected sealed override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);

        protected abstract bool CommentHasIssue(string comment);

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

        private Diagnostic AnalyzeSingleLineComment(SyntaxTrivia trivia, string methodName) => CommentHasIssue(trivia.ToFullString().Remove("//").Trim())
                                                                                                   ? ReportIssue(methodName, trivia.GetLocation())
                                                                                                   : null;
    }
}