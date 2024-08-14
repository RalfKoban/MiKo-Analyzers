using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1078_BuilderMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1078";

        public MiKo_1078_BuilderMethodsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.Name.EndsWith("Builder", StringComparison.Ordinal);

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (symbol.IsConstructor())
            {
                return false;
            }

            // ignore private methods or those that do not have any accessibility (means they are also private)
            return symbol.IsPubliclyVisible() && base.ShallAnalyze(symbol);
        }

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => false; // do not consider local functions at all

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => symbol.GetNamedMethods().SelectMany(_ => AnalyzeMethod(_, compilation));

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.StartsWith(Constants.Names.Create, StringComparison.Ordinal))
            {
                var betterName = FindBetterName(symbolName);

                return new[] { Issue(symbol, CreateBetterNameProposal(betterName)) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static string FindBetterName(string name) => name.Replace(Constants.Names.Create, "Build");
    }
}