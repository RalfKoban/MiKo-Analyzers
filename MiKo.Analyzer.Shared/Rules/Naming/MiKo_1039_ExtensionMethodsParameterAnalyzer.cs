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

        internal static bool IsStringFormatExtension(IParameterSymbol symbol) => symbol?.ContainingSymbol is IMethodSymbol method && IsStringFormatExtension(method);

        protected override bool ShallAnalyze(IMethodSymbol symbol, Compilation compilation)
        {
#if VS2022 || VS2026
            // report only once per extension type as all methods of that extension share the same parameter
            if (symbol.ContainingSymbol is INamedTypeSymbol type && type.IsExtension)
            {
                // so use the first method to report on
                var methodToReportOn = type.GetMethods(MethodKind.Ordinary).FirstOrDefault();

                return symbol.Equals(methodToReportOn, SymbolEqualityComparer.Default);
            }
#endif

            return symbol.IsExtensionMethod;
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var parameter = symbol.GetExtensionMethodParameter();

            if (IsExtensionType(parameter))
            {
                return AnalyzeName(symbol, parameter, Value, Values);
            }

            if (IsConversionExtension(symbol))
            {
                return AnalyzeName(symbol, parameter, Source);
            }

            if (IsStringFormatExtension(symbol))
            {
                return AnalyzeName(symbol, parameter, Format);
            }

            return AnalyzeName(symbol, parameter, Value, Values, Source);
        }

        private static bool IsExtensionType(IParameterSymbol symbol)
        {
#if VS2022 || VS2026
            return symbol.ContainingSymbol is INamedTypeSymbol type && type.IsExtension;
#else
            return false;
#endif
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
            var prefixLength = prefix.Length;

            if (methodName.StartsWith(prefix, StringComparison.Ordinal))
            {
                if (methodName.Length == prefixLength)
                {
                    return true;
                }

                if (methodName.Length > prefixLength && methodName[prefixLength].IsUpperCaseOrNumber())
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsStringFormatExtension(IMethodSymbol method) => method.ReturnType.SpecialType is SpecialType.System_String && method.Name.StartsWith(nameof(string.Format), StringComparison.Ordinal);

        private static string FindBetterName(IMethodSymbol method, IParameterSymbol symbol)
        {
            if (IsExtensionType(symbol) is false)
            {
                // adjust names only in case we have no dedicated "new" extension block, as in such case the parameter is valid for each of the members defined in the extension block
                if (IsConversionExtension(method))
                {
                    return Source;
                }

                if (IsStringFormatExtension(symbol) && symbol.Type.SpecialType is SpecialType.System_String)
                {
                    return Format;
                }
            }

            return symbol.Type.IsEnumerable() ? Values : Value;
        }

        private Diagnostic[] AnalyzeName(IMethodSymbol method, IParameterSymbol parameter, params string[] names)
        {
            var parameterName = parameter.Name;

            if (parameterName.IsNullOrEmpty() || names.Any(_ => _ == parameterName))
            {
                return Array.Empty<Diagnostic>();
            }

            var proposal = FindBetterName(method, parameter);

            return new[] { Issue(parameter, names.HumanizedConcatenated(), CreateBetterNameProposal(proposal)) };
        }
    }
}