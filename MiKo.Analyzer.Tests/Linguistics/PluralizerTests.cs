using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Linguistics
{
    [TestFixture]
    public sealed class PluralizerTests
    {
        [TestCase("Access", ExpectedResult = "Accesses")]
        [TestCase("Array", ExpectedResult = "Arrays")]
        [TestCase("Child", ExpectedResult = "Children")]
        [TestCase("Children", ExpectedResult = "Children")]
        [TestCase("Data", ExpectedResult = "Data")]
        [TestCase("Datas", ExpectedResult = "Data")]
        [TestCase("Hash", ExpectedResult = "Hashes")]
        [TestCase("Index", ExpectedResult = "Indices")]
        [TestCase("Indices", ExpectedResult = "Indices")]
        [TestCase("Information", ExpectedResult = "Information")]
        [TestCase("Informations", ExpectedResult = "Information")]
        [TestCase("Key", ExpectedResult = "Keys")]
        [TestCase("Keys", ExpectedResult = "Keys")]
        [TestCase("Mech", ExpectedResult = "Mechs")]
        [TestCase("Nested", ExpectedResult = "Nested")]
        [TestCase("Security", ExpectedResult = "Securities")]
        [TestCase("Securitys", ExpectedResult = "Securities")]
        [TestCase("complete", ExpectedResult = "all")]
        [TestCase("bases", ExpectedResult = "items")]
        [TestCase("_bases", ExpectedResult = "_items")]
        [TestCase("m_bases", ExpectedResult = "m_items")]
        [TestCase("sources", ExpectedResult = "source")]
        [TestCase("_sources", ExpectedResult = "_source")]
        [TestCase("m_sources", ExpectedResult = "m_source")]
        [TestCase("namesToConvert", ExpectedResult = "names")]
        [TestCase("itemsToModel", ExpectedResult = "items")]
        [TestCase("itemModels", ExpectedResult = "items")]
        public string Creates_correct_plural_name_(string singularName) => Pluralizer.GetPluralName(singularName);
    }
}