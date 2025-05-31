﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3024_RefParameterAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3024";

        public MiKo_3024_RefParameterAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            foreach (var parameter in symbol.Parameters.Where(_ => _.RefKind is RefKind.Ref && _.Type.TypeKind != TypeKind.Struct))
            {
                var keyword = parameter.GetModifier(SyntaxKind.RefKeyword);

                yield return Issue(parameter.Name, keyword);
            }
        }
    }
}