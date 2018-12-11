using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1058_DependencyPropertyKeyFieldPrefixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1058";

        public MiKo_1058_DependencyPropertyKeyFieldPrefixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyPropertyKey();

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol)
        {
            // find properties
            var propertyNames = symbol.ContainingType.GetMembers().OfType<IPropertySymbol>().Select(_ => _.Name).ToHashSet();

            var symbolName = symbol.Name.WithoutSuffix(Constants.DependencyPropertyKeyFieldSuffix);

            return propertyNames.Contains(symbolName)
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(symbol, propertyNames.Select(_ => _ + Constants.DependencyPropertyKeyFieldSuffix).HumanizedConcatenated()) };
        }
    }
}