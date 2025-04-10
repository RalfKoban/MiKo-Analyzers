﻿using System;

using NUnit.Framework;

//// ncrunch: rdi off
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
#pragma warning disable CS1718 // Comparison made to same variable

                                     // ReSharper disable once EqualExpressionComparison
                                     Assert.That(pair1 == pair1, "Equality operator for same instance");
                                     Assert.That(pair1 == pair2, "Equality operator for similar instances");

#pragma warning restore CS1718 // Comparison made to same variable
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
#pragma warning disable CS1718 // Comparison made to same variable

                                     // ReSharper disable once EqualExpressionComparison
                                     Assert.That(pair1 != pair1, Is.False, "Inequality operator for same instance");
                                     Assert.That(pair1 != pair2, Is.False, "Inequality operator for similar instances");

#pragma warning restore CS1718 // Comparison made to same variable
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
        public void Object_Equals_considers_similar_pairs_as_equal()
        {
            var pair1 = new Pair("key", "value");
            var pair2 = new Pair("key", "value");

            Assert.That(pair1.Equals((object)pair1), Is.True);
            Assert.That(pair1.Equals((object)pair2), Is.True);
        }

        [TestCase("some", "value", "some", "other")]
        [TestCase("some", "value", "other", "value")]
        public void Object_Equals_considers_different_pairs_as_not_equal_(string key1, string value1, string key2, string value2)
        {
            var pair1 = new Pair(key1, value1);
            var pair2 = new Pair(key2, value2);

            Assert.That(pair1.Equals((object)pair2), Is.False);
        }

        [Test]
        public void GetHashCode_of_similar_pairs_is_equal()
        {
            var hash1 = new Pair("key", "value").GetHashCode();
            var hash2 = new Pair("key", "value").GetHashCode();

            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [TestCase("some", "value", "some", "other")]
        [TestCase("some", "value", "other", "value")]
        public void GetHashCode_of_different_pairs_is_not_equal_(string key1, string value1, string key2, string value2)
        {
            var hash1 = new Pair(key1, value1).GetHashCode();
            var hash2 = new Pair(key2, value2).GetHashCode();

            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void ToString_returns_value()
        {
            var pair = new Pair("some", "text");

            Assert.That(pair.ToString(), Is.EqualTo("some -> text"));
        }
    }
}