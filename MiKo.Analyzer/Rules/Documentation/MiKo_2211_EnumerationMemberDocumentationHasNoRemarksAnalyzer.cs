﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2211_EnumerationMemberDocumentationHasNoRemarksAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2211";

        public MiKo_2211_EnumerationMemberDocumentationHasNoRemarksAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyzeField(IFieldSymbol symbol) => symbol.ContainingType.IsEnum();

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, string commentXml)
        {
            var comments = symbol.GetRemarks();

            return comments.Any()
                       ? new[] { Issue(symbol) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}