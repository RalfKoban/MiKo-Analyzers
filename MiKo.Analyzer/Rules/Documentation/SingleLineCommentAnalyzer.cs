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

        protected sealed override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);

        protected abstract bool CommentHasIssue(string comment, SemanticModel semanticModel);

        protected virtual void AnalyzeMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax node)
        {
            var issues = AnalyzeSingleLineCommentTrivias(node, context.SemanticModel);

            foreach (var issue in issues)
            {
                context.ReportDiagnostic(issue);
            }
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var node = (MethodDeclarationSyntax)context.Node;

            if (ShallAnalyzeMethod(context.GetEnclosingMethod()))
            {
                AnalyzeMethod(context, node);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeSingleLineCommentTrivias(MethodDeclarationSyntax node, SemanticModel semanticModel)
        {
            return node.DescendantTrivia()
                       .Where(_ => _.IsKind(SyntaxKind.SingleLineCommentTrivia))
                       .Select(_ => AnalyzeSingleLineComment(_, node.Identifier.Text, semanticModel))
                       .Where(_ => _ != null);
        }

        private Diagnostic AnalyzeSingleLineComment(SyntaxTrivia trivia, string methodName, SemanticModel semanticModel)
        {
            if (trivia.IsSpanningMultipleLines() && IgnoreMultipleLines)
            {
                return null; // ignore comment is multi-line comment (could also have with empty lines in between the different comment lines)
            }

            var comment = trivia.ToFullString()
                                .Substring(2) // removes the leading '//'
                                .Trim(); // gets rid of all (leading or trailing) whitespaces to ease comment comparisons

            if (CommentHasIssue(comment, semanticModel))
            {
                return Issue(methodName, trivia.GetLocation());
            }

            return null;
        }
    }
}