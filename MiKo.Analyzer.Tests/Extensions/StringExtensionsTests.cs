﻿using System;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Extensions
{
    [TestFixture]
    public static class StringExtensionsTests
    {
        private const string LowerCaseCharacters = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperCaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string SpecialCases = " \t.?!;:,-1234567890";

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

        [TestCase("", ExpectedResult = false)]
        [TestCase("A", ExpectedResult = true)]
        [TestCase("a", ExpectedResult = false)]
        [TestCase("Aa", ExpectedResult = false)]
        [TestCase("aA", ExpectedResult = false)]
        [TestCase("AA", ExpectedResult = true)]
        public static bool IsAllUpperCase_works_(string input) => input.AsSpan().IsAllUpperCase();

        [Test]
        public static void SplitBy_splits_by_single_item_that_is_contained_multiple_times()
        {
            var result = "do split if something is split here.".SplitBy(["split"]);

            Assert.That(result, Is.EquivalentTo(["do ", "split", " if something is ", "split", " here."]));
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
                                     Assert.That("bla".FirstWord(), Is.EqualTo("bla"), "without whitespace");
                                     Assert.That("bla blubb".FirstWord(), Is.EqualTo("bla"), "without whitespace at both ends");
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
        public static void WordsAsSpan_returns_words() => Assert.That("GetHashCode".AsSpan().WordsAsSpan().Select(_ => _.ToString()), Is.EquivalentTo(["Get", "Hash", "Code"]));

        [TestCase(' ', "", ExpectedResult = " ")]
        [TestCase('-', "", ExpectedResult = "-")]
        [TestCase(' ', " with more ", ExpectedResult = "  with more ")]
        [TestCase('-', " with more ", ExpectedResult = "- with more ")]
        public static string ConcatenatedWith_with_char_and_string_(char c, string s) => c.ConcatenatedWith(s);

        [TestCase(' ', "", ExpectedResult = " ")]
        [TestCase('-', "", ExpectedResult = "-")]
        [TestCase(' ', " with more ", ExpectedResult = "  with more ")]
        [TestCase('-', " with more ", ExpectedResult = "- with more ")]
        public static string ConcatenatedWith_with_char_and_span_(char c, string s) => c.ConcatenatedWith(s.AsSpan());

        [TestCase("", " text ", ExpectedResult = " text ")]
        [TestCase(" Some ", " text ", ExpectedResult = " Some  text ")]
        [TestCase(" Some ", "", ExpectedResult = " Some ")]
        public static string ConcatenatedWith_with_string_and_span_(string s, string span) => s.ConcatenatedWith(span.AsSpan());

        [TestCase("", ' ', ExpectedResult = " ")]
        [TestCase("", '-', ExpectedResult = "-")]
        [TestCase("some start", ' ', ExpectedResult = "some start ")]
        [TestCase("some start ", '-', ExpectedResult = "some start -")]
        public static string ConcatenatedWith_with_string_and_char_(string s, char c) => s.ConcatenatedWith(c);

        [TestCase("", ' ', ExpectedResult = " ")]
        [TestCase("", '-', ExpectedResult = "-")]
        [TestCase("some start", ' ', ExpectedResult = "some start ")]
        [TestCase("some start ", '-', ExpectedResult = "some start -")]
        public static string ConcatenatedWith_with_span_and_char_(string s, char c) => s.AsSpan().ConcatenatedWith(c);

        [TestCase("", " text ", ExpectedResult = " text ")]
        [TestCase(" Some ", " text ", ExpectedResult = " Some  text ")]
        [TestCase(" Some ", "", ExpectedResult = " Some ")]
        public static string ConcatenatedWith_with_span_and_string_(string span, string s) => span.AsSpan().ConcatenatedWith(s);

        [TestCase(' ', "", '-', ExpectedResult = " -")]
        [TestCase('-', "", ' ', ExpectedResult = "- ")]
        [TestCase('-', "", 'a', ExpectedResult = "-a")]
        [TestCase(' ', " with more ", '-', ExpectedResult = "  with more -")]
        [TestCase('-', " with more ", ' ', ExpectedResult = "- with more  ")]
        public static string ConcatenatedWith_with_char_and_string_and_char_(char c1, string s, char c2) => c1.ConcatenatedWith(s, c2);

        [TestCase(' ', "", '-', ExpectedResult = " -")]
        [TestCase('-', "", ' ', ExpectedResult = "- ")]
        [TestCase('-', "", 'a', ExpectedResult = "-a")]
        [TestCase(' ', " with more ", '-', ExpectedResult = "  with more -")]
        [TestCase('-', " with more ", ' ', ExpectedResult = "- with more  ")]
        public static string ConcatenatedWith_with_char_and_span_and_char_(char c1, string s, char c2) => c1.ConcatenatedWith(s.AsSpan(), c2);

        [TestCase(' ', "", "some", ExpectedResult = " some")]
        [TestCase('-', "", " ", ExpectedResult = "- ")]
        [TestCase('-', "", "", ExpectedResult = "-")]
        [TestCase(' ', " with more ", "values", ExpectedResult = "  with more values")]
        [TestCase('-', " with more ", "values", ExpectedResult = "- with more values")]
        public static string ConcatenatedWith_with_char_and_string_and_span_(char c, string s, string span) => c.ConcatenatedWith(s, span.AsSpan());

        [TestCase(" Some ", ' ', " with more ", ExpectedResult = " Some   with more ")]
        [TestCase(" Some ", '-', " with more ", ExpectedResult = " Some - with more ")]
        public static string ConcatenatedWith_with_span_and_char_and_string_(string span, char c, string s) => span.AsSpan().ConcatenatedWith(c, s);

        [TestCase(" Some ", "", " with more ", ExpectedResult = " Some  with more ")]
        [TestCase(" Some ", " text ", " with more ", ExpectedResult = " Some  text  with more ")]
        [TestCase(" Some ", " text ", "", ExpectedResult = " Some  text ")]
        [TestCase("", " text ", "", ExpectedResult = " text ")]
        [TestCase("", "", "", ExpectedResult = "")]
        public static string ConcatenatedWith_with_span_and_string_and_string_(string span, string s1, string s2) => span.AsSpan().ConcatenatedWith(s1, s2);

        [TestCase(" Some ", "", " with more ", ExpectedResult = " Some  with more ")]
        [TestCase(" Some ", " text ", " with more ", ExpectedResult = " Some  text  with more ")]
        [TestCase(" Some ", " text ", "", ExpectedResult = " Some  text ")]
        [TestCase("", " text ", "", ExpectedResult = " text ")]
        [TestCase("", "", "", ExpectedResult = "")]
        public static string ConcatenatedWith_with_span_and_string_and_span_(string span1, string s, string span2) => span1.AsSpan().ConcatenatedWith(s, span2.AsSpan());

        [TestCase(" Some ", "", " with more ", ExpectedResult = " Some  with more ")]
        [TestCase(" Some ", " text ", " with more ", ExpectedResult = " Some  text  with more ")]
        [TestCase(" Some ", " text ", "", ExpectedResult = " Some  text ")]
        [TestCase("", " text ", "", ExpectedResult = " text ")]
        [TestCase("", "", "", ExpectedResult = "")]
        public static string ConcatenatedWith_with_string_and_string_and_span_(string s1, string s2, string span) => s1.ConcatenatedWith(s2, span.AsSpan());

        [TestCase(" Some ", "", '.', ExpectedResult = " Some .")]
        [TestCase(" Some ", " text ", '.', ExpectedResult = " Some  text .")]
        [TestCase("", "", '.', ExpectedResult = ".")]
        public static string ConcatenatedWith_with_string_and_span_and_char_(string s, string span, char c) => s.ConcatenatedWith(span.AsSpan(), c);

        [TestCase(" Some ", "", " with more ", ExpectedResult = " Some  with more ")]
        [TestCase(" Some ", " text ", " with more ", ExpectedResult = " Some  text  with more ")]
        [TestCase(" Some ", " text ", "", ExpectedResult = " Some  text ")]
        [TestCase("", " text ", "", ExpectedResult = " text ")]
        [TestCase("", "", "", ExpectedResult = "")]
        public static string ConcatenatedWith_with_string_and_span_and_string_(string s1, string span, string s2) => s1.ConcatenatedWith(span.AsSpan(), s2);

        [TestCase(" Some ", '-', " with more ", '!', ExpectedResult = " Some - with more !")]
        [TestCase("", 'a', "", 'b', ExpectedResult = "ab")]
        [TestCase("", 'a', " - ", 'b', ExpectedResult = "a - b")]
        [TestCase("Some", ' ', "", '!', ExpectedResult = "Some !")]
        public static string ConcatenatedWith_with_span_and_char_and_string_and_char_(string span, char c1, string s, char c2) => span.AsSpan().ConcatenatedWith(c1, s, c2);

        [TestCaseSource(nameof(SpecialCases))]
        public static void IsUpperCase_is_false_for_char_(char c) => Assert.That(c.IsUpperCase(), Is.False);

        [TestCaseSource(nameof(LowerCaseCharacters))]
        public static void IsUpperCase_is_false_for_(char c) => Assert.That(c.IsUpperCase(), Is.False);

        [TestCaseSource(nameof(UpperCaseCharacters))]
        public static void IsUpperCase_is_true_for_(char c) => Assert.That(c.IsUpperCase(), Is.True);

        [TestCaseSource(nameof(LowerCaseCharacters))]
        public static void ToUpperCase_for_lower_case_(char c) => Assert.That(c.ToUpperCase(), Is.EqualTo(char.ToUpperInvariant(c)));

        [TestCaseSource(nameof(UpperCaseCharacters))]
        public static void ToUpperCase_for_upper_case_(char c) => Assert.That(c.ToUpperCase(), Is.EqualTo(c));

        [TestCaseSource(nameof(LowerCaseCharacters))]
        public static void ToLowerCase_for_lower_case_(char c) => Assert.That(c.ToLowerCase(), Is.EqualTo(c));

        [TestCaseSource(nameof(UpperCaseCharacters))]
        public static void ToLowerCase_for_upper_case_(char c) => Assert.That(c.ToLowerCase(), Is.EqualTo(char.ToLowerInvariant(c)));
    }
}