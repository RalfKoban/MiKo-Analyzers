﻿using System;
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
                                .AsSpan()
                                .Slice(2) // removes the leading '//'
                                .Trim(); // gets rid of all (leading or trailing) whitespaces to ease comment comparisons

            return CommentHasIssue(comment, semanticModel);
        }

        protected virtual IEnumerable<Diagnostic> CollectIssues(string name, in SyntaxTrivia trivia) => new[] { Issue(name, trivia) };

        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => true;

        protected virtual bool ShallAnalyze(in SyntaxTrivia trivia) => trivia.IsSingleLineComment();

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

        private bool AnalyzeComment(in SyntaxTrivia trivia, SemanticModel semanticModel)
        {
            if (IgnoreMultipleLines && trivia.IsSpanningMultipleLines())
            {
                return false; // ignore comment is multi-line comment (could also have with empty lines in between the different comment lines)
            }

            return CommentHasIssue(trivia, semanticModel);
        }

        private IEnumerable<Diagnostic> AnalyzeCommentTrivia(BaseMethodDeclarationSyntax node, SemanticModel semanticModel)
        {
            var triviaToAnalyze = FindTriviaToAnalyze(node);

            if (triviaToAnalyze.Count is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var name = node.GetName();

            return AnalyzeCommentTrivia(name, triviaToAnalyze, semanticModel);
        }

        private IEnumerable<Diagnostic> AnalyzeCommentTrivia(BaseFieldDeclarationSyntax node, SemanticModel semanticModel)
        {
            var triviaToAnalyze = FindTriviaToAnalyze(node);

            if (triviaToAnalyze.Count > 0)
            {
                var name = node.GetName();

                return AnalyzeCommentTrivia(name, triviaToAnalyze, semanticModel);
            }

            return Array.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeCommentTrivia(AccessorDeclarationSyntax node, SemanticModel semanticModel)
        {
            var triviaToAnalyze = FindTriviaToAnalyze(node);

            if (triviaToAnalyze.Count > 0)
            {
                var name = node.GetName();

                return AnalyzeCommentTrivia(name, triviaToAnalyze, semanticModel);
            }

            return Array.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeCommentTrivia(string name, IReadOnlyList<SyntaxTrivia> triviaToAnalyze, SemanticModel semanticModel)
        {
            var count = triviaToAnalyze.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
                {
                    var trivia = triviaToAnalyze[index];

                    var hasIssue = AnalyzeComment(trivia, semanticModel);

                    if (hasIssue)
                    {
                        foreach (var issue in CollectIssues(name, trivia))
                        {
                            yield return issue;
                        }
                    }
                }
            }
        }

        private IReadOnlyList<SyntaxTrivia> FindTriviaToAnalyze(SyntaxNode node)
        {
            List<SyntaxTrivia> triviaToAnalyze = null;

            // ReSharper disable once LoopCanBePartlyConvertedToQuery : foreach loop is used intentionally for performance gains, so there is no need for a Where clause
            foreach (var trivia in node.DescendantTrivia())
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

            return triviaToAnalyze ?? (IReadOnlyList<SyntaxTrivia>)Array.Empty<SyntaxTrivia>();
        }
    }
}