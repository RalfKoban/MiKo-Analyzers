using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Linguistics
{
    [TestFixture]
    public static class ArticleProviderTests
    {
        [TestCase("Apple", ExpectedResult = "An ")]
        [TestCase("apple", ExpectedResult = "An ")]
        [TestCase("Eagle", ExpectedResult = "An ")]
        [TestCase("eagle", ExpectedResult = "An ")]
        [TestCase("Intermediate", ExpectedResult = "An ")]
        [TestCase("intermediate", ExpectedResult = "An ")]
        [TestCase("Observation", ExpectedResult = "An ")]
        [TestCase("observation", ExpectedResult = "An ")]
        [TestCase("Honor", ExpectedResult = "An ")]
        [TestCase("honor", ExpectedResult = "An ")]
        [TestCase("Honest", ExpectedResult = "An ")]
        [TestCase("honest", ExpectedResult = "An ")]
        [TestCase("Hummingbird", ExpectedResult = "A ")]
        [TestCase("hummingbird", ExpectedResult = "A ")]
        [TestCase("Umbrella", ExpectedResult = "An ")]
        [TestCase("umbrella", ExpectedResult = "An ")]
        [TestCase("Uni", ExpectedResult = "A ")]
        [TestCase("uni", ExpectedResult = "A ")]
        [TestCase("University", ExpectedResult = "A ")]
        [TestCase("university", ExpectedResult = "A ")]
        [TestCase("Uniform", ExpectedResult = "A ")]
        [TestCase("uniform", ExpectedResult = "A ")]
        [TestCase("Uninformed", ExpectedResult = "An ")]
        [TestCase("uninformed", ExpectedResult = "An ")]
        [TestCase("State", ExpectedResult = "A ")]
        [TestCase("state", ExpectedResult = "A ")]
        public static string Provides_correct_indefinite_article_for_(string text) => ArticleProvider.GetIndefiniteArticleFor(text);
    }
}