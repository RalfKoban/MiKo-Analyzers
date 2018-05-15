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
        private const string Source = "source";

        public MiKo_1039_ExtensionMethodsParameterAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => method.IsExtensionMethod;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var parameter = method.Parameters[0];

            return IsConversionExtension(method)
                   ? AnalyzeName(parameter, Source)
                   : AnalyzeName(parameter, Value, Source);
        }

        private static bool IsConversionExtension(IMethodSymbol method) => !method.ReturnsVoid
                                                                        && method.Name.Length > 2
                                                                        && method.Name[2].IsUpperCase()
                                                                        && method.Name.StartsWith("To", StringComparison.Ordinal);

        private IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol parameter, params string[] names) => names.Any(_ => _ == parameter.Name)
                                                                                                          ? Enumerable.Empty<Diagnostic>()
                                                                                                          : new[] { ReportIssue(parameter, names.HumanizedConcatenated()) };
    }
}