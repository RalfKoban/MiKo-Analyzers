﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2057_ObjectDisposedExceptionWrongPlacedAnalyzer : ExceptionDocumentationAnalyzer
    {
        public const string Id = "MiKo_2057";

        public MiKo_2057_ObjectDisposedExceptionWrongPlacedAnalyzer() : base(Id, typeof(ObjectDisposedException))
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeException(ISymbol symbol, XmlElementSyntax exceptionComment)
        {
            return symbol.FindContainingType().IsDisposable()
                   ? Array.Empty<Diagnostic>()
                   : new[] { ExceptionIssue(exceptionComment, string.Empty) };
        }
    }
}