﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2000_MalformedDocumentationAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2000";

        public MiKo_2000_MalformedDocumentationAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            if (symbol.GetDocumentationCommentXml().StartsWith("<!--", StringComparison.OrdinalIgnoreCase))
            {
                yield return Issue(symbol);
            }
        }
    }
}