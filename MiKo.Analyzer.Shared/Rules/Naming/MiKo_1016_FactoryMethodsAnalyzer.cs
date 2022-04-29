using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1016_FactoryMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1016";

        internal const string Prefix = "Create";

        public MiKo_1016_FactoryMethodsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        internal static string FindBetterName(IMethodSymbol symbol)
        {
            var returnTypeName = symbol.ReturnType.Name;

            if (symbol.ContainingType.Name == returnTypeName + "Factory")
            {
                // we have a factory that has the name of the type to return, so the method shall be 'Create' only
                return Prefix;
            }

            if (symbol.ReturnType.IsGeneric())
            {
                // we have a generic type, so we do not know an exact name
                return Prefix;
            }

            // we have a concrete type but a multiple-types factory
            return Prefix + returnTypeName;
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsFactory();

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (symbol.IsConstructor())
            {
                return false;
            }

            // ignore private methods or those that do not have any accessibility (means they are also private)
            return symbol.IsPubliclyVisible() && base.ShallAnalyze(symbol);
        }

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => false;

        protected override IEnumerable<Diagnostic> AnalyzeLocalFunctions(IMethodSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>(); // don't consider local functions at all

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => symbol.GetNamedMethods().SelectMany(_ => AnalyzeMethod(_, compilation));

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.Name.StartsWith(Prefix, StringComparison.Ordinal) is false)
            {
                yield return Issue(symbol);
            }
        }
    }
}