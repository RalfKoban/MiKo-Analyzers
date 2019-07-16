using Microsoft.CodeAnalysis;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class NamingAnalyzerTests
    {
        [TestCase("Access", ExpectedResult = "Accesses")]
        [TestCase("Child", ExpectedResult = "Children")]
        [TestCase("Children", ExpectedResult = "Children")]
        [TestCase("Data", ExpectedResult = "Data")]
        [TestCase("Datas", ExpectedResult = "Data")]
        [TestCase("Index", ExpectedResult = "Indices")]
        [TestCase("Indices", ExpectedResult = "Indices")]
        [TestCase("Information", ExpectedResult = "Information")]
        [TestCase("Key", ExpectedResult = "Keys")]
        [TestCase("Keys", ExpectedResult = "Keys")]
        [TestCase("Nested", ExpectedResult = "Nested")]
        [TestCase("Security", ExpectedResult = "Securities")]
        [TestCase("Securitys", ExpectedResult = "Securities")]
        public string Creates_correct_plural_name(string singularName) => TestableNamingAnalyzer.GetPluralName(singularName);

        private sealed class TestableNamingAnalyzer : NamingAnalyzer
        {
            public TestableNamingAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.Method) : base(diagnosticId, kind)
            {
            }

            public static string GetPluralName(string singularName) => NamingAnalyzer.GetPluralName(singularName, singularName);
        }
    }
}