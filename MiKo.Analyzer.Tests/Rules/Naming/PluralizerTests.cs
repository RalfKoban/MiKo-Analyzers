using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class PluralizerTests
    {
        [TestCase("Access", ExpectedResult = "Accesses")]
        [TestCase("Child", ExpectedResult = "Children")]
        [TestCase("Children", ExpectedResult = "Children")]
        [TestCase("Data", ExpectedResult = "Data")]
        [TestCase("Datas", ExpectedResult = "Data")]
        [TestCase("Hash", ExpectedResult = "Hashes")]
        [TestCase("Index", ExpectedResult = "Indices")]
        [TestCase("Indices", ExpectedResult = "Indices")]
        [TestCase("Information", ExpectedResult = "Information")]
        [TestCase("Key", ExpectedResult = "Keys")]
        [TestCase("Keys", ExpectedResult = "Keys")]
        [TestCase("Mech", ExpectedResult = "Mechs")]
        [TestCase("Nested", ExpectedResult = "Nested")]
        [TestCase("Security", ExpectedResult = "Securities")]
        [TestCase("Securitys", ExpectedResult = "Securities")]
        public string Creates_correct_plural_name_(string singularName) => Pluralizer.GetPluralName(singularName);
    }
}