using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2211_EnumerationMemberDocumentationHasNoRemarksAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2211";

        public MiKo_2211_EnumerationMemberDocumentationHasNoRemarksAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is IFieldSymbol field && field.ContainingType.IsEnum();

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var remarks = comment.GetRemarksXmls();

            if (remarks.Count == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            return remarks.Select(_ => Issue(_.StartTag)).ToList();
        }
    }
}