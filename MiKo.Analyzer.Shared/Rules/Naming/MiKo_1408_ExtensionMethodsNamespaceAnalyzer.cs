using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1408_ExtensionMethodsNamespaceAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1408";

        public MiKo_1408_ExtensionMethodsNamespaceAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol is INamedTypeSymbol type && type.ContainsExtensionMethods();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => AnalyzeNamespaceNames(symbol, symbol.ContainingNamespace.ToString());

        private IEnumerable<Diagnostic> AnalyzeNamespaceNames(INamedTypeSymbol symbol, string qualifiedNamespaceOfExtensionMethod)
        {
            // get namespace (qualified) of class and of extension method parameter (first one) and compare those
            var diagnostic = symbol.GetExtensionMethods()
                                   .Select(_ => _.Parameters[0].Type.ContainingNamespace)
                                   .WhereNotNull() // generic types do not have a namespace to detect
                                   .Select(_ => _.ToString())
                                   .Where(_ => _ != qualifiedNamespaceOfExtensionMethod)
                                   .Select(_ => Issue(symbol, _))
                                   .FirstOrDefault();

            return diagnostic is null
                   ? Enumerable.Empty<Diagnostic>()
                   : new[] { diagnostic };
        }
    }
}