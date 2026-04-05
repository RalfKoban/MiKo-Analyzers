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
    public sealed class MiKo_1530_DependencyPropertyChangeHandlingMethodNamesAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1530";

        public MiKo_1530_DependencyPropertyChangeHandlingMethodNamesAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsDependencyPropertyChangedCallback();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;
            var betterName = FindBetterName(symbol, methodName);

            if (methodName != betterName)
            {
                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(IMethodSymbol method, string methodName)
        {
            var registeredPropertyName = GetRegisteredPropertyName(method, methodName);

            if (registeredPropertyName.Length > 0)
            {
                return "On" + registeredPropertyName + "Changed";
            }

            return methodName;
        }

        private static string GetRegisteredPropertyName(IMethodSymbol method, string methodName)
        {
            var fields = method.ContainingType.GetFields();

            foreach (var dependencyProperty in fields.Where(_ => _.IsDependencyProperty()))
            {
                var declaration = dependencyProperty.GetSyntax<FieldDeclarationSyntax>();

                if (declaration is null)
                {
                    // we cannot find that out
                    continue;
                }

                foreach (var declarator in declaration.Declaration.Variables)
                {
                    if (declarator.Initializer is EqualsValueClauseSyntax initializer)
                    {
                        var hasMethodAsArgument = initializer.DescendantNodes<IdentifierNameSyntax>().Any(_ => Matches(_, methodName));

                        if (hasMethodAsArgument && initializer.Value is InvocationExpressionSyntax invocation)
                        {
                            var arguments = invocation.ArgumentList.Arguments;

                            if (arguments.Count > 0)
                            {
                                switch (arguments[0].Expression)
                                {
                                    case LiteralExpressionSyntax l when l.IsKind(SyntaxKind.StringLiteralExpression):
                                        return l.Token.ValueText;

                                    case InvocationExpressionSyntax i when i.IsNameOf() && i.ArgumentList.Arguments.FirstOrDefault() is ArgumentSyntax argument:
                                        return argument.GetName();

                                    default:
                                        return string.Empty;
                                }
                            }
                        }
                    }
                }
            }

            return string.Empty;

            bool Matches(IdentifierNameSyntax identifier, string identifierName)
            {
                if (identifier.Parent is ArgumentSyntax a && identifier.GetName() == identifierName)
                {
                    switch (a.Parent?.Parent)
                    {
                        case ObjectCreationExpressionSyntax _:
                        case ImplicitObjectCreationExpressionSyntax _:
                            return true;
                    }
                }

                return false;
            }
        }
    }
}