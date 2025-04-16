﻿using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2305_CodeFixProvider)), Shared]
    public sealed class MiKo_2305_CodeFixProvider : CommentCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2305";

        protected override SyntaxTrivia ComputeReplacementTrivia(in SyntaxTrivia original, in SyntaxTrivia rewritten)
        {
            var comment = original.ToString();

            if (DocumentationComment.ContainsDoublePeriod(comment.AsSpan().TrimEnd()))
            {
                return SyntaxFactory.Comment(comment.Replace("..", "."));
            }

            return original;
        }
    }
}