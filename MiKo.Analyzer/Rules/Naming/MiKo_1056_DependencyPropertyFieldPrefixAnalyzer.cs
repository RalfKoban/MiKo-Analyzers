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

        private const string Suffix = Constants.DependencyProperty.FieldSuffix;

        // public static System.Windows.DependencyProperty Register(string name, Type propertyType, Type ownerType, System.Windows.PropertyMetadata typeMetadata);
        private const string Invocation = Constants.DependencyProperty.Register;

        public MiKo_1056_DependencyPropertyFieldPrefixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        internal static string FindBetterName(IFieldSymbol symbol)
        {
            var propertyName = FindPropertyNames(symbol).First();

            return propertyName + Suffix;
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol)
        {
            if (symbol.Type.IsDependencyProperty() is false)
            {
                return false;
            }

            // ignore attached properties
            var attachedProperties = symbol.GetAssignmentsVia(Constants.DependencyProperty.RegisterAttached).Any();
            if (attachedProperties)
            {
                return false;
            }

            // ignore "Key.DependencyProperty" assignments
            var keys = symbol.ContainingType.GetMembers().OfType<IFieldSymbol>().Where(_ => _.Type.IsDependencyPropertyKey()).Select(_ => _.Name + "." + Constants.DependencyPropertyKey.DependencyProperty).ToHashSet();

            foreach (var key in keys)
            {
                if (symbol.GetAssignmentsVia(key).Any())
                {
                    return false;
                }
            }

            return true;
        }

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