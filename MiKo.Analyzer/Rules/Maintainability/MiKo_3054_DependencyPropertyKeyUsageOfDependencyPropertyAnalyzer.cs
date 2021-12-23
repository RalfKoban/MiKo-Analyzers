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

        protected override IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol, Compilation compilation)
        {
            var invocation = symbol.Name + "." + Constants.DependencyPropertyKey.DependencyProperty;

            // get fields in same class that are of Type "DependencyProperty" and find out if any gets assigned to the "DependencyProperty" of the "DependencyPropertyKey"
            // hint: names should match
            var owingType = symbol.FindContainingType();

            var dependencyProperties = owingType.GetFields().Where(_ => _.Type.IsDependencyProperty());
            var noneAssigned = dependencyProperties.None(_ => _.GetAssignmentsVia(invocation).Any());
            if (noneAssigned)
            {
                yield return Issue(symbol);
            }
        }
    }
}