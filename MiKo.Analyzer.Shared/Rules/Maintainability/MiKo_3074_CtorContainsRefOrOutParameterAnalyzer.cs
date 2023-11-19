﻿using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3074_CtorContainsRefOrOutParameterAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3074";

        public MiKo_3074_CtorContainsRefOrOutParameterAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsConstructor();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            foreach (var parameter in symbol.Parameters)
            {
                if (parameter.RefKind == RefKind.Ref)
                {
                    var keyword = parameter.GetModifier(SyntaxKind.RefKeyword);

                    yield return Issue(keyword);
                }
                else if (parameter.RefKind == RefKind.Out)
                {
                    var keyword = parameter.GetModifier(SyntaxKind.OutKeyword);

                    yield return Issue(keyword);
                }
            }
        }
    }
}
