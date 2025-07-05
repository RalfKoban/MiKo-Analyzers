using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2313_CommentShouldBeDocumentationAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2313";

        private static readonly string[] DocumentationCommentTags =
                                                                    {
                                                                        "Exception:",
                                                                        "Exceptions:",
                                                                        "Param:",
                                                                        "Parameter:",
                                                                        "Parameters:",
                                                                        "Remark:",
                                                                        "Remarks:",
                                                                        "Return value:",
                                                                        "Return:",
                                                                        "Returns:",
                                                                        "ReturnValue:",
                                                                        "Summary:",
                                                                        "Value:",
                                                                    };

        private static readonly SyntaxKind[] Declarations =
                                                            {
                                                                SyntaxKind.ClassDeclaration,
                                                                SyntaxKind.InterfaceDeclaration,
                                                                SyntaxKind.StructDeclaration,
                                                                SyntaxKind.RecordDeclaration,
                                                                SyntaxKind.EnumDeclaration,
                                                                SyntaxKind.DelegateDeclaration,

                                                                SyntaxKind.MethodDeclaration,
                                                                SyntaxKind.ConstructorDeclaration,
                                                                SyntaxKind.DestructorDeclaration,

                                                                SyntaxKind.OperatorDeclaration,
                                                                SyntaxKind.ConversionOperatorDeclaration,

                                                                SyntaxKind.PropertyDeclaration,
                                                                SyntaxKind.IndexerDeclaration,
                                                                SyntaxKind.EnumMemberDeclaration,

                                                                SyntaxKind.FieldDeclaration,
                                                                SyntaxKind.EventFieldDeclaration,
                                                            };

        public MiKo_2313_CommentShouldBeDocumentationAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, Declarations);

        private static IReadOnlyCollection<string> FindCommentTags(ReadOnlySpan<string> comments)
        {
            var length = DocumentationCommentTags.Length;
            var commentsLength = comments.Length;

            HashSet<string> foundCommentTags = null;

            for (var i = 0; i < length; i++)
            {
                var marker = DocumentationCommentTags[i];

                for (var index = 0; index < commentsLength; index++)
                {
                    if (comments[index].StartsWith(marker, StringComparison.Ordinal))
                    {
                        if (foundCommentTags is null)
                        {
                            foundCommentTags = new HashSet<string>();
                        }

                        foundCommentTags.Add(marker);
                    }
                }
            }

            return (IReadOnlyCollection<string>)foundCommentTags ?? Array.Empty<string>();
        }

        private void AnalyzeComment(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MemberDeclarationSyntax member)
            {
                var comments = member.GetLeadingComments();

                if (comments.Length > 0)
                {
                    var commentTags = FindCommentTags(comments);

                    if (commentTags.Count > 0)
                    {
                        var properties = new Pair(Constants.AnalyzerCodeFixSharedData.CommentTags, commentTags.ConcatenatedWith("|"));

                        context.ReportDiagnostic(Issue(member.GetLeadingTrivia(), properties));
                    }
                }
            }
        }
    }
}