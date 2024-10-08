using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3224_LogicalConditionsUsingValueComparisonCanBeSimplifiedAnalyzer : LogicalConditionsComparisonCanBeSimplifiedMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3224";

        public MiKo_3224_LogicalConditionsUsingValueComparisonCanBeSimplifiedAnalyzer() : base(Id)
        {
        }

        protected override bool IsApplicable(IReadOnlyList<ArgumentSyntax> arguments) => arguments.Count == 1;

        protected override bool IsApplicable(ITypeSymbol typeSymbol) => typeSymbol.IsValueType;
    }
}