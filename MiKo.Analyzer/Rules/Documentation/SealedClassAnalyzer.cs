using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class SealedClassAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2001";
        private const string ExpectedComment = "This class cannot be inherited.";

        public SealedClassAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeClass(INamedTypeSymbol symbol) => symbol.IsReferenceType;

        protected override IEnumerable<Diagnostic> AnalyzeClass(INamedTypeSymbol symbol, string commentXml) => AnalyzeSummary(symbol, commentXml);

        protected override  IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var containsComment = summaries.Any(_ => _.EndsWith(ExpectedComment, StringComparison.Ordinal));

            if (symbol.IsSealed && !containsComment) return new[] { Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name, ExpectedComment) };

            // TODO: RKN find out how to set the localized text to indicate that the comment shall not be part of the summary
            // if (!symbol.IsSealed && containsComment) return new[] { Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name, ExpectedComment) };

            return Enumerable.Empty<Diagnostic>();
        }
    }
}