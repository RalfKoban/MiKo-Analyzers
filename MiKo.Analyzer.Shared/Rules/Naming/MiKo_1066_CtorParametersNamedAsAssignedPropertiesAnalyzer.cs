using System;
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

        private const string BetterName = "BetterName";

        public MiKo_1066_CtorParametersNamedAsAssignedPropertiesAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(IParameterSymbol symbol, Diagnostic diagnostic) => diagnostic.Properties[BetterName];

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.MethodKind == MethodKind.Constructor && symbol.Parameters.Any();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var propertyNames = symbol.ContainingType.GetProperties().ToHashSet(_ => _.Name);

            if (propertyNames.Any())
            {
                var parameterNames = symbol.Parameters.ToDictionary(_ => _.Name, _ => _);

                var syntax = symbol.GetSyntax<ConstructorDeclarationSyntax>();
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
                                if (string.Equals(propertyName, parameterName, StringComparison.OrdinalIgnoreCase) is false)
                                {
                                    // we found a property that gets assigned by a parameter with a wrong name
                                    var betterName = propertyName.ToLowerCaseAt(0);

                                    yield return Issue(parameter, new Dictionary<string, string> { { BetterName, betterName } });
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}