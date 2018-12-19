using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyPropertyKey()
                                                                 && !symbol.GetAssignmentsVia(Constants.Invocations.DependencyProperty.RegisterAttachedReadOnly).Any();

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol)
        {
            // find properties
            var propertyNames = symbol.ContainingType.GetMembers().OfType<IPropertySymbol>().Select(_ => _.Name).ToHashSet();

            // there might be none available; in such case don't report anything
            if (!propertyNames.Any())
                return Enumerable.Empty<Diagnostic>();

            var symbolName = symbol.Name.WithoutSuffix(Suffix);

            // analyze correct name (must match string literal or nameof)
            var registeredName = GetRegisteredName(symbol);
            if (registeredName is null)
            {
                if (propertyNames.Contains(symbolName))
                    return Enumerable.Empty<Diagnostic>();
            }
            else
            {
                if (registeredName == symbolName)
                    return Enumerable.Empty<Diagnostic>();

                propertyNames.Clear();
                propertyNames.Add(registeredName + Suffix);
            }

            return new[] { ReportIssue(symbol, propertyNames.Select(_ => _ + Suffix).HumanizedConcatenated()) };
        }

        private static string GetRegisteredName(IFieldSymbol symbol)
        {
            var arguments = symbol.GetInvocationArgumentsFrom(Invocation);
            if (arguments.Count > 0)
            {
                switch (arguments[0].Expression)
                {
                    case LiteralExpressionSyntax s:
                        return s.Token.ValueText;
                    case InvocationExpressionSyntax s:
                        return s.ArgumentList.Arguments.FirstOrDefault()?.ToString();
                }
            }

            return null;
        }
    }
}