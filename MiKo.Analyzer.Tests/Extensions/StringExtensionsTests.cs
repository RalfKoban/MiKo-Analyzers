using System;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Extensions
{
    [TestFixture]
    public static class StringExtensionsTests
    {
        [TestCase(null, ExpectedResult = null)]
        [TestCase("", ExpectedResult = "")]
        [TestCase("a", ExpectedResult = "a")]
        [TestCase("a1", ExpectedResult = "a")]
        [TestCase("ab", ExpectedResult = "ab")]
        [TestCase("ab1", ExpectedResult = "ab")]
        [TestCase("abc", ExpectedResult = "abc")]
        [TestCase("abc42", ExpectedResult = "abc")]
        [TestCase("a1bc", ExpectedResult = "a1bc")]
        [TestCase("a1bc42", ExpectedResult = "a1bc")]
        public static string WithoutNumberSuffix_works_(string input) => input.WithoutNumberSuffix();

        [TestCase("ab123", ExpectedResult = true)]
        [TestCase("ab1", ExpectedResult = true)]
        [TestCase("ab", ExpectedResult = false)]
        [TestCase("a8b", ExpectedResult = false)]
        [TestCase("42ab", ExpectedResult = false)]
        [TestCase("1", ExpectedResult = true)]
        public static bool EndsWithNumber_works_(string input) => input.EndsWithNumber();

        [TestCase("ab123", ExpectedResult = false)]
        [TestCase("ab1", ExpectedResult = false)]
        [TestCase("ab", ExpectedResult = false)]
        [TestCase("a8b", ExpectedResult = false)]
        [TestCase("42ab", ExpectedResult = true)]
        [TestCase("1ab", ExpectedResult = true)]
        [TestCase("1", ExpectedResult = true)]
        public static bool StartsWithNumber_works_(string input) => input.StartsWithNumber();

        [Test]
        public static void SplitBy_splits_by_single_item_that_is_contained_multiple_times()
        {
            var result = "do split if something is split here.".SplitBy(new[] { "split" });

            Assert.That(result, Is.EquivalentTo(new[]
                                                    {
                                                        "do ",
                                                        "split",
                                                        " if something is ",
                                                        "split",
                                                        " here.",
                                                    }));
        }

        [Test]
        public static void SplitBy_splits_by_multiple_items_that_are_contained_a_single_time()
        {
            var result = "do split_1 if something is split_2 here.".SplitBy(new[] { "split_1", "split_2" });

            Assert.That(result, Is.EquivalentTo(new[]
                                                    {
                                                        "do ",
                                                        "split_1",
                                                        " if something is ",
                                                        "split_2",
                                                        " here.",
                                                    }));
        }

        [Test]
        public static void SplitBy_splits_by_multiple_items_that_are_contained_multiple_times()
        {
            var result = "do split_1 if something is split_2 here. Also split_1 here because split_2 is contained multiple times.".SplitBy(new[] { "split_1", "split_2" });

            Assert.That(result, Is.EquivalentTo(new[]
                                                    {
                                                        "do ",
                                                        "split_1",
                                                        " if something is ",
                                                        "split_2",
                                                        " here. Also ",
                                                        "split_1",
                                                        " here because ",
                                                        "split_2",
                                                        " is contained multiple times.",
                                                    }));
        }
    }
}