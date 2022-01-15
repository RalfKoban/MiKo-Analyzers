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
            var unassignedEnumProperties = new List<IPropertySymbol>();

            foreach (var property in symbol.ContainingType.GetProperties().Where(_ => _.GetReturnType().IsEnum()))
            {
                var syntax = property.GetSyntax<PropertyDeclarationSyntax>();

                if (syntax.Initializer != null)
                {
                    // ignore initializers
                    continue;
                }

                if (property.IsReadOnly)
                {
                    ExpressionSyntax expression = null;

                    if (syntax.ExpressionBody != null)
                    {
                        expression = syntax.ExpressionBody.Expression;
                    }
                    else
                    {
                        var accessorList = syntax.AccessorList;
                        if (accessorList != null)
                        {
                            var getter = accessorList.Accessors[0];

                            if (getter.ExpressionBody != null)
                            {
                                expression = getter.ExpressionBody.Expression;
                            }
                            else
                            {
                                if (getter.Body?.Statements.FirstOrDefault() is ReturnStatementSyntax r)
                                {
                                    expression = r.Expression;
                                }
                            }
                        }
                    }

                    switch (expression)
                    {
                        case InvocationExpressionSyntax _: // ignore properties that simply return a fixed result
                        case MemberAccessExpressionSyntax _: // ignore properties that simply return a calculated result
                            continue;
                    }
                }

                unassignedEnumProperties.Add(property);
            }

            if (unassignedEnumProperties.Any())
            {
                // ignore all properties that get assigned
                var identifierNames = symbol.GetSyntax()
                                            .DescendantNodes()
                                            .Where(_ => _.IsKind(SyntaxKind.SimpleAssignmentExpression))
                                            .Cast<AssignmentExpressionSyntax>()
                                            .Select(_ => _.Left)
                                            .OfType<IdentifierNameSyntax>()
                                            .Select(_ => _.GetName())
                                            .ToHashSet();

                unassignedEnumProperties.RemoveAll(_ => identifierNames.Contains(_.Name));
            }

            return unassignedEnumProperties;
        }
    }
}