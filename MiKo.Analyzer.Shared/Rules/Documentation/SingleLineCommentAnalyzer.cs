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
        protected SingleLineCommentAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1)) => IgnoreMultipleLines = true;

        protected bool IgnoreMultipleLines { get; set; }

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.ConstructorDeclaration);
        }

        protected abstract bool CommentHasIssue(ReadOnlySpan<char> comment, SemanticModel semanticModel);

        protected virtual bool CommentHasIssue(SyntaxTrivia trivia, SemanticModel semanticModel)
        {
            var comment = trivia.ToFullString()
                                .AsSpan()
                                .Slice(2) // removes the leading '//'
                                .Trim(); // gets rid of all (leading or trailing) whitespaces to ease comment comparisons

            return CommentHasIssue(comment, semanticModel);
        }

        protected virtual IEnumerable<Diagnostic> CollectIssues(string name, SyntaxTrivia trivia)
        {
            yield return Issue(name, trivia);
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => true;

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyze(context.GetEnclosingMethod()))
            {
                var node = (BaseMethodDeclarationSyntax)context.Node;

                AnalyzeMethod(context, node);
            }
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context, BaseMethodDeclarationSyntax node)
        {
            var issues = AnalyzeSingleLineCommentTrivia(node, context.SemanticModel);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> AnalyzeSingleLineCommentTrivia(BaseMethodDeclarationSyntax node, SemanticModel semanticModel)
        {
            foreach (var trivia in node.DescendantTrivia().Where(_ => _.IsSingleLineComment()))
            {
                var hasIssue = AnalyzeSingleLineComment(trivia, semanticModel);

                if (hasIssue)
                {
                    var name = node.GetName();

                    foreach (var issue in CollectIssues(name, trivia))
                    {
                        yield return issue;
                    }
                }
            }
        }

        private bool AnalyzeSingleLineComment(SyntaxTrivia trivia, SemanticModel semanticModel)
        {
            if (trivia.IsSpanningMultipleLines() && IgnoreMultipleLines)
            {
                return false; // ignore comment is multi-line comment (could also have with empty lines in between the different comment lines)
            }

            return CommentHasIssue(trivia, semanticModel);
        }
    }
}