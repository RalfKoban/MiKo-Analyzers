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
            var keys = symbol.ContainingType.GetFields().Where(_ => _.Type.IsDependencyPropertyKey()).ToHashSet(_ => _.Name + "." + Constants.DependencyPropertyKey.DependencyProperty);

            foreach (var key in keys)
            {
                if (symbol.GetAssignmentsVia(key).Any())
                {
                    return false;
                }
            }

            return true;
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation)
        {
            var propertyNames = NamesFinder.FindPropertyNames(symbol, Constants.DependencyProperty.FieldSuffix, Constants.DependencyProperty.Register);

            if (propertyNames.None())
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var betterNames = propertyNames.Select(_ => _ + Constants.DependencyProperty.FieldSuffix).ToList();
            var betterName = betterNames[0];

            return new[] { Issue(symbol, betterNames.HumanizedConcatenated(), CreateBetterNameProposal(betterName)) };
        }
    }
}