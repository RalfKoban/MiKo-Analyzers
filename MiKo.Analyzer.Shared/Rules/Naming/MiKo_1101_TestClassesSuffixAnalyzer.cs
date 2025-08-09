using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1101_TestClassesSuffixAnalyzer : TypeSyntaxNamingAnalyzer
    {
        public const string Id = "MiKo_1101";

        public MiKo_1101_TestClassesSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(string typeName, BaseTypeDeclarationSyntax declaration) => declaration.IsTestClass();

        protected override Diagnostic[] AnalyzeName(string typeName, in SyntaxToken typeNameIdentifier, BaseTypeDeclarationSyntax declaration)
        {
            var className = typeName.AsSpan();

            if (className.EndsWith(Constants.TestsSuffix))
            {
                return Array.Empty<Diagnostic>();
            }

            var name = FindBetterName(className);

            return new[] { Issue(typeNameIdentifier, name) };
        }

        private static string FindBetterName(in ReadOnlySpan<char> className)
        {
            if (className.EndsWith("Test"))
            {
                return className.ConcatenatedWith('s');
            }

            if (className.EndsWith("TestBase"))
            {
                return className.Slice(0, className.Length - 4).ConcatenatedWith('s');
            }

            if (className.EndsWith("TestsBase"))
            {
                return className.Slice(0, className.Length - 4).ToString();
            }

            return className.ConcatenatedWith(Constants.TestsSuffix);
        }
    }
}