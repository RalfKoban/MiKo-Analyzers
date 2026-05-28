using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class SingleLineCommentAnalyzer : DocumentationAnalyzer
    {
        private static readonly SyntaxKind[] Declarations =
                                                            {
                                                                SyntaxKind.MethodDeclaration,
                                                                SyntaxKind.ConstructorDeclaration,
                                                                SyntaxKind.DestructorDeclaration,

                                                                SyntaxKind.OperatorDeclaration,
                                                                SyntaxKind.ConversionOperatorDeclaration,

                                                                SyntaxKind.GetAccessorDeclaration,
                                                                SyntaxKind.SetAccessorDeclaration,

                                                                SyntaxKind.AddAccessorDeclaration,
                                                                SyntaxKind.RemoveAccessorDeclaration,

                                                                SyntaxKind.FieldDeclaration,
                                                                SyntaxKind.EventFieldDeclaration,
                                                            };

        protected SingleLineCommentAnalyzer(string diagnosticId) : base(diagnosticId) => IgnoreMultipleLines = true;

        protected bool IgnoreMultipleLines { get; set; }

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, Declarations);

        protected abstract bool CommentHasIssue(in ReadOnlySpan<char> comment, SemanticModel semanticModel);

        protected virtual bool CommentHasIssue(in SyntaxTrivia trivia, SemanticModel semanticModel)
        {
            var comment = trivia.ToFullString()
                                .AsSpan(2) // removes the leading '//'
                                .Trim(); // gets rid of all (leading or trailing) whitespaces to ease comment comparisons

            return CommentHasIssue(comment, semanticModel);
        }

        protected virtual IReadOnlyList<Diagnostic> CollectIssues(string name, in SyntaxTrivia trivia) => new[] { Issue(name, trivia) };

        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => true;

        protected virtual bool ShallAnalyze(in SyntaxTrivia trivia) => trivia.IsSingleLineComment();

        protected virtual IEnumerable<Diagnostic> AnalyzeCommentTrivia(string name, SyntaxTrivia[] triviaToAnalyze, SemanticModel semanticModel)
        {
            for (int index = 0, count = triviaToAnalyze.Length; index < count; index++)
            {
                var trivia = triviaToAnalyze[index];

                if (IgnoreMultipleLines && trivia.IsSpanningMultipleLines())
                {
                    continue;
                }

                if (CommentHasIssue(trivia, semanticModel))
                {
                    foreach (var issue in CollectIssues(name, trivia))
                    {
                        yield return issue;
                    }
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeCommentTrivia(BaseMethodDeclarationSyntax node, SemanticModel semanticModel)
        {
            var triviaToAnalyze = FindTriviaToAnalyze(node);

            if (triviaToAnalyze.Length is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var name = node.GetName();

            return AnalyzeCommentTrivia(name, triviaToAnalyze, semanticModel);
        }

        private IEnumerable<Diagnostic> AnalyzeCommentTrivia(BaseFieldDeclarationSyntax node, SemanticModel semanticModel)
        {
            var triviaToAnalyze = FindTriviaToAnalyze(node);

            if (triviaToAnalyze.Length is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var name = node.GetName();

            return AnalyzeCommentTrivia(name, triviaToAnalyze, semanticModel);
        }

        private IEnumerable<Diagnostic> AnalyzeCommentTrivia(AccessorDeclarationSyntax node, SemanticModel semanticModel)
        {
            var triviaToAnalyze = FindTriviaToAnalyze(node);

            if (triviaToAnalyze.Length is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var name = node.GetName();

            return AnalyzeCommentTrivia(name, triviaToAnalyze, semanticModel);
        }

        private void AnalyzeComment(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyze(context.GetEnclosingMethod()))
            {
                switch (context.Node)
                {
                    case BaseMethodDeclarationSyntax method:
                        AnalyzeComment(context, method);

                        break;

                    case BaseFieldDeclarationSyntax field: // fields seem te be more than accessors
                        AnalyzeComment(context, field);

                        break;

                    case AccessorDeclarationSyntax accessor:
                        AnalyzeComment(context, accessor);

                        break;
                }
            }
        }

        private void AnalyzeComment(in SyntaxNodeAnalysisContext context, BaseMethodDeclarationSyntax node)
        {
            var issues = AnalyzeCommentTrivia(node, context.SemanticModel);

            if (issues.IsEmptyArray() is false)
            {
                ReportDiagnostics(context, issues);
            }
        }

        private void AnalyzeComment(in SyntaxNodeAnalysisContext context, BaseFieldDeclarationSyntax node)
        {
            var issues = AnalyzeCommentTrivia(node, context.SemanticModel);

            if (issues.IsEmptyArray() is false)
            {
                ReportDiagnostics(context, issues);
            }
        }

        private void AnalyzeComment(in SyntaxNodeAnalysisContext context, AccessorDeclarationSyntax node)
        {
            var issues = AnalyzeCommentTrivia(node, context.SemanticModel);

            if (issues.IsEmptyArray() is false)
            {
                ReportDiagnostics(context, issues);
            }
        }

        private SyntaxTrivia[] FindTriviaToAnalyze(SyntaxNode node)
        {
            var span = node.FullSpan;

            if (node.HasStructuredTrivia)
            {
                // seems we have an XML comment (or a region)
                // so we have to jump over the first descendant token (not the child itself as the node may consist of other nodes!)
                // as the trivia is located there
                var token = node.FirstDescendantToken();

                span = TextSpan.FromBounds(token.FullSpan.End, span.End);
            }

            List<SyntaxTrivia> triviaToAnalyze = null;

            foreach (var trivia in node.DescendantTrivia(span))
            {
                // we use 'RawKind' for performance reasons as most likely, we have single line comments
                // (SyntaxKind.MultiLineCommentTrivia is 1 higher than SyntaxKind.SingleLineCommentTrivia, so we include both)
                // Note that the method 'IsComment' got inlined here for performance reasons as invoking the method would have some remarkably costly overhead
                if ((uint)(trivia.RawKind - (int)SyntaxKind.SingleLineCommentTrivia) <= 1)
                {
                    if (ShallAnalyze(trivia))
                    {
                        if (triviaToAnalyze is null)
                        {
                            triviaToAnalyze = new List<SyntaxTrivia>(4);
                        }

                        triviaToAnalyze.Add(trivia);
                    }
                }
            }

            // most times we do not have trivia, so it's acceptable to return a new array
            return triviaToAnalyze?.ToArray() ?? Array.Empty<SyntaxTrivia>();
        }
    }
}