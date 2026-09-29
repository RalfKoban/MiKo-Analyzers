using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2246_XmlTextStartsWithMultipleSpacesAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2246";

        public MiKo_2246_XmlTextStartsWithMultipleSpacesAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            List<Diagnostic> results = null;

            foreach (var token in comment.DescendantTokens(SyntaxKind.XmlTextLiteralToken))
            {
                if (token.Parent is SyntaxNode parent)
                {
                    if (parent.IsKind(SyntaxKind.XmlCDataSection) || parent.Parent.IsCode())
                    {
                        // ignore the code sections
                        continue;
                    }
                }

                var whitespaces = token.ValueText.CountLeadingWhitespaces() - 1;

                if (whitespaces > 0)
                {
                    if (results is null)
                    {
                        results = new List<Diagnostic>(1);
                    }

                    var start = token.SpanStart + 1; // we want to squiggle all the other whitespaces, but not the very first one
                    var location = token.GetLocation(start, start + whitespaces);

                    results.Add(Issue(location, CreateProposalForSpaces(whitespaces)));
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}