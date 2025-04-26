﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2311_CodeFixProvider)), Shared]
    public sealed class MiKo_2311_CodeFixProvider : CommentCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2311";

        protected override SyntaxTrivia ComputeReplacementTrivia(in SyntaxTrivia original, in SyntaxTrivia rewritten) => rewritten;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, in SyntaxTrivia trivia, Diagnostic issue)
        {
            return root.ReplaceTrivia(new[] { trivia }, (original, rewritten) => SyntaxFactory.ElasticCarriageReturnLineFeed);
        }
    }
}