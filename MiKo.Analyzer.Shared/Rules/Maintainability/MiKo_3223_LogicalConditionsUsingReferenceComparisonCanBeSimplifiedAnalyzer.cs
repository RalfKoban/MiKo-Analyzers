using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3223_LogicalConditionsUsingReferenceComparisonCanBeSimplifiedAnalyzer : LogicalConditionsComparisonCanBeSimplifiedMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3223";

        public MiKo_3223_LogicalConditionsUsingReferenceComparisonCanBeSimplifiedAnalyzer() : base(Id)
        {
        }

        protected override bool IsApplicable(IReadOnlyList<ArgumentSyntax> arguments) => arguments.Count == 1;

        protected override bool IsApplicable(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.IsReferenceType)
            {
                // ignore strings as they are fixed by MiKo_3222
                return typeSymbol.IsString() is false;
            }

            return false;
        }
    }
}