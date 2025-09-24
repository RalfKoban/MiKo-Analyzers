using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2222_DocumentationUsesIdentificationInsteadOfIdentAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2222";

        public MiKo_2222_DocumentationUsesIdentificationInsteadOfIdentAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            return AnalyzeComment(comment, Constants.Comments.IdentTerms, Constants.Comments.IdentTerms);
        }
    }
}