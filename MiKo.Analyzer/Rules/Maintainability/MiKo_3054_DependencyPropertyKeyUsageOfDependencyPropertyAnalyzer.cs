using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3054_DependencyPropertyKeyUsageOfDependencyPropertyAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3054";

        public MiKo_3054_DependencyPropertyKeyUsageOfDependencyPropertyAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyPropertyKey();

        protected override IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol)
        {
            var invocation = symbol.Name + ".DependencyProperty";

            // get fields in same class that are of Type "DependencyProperty" and find out if any gets assigned to the "DependencyProperty" of the "DependencyPropertyKey"
            // hint: names should match
            var owingType = symbol.FindContainingType();

            var dependencyProperties = owingType.GetMembers().OfType<IFieldSymbol>().Where(_ => _.Type.IsDependencyProperty());
            return dependencyProperties.Any(_ => _.GetAssignmentsVia(invocation).Any())
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(symbol) };
        }
    }
}