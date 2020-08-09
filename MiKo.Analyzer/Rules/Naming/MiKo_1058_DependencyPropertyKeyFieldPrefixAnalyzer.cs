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

        private const string Suffix = Constants.DependencyPropertyKeyFieldSuffix;

        // public static System.Windows.DependencyPropertyKey RegisterReadOnly(string name, Type propertyType, Type ownerType, System.Windows.PropertyMetadata typeMetadata);
        private const string Invocation = Constants.Invocations.DependencyProperty.RegisterReadOnly;

        public MiKo_1058_DependencyPropertyKeyFieldPrefixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        internal static string FindBetterName(IFieldSymbol symbol)
        {
            var propertyName = FindPropertyNames(symbol).First();
            return propertyName + Suffix;
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyPropertyKey()
                                                                  && symbol.GetAssignmentsVia(Constants.Invocations.DependencyProperty.RegisterAttachedReadOnly).None();

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol)
        {
            var propertyNames = FindPropertyNames(symbol);
            if (propertyNames.Any())
            {
                return new[] { Issue(symbol, propertyNames.Select(_ => _ + Suffix).HumanizedConcatenated()) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static IEnumerable<string> FindPropertyNames(IFieldSymbol symbol) => NamesFinder.FindPropertyNames(symbol, Suffix, Invocation);
    }
}