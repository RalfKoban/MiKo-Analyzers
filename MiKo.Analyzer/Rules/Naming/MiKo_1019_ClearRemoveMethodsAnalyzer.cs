using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1019_ClearRemoveMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1019";

        private const string Remove = "Remove";
        private const string Clear = "Clear";

        public MiKo_1019_ClearRemoveMethodsAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(IMethodSymbol method) => method.Parameters.Any()
                                                                           ? method.Name.Replace(Clear, Remove)
                                                                           : method.Name.Replace(Remove, Clear);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod() is false;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol)
        {
            if (symbol.ContainingSymbol is IMethodSymbol method && method.IsTestMethod())
            {
                return false;
            }

            return true;
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            const StringComparison Comparison = StringComparison.Ordinal;

            if (methodName.StartsWith(Clear, Comparison) && methodName.StartsWith(Clear + "s", Comparison) is false)
            {
                return symbol.Parameters.Any()
                       ? new[] { Issue(symbol, methodName.Replace(Clear, Remove)) }
                       : Enumerable.Empty<Diagnostic>();
            }

            if (methodName.StartsWith(Remove, Comparison) && methodName.StartsWith(Remove + "s", Comparison) is false)
            {
                return symbol.Parameters.Any()
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { Issue(symbol, methodName.Replace(Remove, Clear)) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}