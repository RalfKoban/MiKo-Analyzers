using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    internal static class NamesFinder
    {
        internal static IEnumerable<string> FindPropertyNames(IFieldSymbol symbol, string unwantedSuffix, string invocation)
        {
            // find properties
            var propertyNames = symbol.ContainingType.GetMembers().OfType<IPropertySymbol>().Select(_ => _.Name).ToHashSet();

            // there might be none available; in such case don't report anything
            if (propertyNames.None())
            {
                return Enumerable.Empty<string>();
            }

            var symbolName = symbol.Name.WithoutSuffix(unwantedSuffix);

            // analyze correct name (must match string literal or nameof)
            var registeredName = GetRegisteredName(symbol, invocation);
            if (registeredName is null)
            {
                if (propertyNames.Contains(symbolName))
                {
                    return Enumerable.Empty<string>();
                }
            }
            else
            {
                if (registeredName == symbolName)
                {
                    return Enumerable.Empty<string>();
                }

                propertyNames.Clear();
                propertyNames.Add(registeredName);
            }

            return propertyNames;
        }

        private static string GetRegisteredName(IFieldSymbol symbol, string invocation)
        {
            var arguments = symbol.GetInvocationArgumentsFrom(invocation);
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