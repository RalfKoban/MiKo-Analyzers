using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3222_LogicalConditionsUsingStringComparisonCanBeSimplifiedAnalyzer : LogicalConditionsComparisonCanBeSimplifiedMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3222";

        public MiKo_3222_LogicalConditionsUsingStringComparisonCanBeSimplifiedAnalyzer() : base(Id)
        {
        }

        protected override bool IsApplicable(IReadOnlyList<ArgumentSyntax> arguments) => arguments.Count > 0;

        protected override bool IsApplicable(ITypeSymbol typeSymbol) => typeSymbol.IsString();
    }
}