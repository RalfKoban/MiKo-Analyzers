using System;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Extensions
{
    [TestFixture]
    public static partial class StringExtensionsTests
    {
        private const string LowerCaseCharacters = "abcdefghijklmnopqrstuvwxyz";
        private const string Numbers = "1234567890";
        private const string SentenceParts = ".?!;:,-";
        private const string SpecialCases = " \t" + SentenceParts + Numbers;
        private const string UpperCaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly char[] WhiteSpaces = [' ', '\t', '\r', '\n', '\v', '\f'];

        [TestCase("ab123", ExpectedResult = true)]
        [TestCase("ab1", ExpectedResult = true)]
        [TestCase("ab", ExpectedResult = false)]
        [TestCase("a8b", ExpectedResult = false)]
        [TestCase("42ab", ExpectedResult = false)]
        [TestCase("1", ExpectedResult = true)]
        public static bool EndsWithNumber_detects_number_as_suffix_(string input) => input.EndsWithNumber();

        [TestCase(null, ExpectedResult = null)]
        [TestCase("", ExpectedResult = "")]
        [TestCase("   ", ExpectedResult = "")]
        [TestCase("Gets the value.", ExpectedResult = "Gets the value")]
        [TestCase("Gets the value? rest", ExpectedResult = "Gets the value")]
        [TestCase("Gets the value! rest", ExpectedResult = "Gets the value")]
        [TestCase("Gets the value. rest", ExpectedResult = "Gets the value")]
        [TestCase("Gets Some", ExpectedResult = "Gets ")]
        [TestCase("GetsValue", ExpectedResult = "Gets")]
        [TestCase("single", ExpectedResult = "single")]
        [TestCase("   leading whitespace", ExpectedResult = "leading whitespace")]
        public static string FirstSentence_returns_correct_result_(string value) => value.FirstSentence();

        [Test]
        public static void FirstWord_returns_only_first_word()
        {
            Assert.Multiple(() =>
                                 {
                                     Assert.That("bla".FirstWord(), Is.EqualTo("bla"), "without whitespace");
                                     Assert.That("bla blubb".FirstWord(), Is.EqualTo("bla"), "without whitespace at both ends");
                                     Assert.That("bla blubb ".FirstWord(), Is.EqualTo("bla"), "without whitespace at start");
                                     Assert.That(" bla blubb".FirstWord(), Is.EqualTo("bla"), "with single whitespace at start");
                                     Assert.That("    bla blubb".FirstWord(), Is.EqualTo("bla"), "with multiple whitespace at start");
                                     Assert.That("    bla".FirstWord(), Is.EqualTo("bla"), "only word with multiple whitespaces at start");
                                 });
        }

        [Test]
        public static void GetNumber_returns_big_numbers_with_commas_and_dots()
        {
            var numbers = "some text with 12,345.321 and 67,890.5".GetNumbers();

            Assert.That(numbers, Is.EquivalentTo(["12,345.321", "67,890.5"]));
        }

        [Test]
        public static void GetNumber_returns_big_numbers_with_commas_as_thousand_separators()
        {
            var numbers = "some text with 12,345 and 67,890".GetNumbers();

            Assert.That(numbers, Is.EquivalentTo(["12,345", "67,890"]));
        }

        [Test]
        public static void GetNumber_returns_big_numbers_with_dots_and_commas()
        {
            var numbers = "some text with 12.345,321 and 67.890,5".GetNumbers();

            Assert.That(numbers, Is.EquivalentTo(["12.345,321", "67.890,5"]));
        }

        [Test]
        public static void GetNumber_returns_big_numbers_with_dots_as_thousand_separators()
        {
            var numbers = "some text with 12.345 and 67.890".GetNumbers();

            Assert.That(numbers, Is.EquivalentTo(["12.345", "67.890"]));
        }

        [Test]
        public static void GetNumber_returns_big_numbers_with_underlines_as_separators()
        {
            var numbers = "some text with 12_345 and 67_890".GetNumbers();

            Assert.That(numbers, Is.EquivalentTo(["12_345", "67_890"]));
        }

        [Test]
        public static void GetNumber_returns_big_numbers_without_separators()
        {
            var numbers = "some text with 12345 and 67890".GetNumbers();

            Assert.That(numbers, Is.EquivalentTo(["12345", "67890"]));
        }

        [Test]
        public static void GetNumber_returns_single_big_number_with_underlines_as_separators()
        {
            var numbers = "some text with 1_000_000".GetNumbers();

            Assert.That(numbers, Is.EquivalentTo(["1_000_000"]));
        }

        [Test]
        public static void GetNumber_returns_small_numbers()
        {
            var numbers = "some text with 1, 2 and -3".GetNumbers();

            Assert.That(numbers, Is.EquivalentTo(["1", "2", "-3"]));
        }

        [Test]
        public static void HumanizedConcatenated_concatenates_texts()
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

        [TestCase("", ExpectedResult = false)]
        [TestCase("A", ExpectedResult = true)]
        [TestCase("a", ExpectedResult = false)]
        [TestCase("Aa", ExpectedResult = false)]
        [TestCase("aA", ExpectedResult = false)]
        [TestCase("AA", ExpectedResult = true)]
        public static bool IsAllUpperCase_detects_text_being_all_upper_case_characters_(string input) => input.AsSpan().IsAllUpperCase();

        [TestCase('+')]
        [TestCaseSource(nameof(UpperCaseCharacters))]
        [TestCaseSource(nameof(LowerCaseCharacters))]
        [TestCaseSource(nameof(SentenceParts))]
        [TestCaseSource(nameof(WhiteSpaces))]
        public static void IsNumber_char_is_false_for_(char value) => Assert.That(value.IsNumber(), Is.False);

        [Test]
        public static void IsNumber_char_is_true_for_([ValueSource(nameof(Numbers))] char value) => Assert.That(value.IsNumber());

        [TestCase(" ")]
        [TestCase("")]
        [TestCase("-")]
        [TestCase(".")]
        [TestCase("+-1")]
        [TestCase("1 2")]
        [TestCase("1234.-215.12.12412.1sgsgd")]
        [TestCase("1abc")]
        [TestCase("3.1415927.")]
        [TestCase("a")]
        [TestCase("abc")]
        [TestCase("abc1")]
        [TestCase("some")]
        [TestCase("TestMe42")]
        [TestCase("X-123")]
        public static void IsNumber_is_false_for_(string value) => Assert.That(value.IsNumber(), Is.False);

        [TestCase("+1")]
        [TestCase("+1_234_567_890")]
        [TestCase("+42")]
        [TestCase("0")]
        [TestCase("-0")]
        [TestCase("1")]
        [TestCase("-1")]
        [TestCase("1,000")]
        [TestCase("1,000.5")]
        [TestCase("1,5")]
        [TestCase("1.000")]
        [TestCase("1.000,5")]
        [TestCase("1.5")]
        [TestCase("1_000")]
        [TestCase("1_000_000")]
        [TestCase("1_000_000_000")]
        [TestCase("1_234_567_890")]
        [TestCase("-1_234_567_890")]
        [TestCase("123")]
        [TestCase("-123")]
        [TestCase("123.45")]
        [TestCase("3.1415927")]
        [TestCase("42")]
        [TestCase("-42")]
        [TestCase("5")]
        [TestCase("9")]
        public static void IsNumber_is_true_for_(string value) => Assert.That(value.IsNumber());

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(".")]
        [TestCase("-")]
        [TestCase("a")]
        [TestCase("abc")]
        [TestCase("some")]
        [TestCase("1abc")]
        [TestCase("abc1")]
        [TestCase("1 2")]
        [TestCase("+-1")]
        [TestCase("1234.-215.12.12412.1sgsgd")]
        [TestCase("3.1415927.")]
        [TestCase("X-123")]
        [TestCase("TestMe42")]
        public static void IsNumber_span_is_false_for_(string value) => Assert.That(value.AsSpan().IsNumber(), Is.False);

        [TestCase("0")]
        [TestCase("-0")]
        [TestCase("+1")]
        [TestCase("-1")]
        [TestCase("+42")]
        [TestCase("-42")]
        [TestCase("-123")]
        [TestCase("1.5")]
        [TestCase("1,5")]
        [TestCase("42")]
        [TestCase("123")]
        [TestCase("123.45")]
        [TestCase("3.1415927")]
        [TestCase("1.000")]
        [TestCase("1,000")]
        [TestCase("1_000")]
        [TestCase("1_000_000")]
        [TestCase("1_000_000_000")]
        [TestCase("1_234_567_890")]
        [TestCase("+1_234_567_890")]
        [TestCase("-1_234_567_890")]
        [TestCase("1,000.5")]
        [TestCase("1.000,5")]
        public static void IsNumber_span_is_true_for_(string value) => Assert.That(value.AsSpan().IsNumber());

        [TestCase(null)]
        [TestCase("some word")]
        [TestCase("some\tword")]
        [TestCase("some\r\nword")]
        [TestCase(" leading")]
        [TestCase("trailing ")]
        public static void IsSingleWord_returns_false_for_(string value) => Assert.That(value.IsSingleWord(), Is.False);

        [TestCase("")]
        [TestCase("a")]
        [TestCase("abc")]
        [TestCase("SomeWord")]
        [TestCase("someword")]
        public static void IsSingleWord_returns_true_for_(string value) => Assert.That(value.IsSingleWord(), Is.True);

        [TestCaseSource(nameof(LowerCaseCharacters))]
        public static void IsUpperCase_is_false_for_(in char c) => Assert.That(c.IsUpperCase(), Is.False);

        [TestCaseSource(nameof(SpecialCases))]
        public static void IsUpperCase_is_false_for_char_(in char c) => Assert.That(c.IsUpperCase(), Is.False);

        [TestCaseSource(nameof(UpperCaseCharacters))]
        public static void IsUpperCase_is_true_for_(in char c) => Assert.That(c.IsUpperCase(), Is.True);

        [TestCaseSource(nameof(UpperCaseCharacters))]
        [TestCaseSource(nameof(LowerCaseCharacters))]
        [TestCaseSource(nameof(Numbers))]
        [TestCaseSource(nameof(SentenceParts))]
        public static void IsWhiteSpace_is_false_for_letter_(in char c) => Assert.That(c.IsWhiteSpace(), Is.False);

        [Test]
        public static void IsWhiteSpace_is_true_for_whitespace_([ValueSource(nameof(WhiteSpaces))] in char c) => Assert.That(c.IsWhiteSpace(), Is.True);

        [Test]
        public static void LastWord_returns_only_last_word()
        {
            Assert.Multiple(() =>
                                 {
                                     Assert.That(((string)null).LastWord(), Is.Null, "with null as text");
                                     Assert.That(string.Empty.LastWord(), Is.EqualTo(string.Empty), "with empty text");
                                     Assert.That("   ".LastWord(), Is.EqualTo(string.Empty), "with whitespace-only text");
                                     Assert.That("bla".LastWord(), Is.EqualTo("bla"), "without whitespace");
                                     Assert.That("bla blubb".LastWord(), Is.EqualTo("blubb"), "without whitespace at end");
                                     Assert.That(" bla blubb ".LastWord(), Is.EqualTo("blubb"), "with single whitespace at end");
                                     Assert.That(" bla blubb   ".LastWord(), Is.EqualTo("blubb"), "with multiple whitespace at end");
                                     Assert.That(" bla          ".LastWord(), Is.EqualTo("bla"), "only word with multiple whitespaces at end");
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
        public static void SplitBy_splits_by_multiple_items_that_are_contained_a_single_time()
        {
            var result = "do split_1 if something is split_2 here.".SplitBy(["split_1", "split_2"]);

            Assert.That(result, Is.EquivalentTo(["do ", "split_1", " if something is ", "split_2", " here."]));
        }

        [Test]
        public static void SplitBy_splits_by_multiple_items_that_are_contained_multiple_times()
        {
            var result = "do split_1 if something is split_2 here. Also split_1 here because split_2 is contained multiple times.".SplitBy(["split_1", "split_2"]);

            Assert.That(result, Is.EquivalentTo(["do ", "split_1", " if something is ", "split_2", " here. Also ", "split_1", " here because ", "split_2", " is contained multiple times."]));
        }

        [Test]
        public static void SplitBy_splits_by_single_item_that_is_contained_multiple_times()
        {
            var result = "do split if something is split here.".SplitBy(["split"]);

            Assert.That(result, Is.EquivalentTo(["do ", "split", " if something is ", "split", " here."]));
        }

        [TestCase("ab123", ExpectedResult = false)]
        [TestCase("ab1", ExpectedResult = false)]
        [TestCase("ab", ExpectedResult = false)]
        [TestCase("a8b", ExpectedResult = false)]
        [TestCase("42ab", ExpectedResult = true)]
        [TestCase("1ab", ExpectedResult = true)]
        [TestCase("1", ExpectedResult = true)]
        public static bool StartsWithNumber_detects_number_as_prefix_(string input) => input.StartsWithNumber();

        [TestCaseSource(nameof(LowerCaseCharacters))]
        public static void ToLowerCase_for_lower_case_(in char c) => Assert.That(c.ToLowerCase(), Is.EqualTo(c));

        [TestCaseSource(nameof(UpperCaseCharacters))]
        public static void ToLowerCase_for_upper_case_(in char c) => Assert.That(c.ToLowerCase(), Is.EqualTo(char.ToLowerInvariant(c)));

        [TestCaseSource(nameof(LowerCaseCharacters))]
        public static void ToUpperCase_for_lower_case_(in char c) => Assert.That(c.ToUpperCase(), Is.EqualTo(char.ToUpperInvariant(c)));

        [TestCaseSource(nameof(UpperCaseCharacters))]
        public static void ToUpperCase_for_upper_case_(in char c) => Assert.That(c.ToUpperCase(), Is.EqualTo(c));

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
        [TestCase("a1bc_42", ExpectedResult = "a1bc")]
        [TestCase("a1bc_4_2", ExpectedResult = "a1bc")]
        [TestCase("_4_2", ExpectedResult = "")]
        public static string WithoutNumberSuffix_String_returns_text_without_number_suffix_(string input) => input.WithoutNumberSuffix();

        [TestCase("", ExpectedResult = "")]
        [TestCase("a", ExpectedResult = "a")]
        [TestCase("a1", ExpectedResult = "a")]
        [TestCase("ab", ExpectedResult = "ab")]
        [TestCase("ab1", ExpectedResult = "ab")]
        [TestCase("abc", ExpectedResult = "abc")]
        [TestCase("abc42", ExpectedResult = "abc")]
        [TestCase("a1bc", ExpectedResult = "a1bc")]
        [TestCase("a1bc42", ExpectedResult = "a1bc")]
        [TestCase("a1bc_42", ExpectedResult = "a1bc")]
        [TestCase("a1bc_4_2", ExpectedResult = "a1bc")]
        [TestCase("_4_2", ExpectedResult = "")]
        public static string WithoutNumberSuffix_ReadOnlySpan_returns_text_without_number_suffix_(string input) => input.AsSpan().WithoutNumberSuffix().ToString();

        [Test]
        public static void WordsAsSpan_returns_words_in_method_name() => Assert.That("GetHashCode".WordsAsSpan().Select(_ => _.ToString()), Is.EquivalentTo(["Get", "Hash", "Code"]));

        [Test]
        public static void WordsAsSpan_returns_words_in_text_with_leading_whitespaces() => Assert.That(" This is some text for -1. Just to be sure.".WordsAsSpan(WordBoundary.WhiteSpaces).Select(_ => _.ToString()), Is.EquivalentTo(["This", "is", "some", "text", "for", "-1", "Just", "to", "be", "sure"]));

        [Test]
        public static void WordsAsSpan_returns_words_in_text_without_leading_whitespaces() => Assert.That("This is some text for -1. Just to be sure.".WordsAsSpan(WordBoundary.WhiteSpaces).Select(_ => _.ToString()), Is.EquivalentTo(["This", "is", "some", "text", "for", "-1", "Just", "to", "be", "sure"]));
    }
}