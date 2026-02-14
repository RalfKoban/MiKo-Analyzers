using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Linguistics
{
    [TestFixture]
    public static class ArticleProviderTests
    {
        [TestCase("apple", ExpectedResult = "An ")]
        [TestCase("Apple", ExpectedResult = "An ")]
        [TestCase("eagle", ExpectedResult = "An ")]
        [TestCase("Eagle", ExpectedResult = "An ")]
        [TestCase("european", ExpectedResult = "A ")]
        [TestCase("European", ExpectedResult = "A ")]
        [TestCase("heir", ExpectedResult = "An ")]
        [TestCase("Heir", ExpectedResult = "An ")]
        [TestCase("hero", ExpectedResult = "A ")]
        [TestCase("Hero", ExpectedResult = "A ")]
        [TestCase("herb", ExpectedResult = "An ")] // US English
        [TestCase("Herb", ExpectedResult = "An ")] // US English
        [TestCase("honest", ExpectedResult = "An ")]
        [TestCase("Honest", ExpectedResult = "An ")]
        [TestCase("honor", ExpectedResult = "An ")]
        [TestCase("Honor", ExpectedResult = "An ")]
        [TestCase("hour", ExpectedResult = "An ")]
        [TestCase("Hour", ExpectedResult = "An ")]
        [TestCase("hummingbird", ExpectedResult = "A ")]
        [TestCase("Hummingbird", ExpectedResult = "A ")]
        [TestCase("intermediate", ExpectedResult = "An ")]
        [TestCase("Intermediate", ExpectedResult = "An ")]
        [TestCase("observation", ExpectedResult = "An ")]
        [TestCase("Observation", ExpectedResult = "An ")]
        [TestCase("occupation", ExpectedResult = "An ")]
        [TestCase("Occupation", ExpectedResult = "An ")]
        [TestCase("offset", ExpectedResult = "An ")]
        [TestCase("Offset", ExpectedResult = "An ")]
        [TestCase("once", ExpectedResult = "A ")]
        [TestCase("Once", ExpectedResult = "A ")]
        [TestCase("one", ExpectedResult = "A ")]
        [TestCase("One", ExpectedResult = "A ")]
        [TestCase("state", ExpectedResult = "A ")]
        [TestCase("State", ExpectedResult = "A ")]
        [TestCase("umbrella", ExpectedResult = "An ")]
        [TestCase("Umbrella", ExpectedResult = "An ")]
        [TestCase("uni", ExpectedResult = "A ")]
        [TestCase("Uni", ExpectedResult = "A ")]
        [TestCase("unicorn", ExpectedResult = "A ")]
        [TestCase("Unicorn", ExpectedResult = "A ")]
        [TestCase("uniform", ExpectedResult = "A ")]
        [TestCase("Uniform", ExpectedResult = "A ")]
        [TestCase("uninformed", ExpectedResult = "An ")]
        [TestCase("Uninformed", ExpectedResult = "An ")]
        [TestCase("university", ExpectedResult = "A ")]
        [TestCase("University", ExpectedResult = "A ")]
        [TestCase("user", ExpectedResult = "A ")]
        [TestCase("User", ExpectedResult = "A ")]
        public static string Provides_matching_indefinite_article_for_(string text) => ArticleProvider.GetArticleFor(text);
    }
}