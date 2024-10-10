using System;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers
{
    [TestFixture]
    public sealed class PairTests
    {
        [TestCase("some", "value")]
        public void Equality_operator_considers_similar_pairs_as_equal_(string key, string value)
        {
            var pair1 = new Pair(key, value);
            var pair2 = new Pair(key, value);

            Assert.Multiple(() =>
                                 {
                                     // ReSharper disable once EqualExpressionComparison
                                     Assert.That(pair1 == pair1, "Equality operator for same instance");
                                     Assert.That(pair1 == pair2, "Equality operator for similar instances");
                                 });
        }

        [TestCase("some", "value", "some", "other")]
        [TestCase("some", "value", "other", "value")]
        public void Equality_operator_considers_different_pairs_as_not_equal_(string key1, string value1, string key2, string value2)
        {
            var pair1 = new Pair(key1, value1);
            var pair2 = new Pair(key2, value2);

            Assert.That(pair1 == pair2, Is.False);
        }

        [TestCase("some", "value")]
        public void Inequality_operator_considers_similar_pairs_as_equal_(string key, string value)
        {
            var pair1 = new Pair(key, value);
            var pair2 = new Pair(key, value);

            Assert.Multiple(() =>
                                 {
                                     // ReSharper disable once EqualExpressionComparison
                                     Assert.That(pair1 != pair1, Is.False, "Inequality operator for same instance");
                                     Assert.That(pair1 != pair2, Is.False, "Inequality operator for similar instances");
                                 });
        }

        [TestCase("some", "value", "some", "other")]
        [TestCase("some", "value", "other", "value")]
        public void Inequality_operator_considers_different_pairs_as_not_equal_(string key1, string value1, string key2, string value2)
        {
            var pair1 = new Pair(key1, value1);
            var pair2 = new Pair(key2, value2);

            Assert.That(pair1 != pair2);
        }

        [Test]
        public void ToString_returns_value()
        {
            var pair = new Pair("some", "text");

            Assert.That(pair.ToString(), Is.EqualTo("some -> text"));
        }
    }
}