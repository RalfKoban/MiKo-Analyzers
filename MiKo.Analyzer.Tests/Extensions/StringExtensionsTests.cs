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

        [Test]
        public static void HumanizedConcatenated_concatenates_correctly()
        {
            Assert.Multiple(() =>
                                 {
                                     Assert.That(Array.Empty<string>().HumanizedConcatenated(), Is.EqualTo(string.Empty), "0 value");
                                     Assert.That(new[] { "a" }.HumanizedConcatenated(), Is.EqualTo("'a'"), "1 value");
                                     Assert.That(new[] { "a", "b" }.HumanizedConcatenated(), Is.EqualTo("'a' or 'b'"), "2 values");
                                     Assert.That(new[] { "a", "b", "c" }.HumanizedConcatenated(), Is.EqualTo("'a', 'b' or 'c'"), "3 values");
                                     Assert.That(new[] { "a", "b", "c", "d" }.HumanizedConcatenated(), Is.EqualTo("'a', 'b', 'c' or 'd'"), "4 values");
                                     Assert.That(new[] { "a", "b", "c", "d", "e" }.HumanizedConcatenated(), Is.EqualTo("'a', 'b', 'c', 'd' or 'e'"), "5 values");
                                 });
        }

        [Test]
        public static void FirstWord_returns_only_first_word()
        {
            Assert.Multiple(() =>
                                 {
                                     Assert.That("bla blubb".FirstWord(), Is.EqualTo("bla"), "without whitespace");
                                     Assert.That("bla blubb ".FirstWord(), Is.EqualTo("bla"), "without whitespace at start");
                                     Assert.That(" bla blubb".FirstWord(), Is.EqualTo("bla"), "with single whitespace at start");
                                     Assert.That("    bla blubb".FirstWord(), Is.EqualTo("bla"), "with multiple whitespace at start");
                                     Assert.That("    bla".FirstWord(), Is.EqualTo("bla"), "only word with multiple whitespaces at start");
                                 });
        }

        [Test]
        public static void SecondWord_returns_only_2nd_word()
        {
            Assert.Multiple(() =>
                                 {
                                     Assert.That("bla blubb".SecondWord(), Is.EqualTo("blubb"), "without whitespaces");
                                     Assert.That("bla blubb ".SecondWord(), Is.EqualTo("blubb"), "with single whitespace at end");
                                     Assert.That("bla blubb    ".SecondWord(), Is.EqualTo("blubb"), "with multiple whitespaces at end");
                                     Assert.That("bla blubb blubdiblub".SecondWord(), Is.EqualTo("blubb"), "with 3 words");
                                 });
        }

        [Test]
        public static void Words() => Assert.That("GetHashCode".Words(), Is.EquivalentTo(new[] { "Get", "Hash", "Code" }));
    }
}