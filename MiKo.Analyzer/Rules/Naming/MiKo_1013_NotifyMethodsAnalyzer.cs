﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1013_NotifyMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1013";

        private static readonly string[] StartingPhrases = { "Notify", "OnNotify" };

        public MiKo_1013_NotifyMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method) => method.Name.StartsWithAny(StartingPhrases, StringComparison.Ordinal)
                                                                                            ? new[] { ReportIssue(method) }
                                                                                            : Enumerable.Empty<Diagnostic>();
    }
}