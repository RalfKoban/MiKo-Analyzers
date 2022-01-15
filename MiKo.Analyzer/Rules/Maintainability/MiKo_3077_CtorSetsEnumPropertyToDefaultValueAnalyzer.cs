using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3077_CtorSetsEnumPropertyToDefaultValueAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3077";

        public MiKo_3077_CtorSetsEnumPropertyToDefaultValueAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsConstructor() && symbol.IsSerializationConstructor() is false;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var propertiesWithoutInitializer = GetUnassignedEnumProperties(symbol);
            if (propertiesWithoutInitializer.Any())
            {
                yield return Issue(symbol, propertiesWithoutInitializer.Select(_ => _.Name).HumanizedConcatenated("and"));
            }
        }

        private static IEnumerable<IPropertySymbol> GetUnassignedEnumProperties(ISymbol symbol)
        {
            var propertiesWithoutInitializer = symbol.ContainingType.GetProperties()
                                                     .Where(_ => _.GetReturnType().IsEnum())
                                                     .Where(_ => _.GetSyntax<PropertyDeclarationSyntax>().Initializer is null)
                                                     .ToList();

            if (propertiesWithoutInitializer.Any())
            {
                var identifierNames = symbol.GetSyntax()
                                            .DescendantNodes()
                                            .Where(_ => _.IsKind(SyntaxKind.SimpleAssignmentExpression))
                                            .Cast<AssignmentExpressionSyntax>()
                                            .Select(_ => _.Left)
                                            .OfType<IdentifierNameSyntax>()
                                            .Select(_ => _.GetName())
                                            .ToHashSet();

                propertiesWithoutInitializer.RemoveAll(_ => identifierNames.Contains(_.Name));
            }

            return propertiesWithoutInitializer;
        }
    }
}