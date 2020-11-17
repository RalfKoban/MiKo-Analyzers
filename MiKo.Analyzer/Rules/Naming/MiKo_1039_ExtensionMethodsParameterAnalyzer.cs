using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1039_ExtensionMethodsParameterAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1039";

        private const string Value = "value";
        private const string Values = "values";
        private const string Source = "source";

        public MiKo_1039_ExtensionMethodsParameterAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(IParameterSymbol symbol)
        {
            if (symbol.ContainingSymbol is IMethodSymbol method && IsConversionExtension(method))
            {
                return Source;
            }

            return symbol.Type.IsEnumerable() ? Values : Value;
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsExtensionMethod;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol)
        {
            var parameter = symbol.Parameters[0];

            return IsConversionExtension(symbol)
                   ? AnalyzeName(parameter, Source)
                   : AnalyzeName(parameter, Value, Values, Source);
        }

        private static bool IsConversionExtension(IMethodSymbol method)
        {
            if (method.ReturnsVoid)
            {
                return false;
            }

            return IsConversionExtension(method, "To") || IsConversionExtension(method, "From");
        }

        private static bool IsConversionExtension(IMethodSymbol method, string prefix)
        {
            var methodName = method.Name;

            if (methodName.StartsWith(prefix, StringComparison.Ordinal))
            {
                if (methodName.Length == prefix.Length)
                {
                    return true;
                }

                if (methodName.Length > prefix.Length && methodName[prefix.Length].IsUpperCase())
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol parameter, params string[] names) => names.Any(_ => _ == parameter.Name)
                                                                                                              ? Enumerable.Empty<Diagnostic>()
                                                                                                              : new[] { Issue(parameter, names.HumanizedConcatenated()) };
    }
}