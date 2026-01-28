using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1119_TestMethodsWhenPresentAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1119";

        private static readonly string[] ProblematicTexts =
                                                            {
                                                                "if_present",
                                                                "if_not_present",
                                                                "when_present",
                                                                "when_not_present",
                                                                "IfPresent",
                                                                "IfNotPresent",
                                                                "If_Present",
                                                                "If_Not_Present",
                                                                "If_NotPresent",
                                                                "WhenPresent",
                                                                "WhenNotPresent",
                                                                "When_Present",
                                                                "When_Not_Present",
                                                                "When_NotPresent",
                                                            };

        public MiKo_1119_TestMethodsWhenPresentAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name
                                   .Replace("resentation", "###") // ignore 'presentation'
                                   .Replace("resenting", "###");  // ignore 'presenting'

            foreach (var text in ProblematicTexts)
            {
                if (symbolName.Contains(text, StringComparison.Ordinal))
                {
                    return new[] { Issue(symbol, text) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}