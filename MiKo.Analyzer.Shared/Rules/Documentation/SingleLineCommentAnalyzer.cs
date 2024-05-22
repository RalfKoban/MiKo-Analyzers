using System;
using System.Collections.Generic;

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
            context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.DestructorDeclaration);

            context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.ConversionOperatorDeclaration);

            context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.GetAccessorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.SetAccessorDeclaration);

            context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.AddAccessorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.RemoveAccessorDeclaration);

            context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.EventFieldDeclaration);
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

        protected virtual bool ShallAnalyze(SyntaxTrivia trivia) => trivia.IsSingleLineComment();

        private void AnalyzeComment(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyze(context.GetEnclosingMethod()))
            {
                switch (context.Node)
                {
                    case BaseMethodDeclarationSyntax method:
                        AnalyzeComment(context, method);
                        break;

                    case AccessorDeclarationSyntax accessor:
                        AnalyzeComment(context, accessor);
                        break;

                    case BaseFieldDeclarationSyntax field:
                        AnalyzeComment(context, field);
                        break;
                }
            }
        }

        private void AnalyzeComment(SyntaxNodeAnalysisContext context, BaseMethodDeclarationSyntax node)
        {
            var issues = AnalyzeCommentTrivia(node, context.SemanticModel);

            ReportDiagnostics(context, issues);
        }

        private void AnalyzeComment(SyntaxNodeAnalysisContext context, BaseFieldDeclarationSyntax node)
        {
            var issues = AnalyzeCommentTrivia(node, context.SemanticModel);

            ReportDiagnostics(context, issues);
        }

        private void AnalyzeComment(SyntaxNodeAnalysisContext context, AccessorDeclarationSyntax node)
        {
            var issues = AnalyzeCommentTrivia(node, context.SemanticModel);

            ReportDiagnostics(context, issues);
        }

        private bool AnalyzeComment(SyntaxTrivia trivia, SemanticModel semanticModel)
        {
            if (trivia.IsSpanningMultipleLines() && IgnoreMultipleLines)
            {
                return false; // ignore comment is multi-line comment (could also have with empty lines in between the different comment lines)
            }

            return CommentHasIssue(trivia, semanticModel);
        }

        private IEnumerable<Diagnostic> AnalyzeCommentTrivia(BaseMethodDeclarationSyntax node, SemanticModel semanticModel)
        {
            foreach (var trivia in node.DescendantTrivia())
            {
                if (ShallAnalyze(trivia))
                {
                    var hasIssue = AnalyzeComment(trivia, semanticModel);

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
        }

        private IEnumerable<Diagnostic> AnalyzeCommentTrivia(BaseFieldDeclarationSyntax node, SemanticModel semanticModel)
        {
            foreach (var trivia in node.DescendantTrivia())
            {
                if (ShallAnalyze(trivia))
                {
                    var hasIssue = AnalyzeComment(trivia, semanticModel);

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
        }

        private IEnumerable<Diagnostic> AnalyzeCommentTrivia(AccessorDeclarationSyntax node, SemanticModel semanticModel)
        {
            foreach (var trivia in node.DescendantTrivia())
            {
                if (ShallAnalyze(trivia))
                {
                    var hasIssue = AnalyzeComment(trivia, semanticModel);

                    if (hasIssue)
                    {
                        var name = GetName();

                        foreach (var issue in CollectIssues(name, trivia))
                        {
                            yield return issue;
                        }
                    }
                }
            }

            string GetName()
            {
                var syntaxNode = node.Parent?.Parent;

                switch (syntaxNode)
                {
                    case BasePropertyDeclarationSyntax b:
                        return b.GetName();

                    case EventFieldDeclarationSyntax ef:
                        return ef.GetName();
                }

                return string.Empty;
            }
        }
    }
}