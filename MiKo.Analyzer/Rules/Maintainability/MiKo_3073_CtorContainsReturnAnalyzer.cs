﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3073_CtorContainsReturnAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3073";

        public MiKo_3073_CtorContainsReturnAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsConstructor();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol method) => method.DeclaringSyntaxReferences
                                                                                          .SelectMany(_ => _.GetSyntax().DescendantNodes().OfType<ReturnStatementSyntax>())
                                                                                          .Select(_ => Issue(method.Name, _.GetLocation()));
    }
}