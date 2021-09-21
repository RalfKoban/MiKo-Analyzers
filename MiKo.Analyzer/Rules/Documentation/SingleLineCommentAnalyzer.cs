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
            context.RegisterCompilationStartAction(_ =>
                                                       {
                                                           PrepareAnalyzeMethod(_.Compilation);

                                                           context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
                                                           context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.ConstructorDeclaration);
                                                       });
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

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context, BaseMethodDeclarationSyntax node)
        {
            var issues = AnalyzeSingleLineCommentTrivias(node, context.SemanticModel);

            foreach (var issue in issues)
            {
                context.ReportDiagnostic(issue);
            }
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var node = (BaseMethodDeclarationSyntax)context.Node;

            if (ShallAnalyzeMethod(context.GetEnclosingMethod()))
            {
                AnalyzeMethod(context, node);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeSingleLineCommentTrivias(BaseMethodDeclarationSyntax node, SemanticModel semanticModel)
        {
            var methodName = node.GetName();

            return node.DescendantTrivia()
                       .Where(_ => _.IsKind(SyntaxKind.SingleLineCommentTrivia))
                       .Select(_ => AnalyzeSingleLineComment(_, methodName, semanticModel))
                       .Where(_ => _ != null);
        }

        private Diagnostic AnalyzeSingleLineComment(SyntaxTrivia trivia, string methodName, SemanticModel semanticModel)
        {
            if (trivia.IsSpanningMultipleLines() && IgnoreMultipleLines)
            {
                return null; // ignore comment is multi-line comment (could also have with empty lines in between the different comment lines)
            }

            if (CommentHasIssue(trivia, semanticModel))
            {
                return Issue(methodName, trivia);
            }

            return null;
        }
    }
}