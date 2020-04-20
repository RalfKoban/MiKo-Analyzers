﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4004_ImplementedInterfaceOrderedAfterClassAnalyzer : OrderingAnalyzer
    {
        public const string Id = "MiKo_4004";

        public MiKo_4004_ImplementedInterfaceOrderedAfterClassAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            var interfaces = symbol.Interfaces;

            if (interfaces.Length > 1)
            {
                var defaultInterfaceName = "I" + symbol.Name;

                foreach (var i in interfaces)
                {
                    if (i.Name == defaultInterfaceName)
                    {
                        if (IsAtFirstPosition(symbol, defaultInterfaceName) is false)
                        {
                            yield return Issue(symbol, defaultInterfaceName);
                        }

                        yield break;
                    }
                }
            }
        }

        private static bool IsAtFirstPosition(INamedTypeSymbol symbol, string defaultInterfaceName)
        {
            var bases = GetBaseListSyntax(symbol);
            if (bases is null)
            {
                return true;
            }

            // find out the order
            var types = bases.Types;

            var index = types.IndexOf(_ => _.Type.GetNameOnlyPart() == defaultInterfaceName);
            if (index <= 0)
            {
                return true;
            }

            // we might have a base type, so check for it
            if (symbol.BaseType is null)
            {
                return false;
            }

            var hasBaseTypeDefined = types.Any(_ => _.Type.GetNameOnlyPart() == symbol.BaseType.Name);

            // interface should be first after base type
            return hasBaseTypeDefined && index == 1;
        }

        private static BaseListSyntax GetBaseListSyntax(INamedTypeSymbol symbol)
        {
            switch (symbol.GetSyntax())
            {
                case ClassDeclarationSyntax c: return c.BaseList;
                case StructDeclarationSyntax s: return s.BaseList;
                default: return null;
            }
        }
    }
}