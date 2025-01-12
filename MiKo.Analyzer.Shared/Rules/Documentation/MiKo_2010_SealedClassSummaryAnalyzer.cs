﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2010_SealedClassSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2010";

        public MiKo_2010_SealedClassSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsReferenceType && symbol.DeclaredAccessibility == Accessibility.Public && symbol.IsTestClass() is false && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            if (symbol.IsSealed && summaries.None(_ => _.EndsWith(Constants.Comments.SealedClassPhrase, StringComparison.Ordinal)))
            {
                return new[] { Issue(symbol, Constants.Comments.SealedClassPhrase) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}