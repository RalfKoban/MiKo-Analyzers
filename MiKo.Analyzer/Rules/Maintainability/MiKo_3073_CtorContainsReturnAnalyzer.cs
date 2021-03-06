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

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol)
        {
            var methodName = symbol.Name;

            return symbol.GetSyntax()
                         .DescendantNodes()
                         .OfType<ReturnStatementSyntax>()
                         .Where(_ => _.Ancestors().OfType<ParenthesizedLambdaExpressionSyntax>().None()) // filter callbacks inside constructors
                         .Select(_ => Issue(methodName, _));
        }
    }
}