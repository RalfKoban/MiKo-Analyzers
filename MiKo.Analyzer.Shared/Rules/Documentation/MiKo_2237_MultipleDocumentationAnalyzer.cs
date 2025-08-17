using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2237_MultipleDocumentationAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2237";

        public MiKo_2237_MultipleDocumentationAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel) => HasIssue(comment)
                                                                                                                                                              ? new[] { Issue(comment) }
                                                                                                                                                              : Array.Empty<Diagnostic>();

        private static bool HasIssue(DocumentationCommentTriviaSyntax comment)
        {
            var token = comment.ParentTrivia.Token;

            var comments = token.GetDocumentationCommentTriviaSyntax();

            if (comments.Length <= 1)
            {
                // we only have a single comment, so nothing to report at all
                return false;
            }

            if (comments.IndexOf(comment) <= 0)
            {
                // we have multiple comments, but the comment in question is the first one, and we do not want to report that one
                return false;
            }

            if (token.HasLeadingComment())
            {
                // this seems to be some commented out code, so we cannot combine both documentation comments
                return false;
            }

            return true;
        }
    }
}