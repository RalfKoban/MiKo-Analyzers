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
        private const string Format = "format";

        public MiKo_1039_ExtensionMethodsParameterAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(IParameterSymbol symbol)
        {
            if (symbol.ContainingSymbol is IMethodSymbol method && IsConversionExtension(method))
            {
                return Source;
            }

            if (IsStringFormatExtension(symbol) && symbol.Type.SpecialType == SpecialType.System_String)
            {
                return Format;
            }

            return symbol.Type.IsEnumerable() ? Values : Value;
        }

        internal static bool IsStringFormatExtension(IParameterSymbol symbol) => symbol.ContainingSymbol is IMethodSymbol method && IsStringFormatExtension(method);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsExtensionMethod;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var parameter = symbol.Parameters[0];

            if (IsConversionExtension(symbol))
            {
                return AnalyzeName(parameter, Source);
            }

            if (IsStringFormatExtension(symbol))
            {
                return AnalyzeName(parameter, Format);
            }

            return AnalyzeName(parameter, Value, Values, Source);
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

        private static bool IsStringFormatExtension(IMethodSymbol method) => method.ReturnType.SpecialType == SpecialType.System_String && method.Name.StartsWith("Format", StringComparison.Ordinal);

        private IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol parameter, params string[] names) => names.Any(_ => _ == parameter.Name)
                                                                                                          ? Enumerable.Empty<Diagnostic>()
                                                                                                          : new[] { Issue(parameter, names.HumanizedConcatenated()) };
    }
}