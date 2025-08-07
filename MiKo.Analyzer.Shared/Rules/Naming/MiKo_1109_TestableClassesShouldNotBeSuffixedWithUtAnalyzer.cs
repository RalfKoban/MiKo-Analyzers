using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1109_TestableClassesShouldNotBeSuffixedWithUtAnalyzer : TypeSyntaxNamingAnalyzer
    {
        public const string Id = "MiKo_1109";

        private const string Prefix = "Testable";
        private const string Suffix = "Ut";

        public MiKo_1109_TestableClassesShouldNotBeSuffixedWithUtAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override Diagnostic[] AnalyzeName(string typeName, in SyntaxToken typeNameIdentifier, BaseTypeDeclarationSyntax declaration)
        {
            var className = typeName.AsSpan();

            if (className.EndsWith(Suffix, StringComparison.Ordinal))
            {
                var betterName = FindBetterName(className);

                return new[] { Issue(typeNameIdentifier, betterName) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(in ReadOnlySpan<char> symbolName) => symbolName.StartsWith(Prefix, StringComparison.Ordinal)
                                                                                  ? symbolName.WithoutSuffix(Suffix).ToString()
                                                                                  : Prefix.ConcatenatedWith(symbolName.WithoutSuffix(Suffix));
    }
}