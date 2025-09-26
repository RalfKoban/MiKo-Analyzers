using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2075_ActionFunctionParameterPhraseAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2075";

        private static readonly string[] ActionTermsWithDelimiters = Constants.Comments.ActionTerms.WithDelimiters();

        public MiKo_2075_ActionFunctionParameterPhraseAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            return AnalyzeComment(comment, Constants.Comments.ActionTerms, ActionTermsWithDelimiters, Constants.Comments.CallbackTerm, StringComparison.Ordinal, "local function");
        }
    }
}