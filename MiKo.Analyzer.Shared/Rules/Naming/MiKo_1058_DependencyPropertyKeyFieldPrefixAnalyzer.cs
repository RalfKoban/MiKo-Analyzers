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

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyPropertyKey()
                                                                  && symbol.GetAssignmentsVia(Constants.DependencyProperty.RegisterAttachedReadOnly).None();

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation)
        {
            var propertyNames = NamesFinder.FindPropertyNames(symbol, Constants.DependencyPropertyKey.FieldSuffix, Constants.DependencyProperty.RegisterReadOnly);

            if (propertyNames.None())
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var betterNames = propertyNames.Select(_ => _ + Constants.DependencyPropertyKey.FieldSuffix).ToList();
            var betterName = betterNames[0];

            return new[] { Issue(symbol, betterNames.HumanizedConcatenated(), CreateBetterNameProposal(betterName)) };
        }
    }
}