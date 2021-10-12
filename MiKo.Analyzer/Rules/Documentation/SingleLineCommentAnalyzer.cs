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

        protected sealed override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.ConstructorDeclaration);

            context.RegisterCompilationAction(_ => PrepareAnalyzeMethod(_.Compilation));
        }

        protected virtual void PrepareAnalyzeMethod(Compilation compilation)
        {
            // nothing to do here per default
        }

        protected abstract bool CommentHasIssue(string comment, SemanticModel semanticModel);

        protected virtual bool CommentHasIssue(SyntaxTrivia trivia, SemanticModel semanticModel)
        {
            var comment = trivia.ToFullString()
                                .Substring(2) // removes the leading '//'
                                .Trim(); // gets rid of all (leading or trailing) whitespaces to ease comment comparisons

            return CommentHasIssue(comment, semanticModel);
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyzeMethod(context.GetEnclosingMethod()))
            {
                var node = (BaseMethodDeclarationSyntax)context.Node;

                AnalyzeMethod(context, node);
            }
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context, BaseMethodDeclarationSyntax node)
        {
            var issues = AnalyzeSingleLineCommentTrivia(node, context.SemanticModel);

            foreach (var issue in issues)
            {
                context.ReportDiagnostic(issue);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeSingleLineCommentTrivia(BaseMethodDeclarationSyntax node, SemanticModel semanticModel)
        {
            foreach (var trivia in node.DescendantTrivia().Where(_ => _.IsKind(SyntaxKind.SingleLineCommentTrivia)))
            {
                var hasIssue = AnalyzeSingleLineComment(trivia, semanticModel);

                if (hasIssue)
                {
                    yield return Issue(node.GetName(), trivia);
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