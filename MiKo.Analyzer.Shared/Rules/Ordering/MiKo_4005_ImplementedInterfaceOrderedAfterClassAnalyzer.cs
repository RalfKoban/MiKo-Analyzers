using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4005_ImplementedInterfaceOrderedAfterClassAnalyzer : OrderingAnalyzer
    {
        public const string Id = "MiKo_4005";

        public MiKo_4005_ImplementedInterfaceOrderedAfterClassAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            var interfaces = symbol.Interfaces;

            if (interfaces.Length > 1)
            {
                var defaultInterfaceName = "I" + symbol.Name;

                if (interfaces.Any(_ => _.Name == defaultInterfaceName && IsAtFirstPosition(symbol, defaultInterfaceName) is false))
                {
                    var type = symbol.GetSyntax() as BaseTypeDeclarationSyntax;
                    var syntax = type?.BaseList?.Types.First(_ => _.Type.GetNameOnlyPartWithoutGeneric() == defaultInterfaceName);

                    if (syntax != null)
                    {
                        return new[] { Issue(symbol.Name, syntax, defaultInterfaceName) };
                    }
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static bool IsAtFirstPosition(INamedTypeSymbol symbol, string defaultInterfaceName)
        {
            // we might have a base type, so check for it
            var baseTypeName = symbol.BaseType?.Name;

            if (baseTypeName is null)
            {
                return false;
            }

            var bases = GetBaseListSyntax(symbol);

            if (bases is null)
            {
                return true;
            }

            // find out the order
            var types = bases.Types.Select(_ => _.Type.GetNameOnlyPartWithoutGeneric()).ToList();

            var index = types.IndexOf(defaultInterfaceName);

            if (index <= 0)
            {
                return true;
            }

            // interface should be first after base type
            return types.Contains(baseTypeName) && index == 1;
        }

        private static BaseListSyntax GetBaseListSyntax(INamedTypeSymbol symbol)
        {
            switch (symbol.GetSyntax())
            {
                case ClassDeclarationSyntax c: return c.BaseList;
                case InterfaceDeclarationSyntax i: return i.BaseList;
                case RecordDeclarationSyntax r: return r.BaseList;
                case StructDeclarationSyntax s: return s.BaseList;
                default: return null;
            }
        }
    }
}