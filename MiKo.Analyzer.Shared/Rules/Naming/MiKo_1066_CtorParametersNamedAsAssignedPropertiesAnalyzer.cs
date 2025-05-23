﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1066_CtorParametersNamedAsAssignedPropertiesAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1066";

        public MiKo_1066_CtorParametersNamedAsAssignedPropertiesAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsConstructor() && symbol.Parameters.Any();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var syntax = symbol.GetSyntax<ConstructorDeclarationSyntax>();

            // having 'null' as syntax may happen if we have a primary constructor on a record
            if (syntax != null)
            {
                var propertyNames = symbol.ContainingType.GetProperties().ToHashSet(_ => _.Name);

                if (propertyNames.Count != 0)
                {
                    var parameterNames = symbol.Parameters.ToDictionary(_ => _.Name, _ => _);

                    var assignments = syntax.DescendantNodes<AssignmentExpressionSyntax>(SyntaxKind.SimpleAssignmentExpression);

                    foreach (var assignment in assignments)
                    {
                        if (assignment.Left is IdentifierNameSyntax left && assignment.Right is IdentifierNameSyntax right)
                        {
                            var propertyName = left.GetName();

                            if (propertyNames.Contains(propertyName))
                            {
                                var parameterName = right.GetName();

                                if (parameterNames.TryGetValue(parameterName, out var parameter))
                                {
                                    if (propertyName.Equals(parameterName, StringComparison.OrdinalIgnoreCase) is false)
                                    {
                                        // we found a property that gets assigned by a parameter with a wrong name
                                        var betterName = propertyName.ToLowerCaseAt(0);

                                        yield return Issue(parameter, CreateBetterNameProposal(betterName));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}