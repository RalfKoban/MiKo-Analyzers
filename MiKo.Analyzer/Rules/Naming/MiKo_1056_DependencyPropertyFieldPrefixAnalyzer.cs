using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1056_DependencyPropertyFieldPrefixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1056";

        public MiKo_1056_DependencyPropertyFieldPrefixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyProperty();

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol)
        {
            // find properties
            var propertyNames = symbol.ContainingType.GetMembers().OfType<IPropertySymbol>().Select(_ => _.Name).ToHashSet();

            var symbolName = symbol.Name.WithoutSuffix(Constants.DependencyPropertyFieldSuffix);

            return propertyNames.Contains(symbolName)
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(symbol, propertyNames.Select(_ => _ + Constants.DependencyPropertyFieldSuffix).HumanizedConcatenated()) };
        }
    }
}