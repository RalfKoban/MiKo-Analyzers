using System;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Extensions
{
    [TestFixture]
    public static partial class StringExtensionsTests
    {
        [TestCase(' ', "", ExpectedResult = " ")]
        [TestCase('-', "", ExpectedResult = "-")]
        [TestCase(' ', " with more ", ExpectedResult = "  with more ")]
        [TestCase('-', " with more ", ExpectedResult = "- with more ")]
        public static string ConcatenatedWith_with_char_and_string_(in char c, string s) => c.ConcatenatedWith(s);

        [TestCase(' ', "", ExpectedResult = " ")]
        [TestCase('-', "", ExpectedResult = "-")]
        [TestCase(' ', " with more ", ExpectedResult = "  with more ")]
        [TestCase('-', " with more ", ExpectedResult = "- with more ")]
        public static string ConcatenatedWith_with_char_and_span_(in char c, string s) => c.ConcatenatedWith(s.AsSpan());

        [TestCase("", " text ", ExpectedResult = " text ")]
        [TestCase(" Some ", " text ", ExpectedResult = " Some  text ")]
        [TestCase(" Some ", "", ExpectedResult = " Some ")]
        public static string ConcatenatedWith_with_string_and_span_(string s, string span) => s.ConcatenatedWith(span.AsSpan());

        [TestCase("", ' ', ExpectedResult = " ")]
        [TestCase("", '-', ExpectedResult = "-")]
        [TestCase("some start", ' ', ExpectedResult = "some start ")]
        [TestCase("some start ", '-', ExpectedResult = "some start -")]
        public static string ConcatenatedWith_with_string_and_char_(string s, in char c) => s.ConcatenatedWith(c);

        [TestCase("", ' ', ExpectedResult = " ")]
        [TestCase("", '-', ExpectedResult = "-")]
        [TestCase("some start", ' ', ExpectedResult = "some start ")]
        [TestCase("some start ", '-', ExpectedResult = "some start -")]
        public static string ConcatenatedWith_with_span_and_char_(string s, in char c) => s.AsSpan().ConcatenatedWith(c);

        [TestCase("", " text ", ExpectedResult = " text ")]
        [TestCase(" Some ", " text ", ExpectedResult = " Some  text ")]
        [TestCase(" Some ", "", ExpectedResult = " Some ")]
        public static string ConcatenatedWith_with_span_and_string_(string span, string s) => span.AsSpan().ConcatenatedWith(s);

        [TestCase(' ', "", '-', ExpectedResult = " -")]
        [TestCase('-', "", ' ', ExpectedResult = "- ")]
        [TestCase('-', "", 'a', ExpectedResult = "-a")]
        [TestCase(' ', " with more ", '-', ExpectedResult = "  with more -")]
        [TestCase('-', " with more ", ' ', ExpectedResult = "- with more  ")]
        public static string ConcatenatedWith_with_char_and_string_and_char_(in char c1, string s, in char c2) => c1.ConcatenatedWith(s, c2);

        [TestCase(' ', "", '-', ExpectedResult = " -")]
        [TestCase('-', "", ' ', ExpectedResult = "- ")]
        [TestCase('-', "", 'a', ExpectedResult = "-a")]
        [TestCase(' ', " with more ", '-', ExpectedResult = "  with more -")]
        [TestCase('-', " with more ", ' ', ExpectedResult = "- with more  ")]
        public static string ConcatenatedWith_with_char_and_span_and_char_(in char c1, string s, in char c2) => c1.ConcatenatedWith(s.AsSpan(), c2);

        [TestCase(' ', "", "some", ExpectedResult = " some")]
        [TestCase('-', "", " ", ExpectedResult = "- ")]
        [TestCase('-', "", "", ExpectedResult = "-")]
        [TestCase(' ', " with more ", "values", ExpectedResult = "  with more values")]
        [TestCase('-', " with more ", "values", ExpectedResult = "- with more values")]
        public static string ConcatenatedWith_with_char_and_string_and_span_(in char c, string s, string span) => c.ConcatenatedWith(s, span.AsSpan());

        [TestCase(" Some ", ' ', " with more ", ExpectedResult = " Some   with more ")]
        [TestCase(" Some ", '-', " with more ", ExpectedResult = " Some - with more ")]
        public static string ConcatenatedWith_with_span_and_char_and_string_(string span, in char c, string s) => span.AsSpan().ConcatenatedWith(c, s);

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
        public static string ConcatenatedWith_with_string_and_span_and_char_(string s, string span, in char c) => s.ConcatenatedWith(span.AsSpan(), c);

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
        public static string ConcatenatedWith_with_span_and_char_and_string_and_char_(string span, in char c1, string s, in char c2) => span.AsSpan().ConcatenatedWith(c1, s, c2);
    }
}