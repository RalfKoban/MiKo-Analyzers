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

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.ContainsExtensionMethods();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol) => AnalyzeNamespaceNames(symbol, symbol.ContainingNamespace.ToString());

        private IEnumerable<Diagnostic> AnalyzeNamespaceNames(INamedTypeSymbol symbol, string qualifiedNamespaceOfExtensionMethod)
        {
            // get namespace (qualified) of class and of extension method parameter (first one) and compare those
            var diagnostics = symbol.GetMembers()
                                    .OfType<IMethodSymbol>()
                                    .Where(_ => _.IsExtensionMethod)
                                    .Select(_ => _.Parameters[0].Type.ContainingNamespace.ToString())
                                    .Where(_ => _ != qualifiedNamespaceOfExtensionMethod)
                                    .Select(_ => ReportIssue(symbol, _))
                                    .ToList();
            return diagnostics;
        }
    }
}