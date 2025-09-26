using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2203";

        public MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            return AnalyzeComment(comment, Constants.Comments.Guids, Constants.Comments.GuidTermsWithDelimiters, comparison: StringComparison.Ordinal);
        }
    }
}