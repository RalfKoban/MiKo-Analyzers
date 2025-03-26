using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var documentation = comment.ParentTrivia.Token.Parent.GetDocumentationCommentTriviaSyntax();

            if (documentation.Length > 1)
            {
                if (documentation.IndexOf(comment) > 0)
                {
                    return new[] { Issue(comment) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}