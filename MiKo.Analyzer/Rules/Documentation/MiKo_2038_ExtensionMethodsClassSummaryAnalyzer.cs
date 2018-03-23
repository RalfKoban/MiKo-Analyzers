﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2038_ExtensionMethodsClassSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2038";

        public MiKo_2038_ExtensionMethodsClassSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => symbol.TypeKind == TypeKind.Class && symbol.IsStatic && symbol.GetMembers().OfType<IMethodSymbol>().Any(_ => _.IsExtensionMethod);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var phrases = Constants.Comments.ExtensionMethodClassStartingPhrase;

            return summaries.All(_ => !_.StartsWithAny(StringComparison.Ordinal, phrases))
                       ? new[] { ReportIssue(symbol, Constants.XmlTag.Summary, phrases.First()) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}