using System;
using System.Collections.Generic;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Extensions
{
    [TestFixture]
    public static partial class StringExtensionsTests
    {
        [Test]
        public static void EqualsAny_with_IEnumerable_returns_false_for_null_value() => Assert.That(((string)null).EqualsAny(new List<string> { "abc" }), Is.False);

        [Test]
        public static void EqualsAny_with_IEnumerable_returns_false_for_empty_value() => Assert.That(string.Empty.EqualsAny(new List<string> { "abc" }), Is.False);

        [Test]
        public static void EqualsAny_with_IEnumerable_returns_false_for_empty_phrases() => Assert.That("abc".EqualsAny(new List<string>()), Is.False);

        [Test]
        public static void EqualsAny_with_IEnumerable_returns_false_when_no_phrase_matches() => Assert.That("abc".EqualsAny(new List<string> { "xyz", "def" }), Is.False);

        [Test]
        public static void EqualsAny_with_IEnumerable_returns_true_when_first_phrase_matches() => Assert.That("abc".EqualsAny(new List<string> { "abc", "def" }), Is.True);

        [Test]
        public static void EqualsAny_with_IEnumerable_returns_true_when_last_phrase_matches() => Assert.That("abc".EqualsAny(new List<string> { "xyz", "abc" }), Is.True);

        [Test]
        public static void EqualsAny_with_IEnumerable_returns_false_for_different_casing_with_ordinal_comparison() => Assert.That("abc".EqualsAny(new List<string> { "ABC" }), Is.False);

        [Test]
        public static void EqualsAny_with_IEnumerable_returns_true_for_different_casing_with_OrdinalIgnoreCase_comparison() => Assert.That("abc".EqualsAny(new List<string> { "ABC" }, StringComparison.OrdinalIgnoreCase), Is.True);

        [Test]
        public static void EqualsAny_with_array_returns_false_for_null_value() => Assert.That(((string)null).EqualsAny(new[] { "abc" }), Is.False);

        [Test]
        public static void EqualsAny_with_array_returns_false_for_empty_value() => Assert.That(string.Empty.EqualsAny(new[] { "abc" }), Is.False);

        [Test]
        public static void EqualsAny_with_array_returns_false_for_empty_phrases() => Assert.That("abc".EqualsAny(Array.Empty<string>()), Is.False);

        [Test]
        public static void EqualsAny_with_array_returns_false_when_no_phrase_matches() => Assert.That("abc".EqualsAny(new[] { "xyz", "def" }), Is.False);

        [Test]
        public static void EqualsAny_with_array_returns_true_when_first_phrase_matches() => Assert.That("abc".EqualsAny(new[] { "abc", "def" }), Is.True);

        [Test]
        public static void EqualsAny_with_array_returns_true_when_last_phrase_matches() => Assert.That("abc".EqualsAny(new[] { "xyz", "abc" }), Is.True);

        [Test]
        public static void EqualsAny_with_array_returns_false_for_different_casing_with_ordinal_comparison() => Assert.That("abc".EqualsAny(new[] { "ABC" }), Is.False);

        [Test]
        public static void EqualsAny_with_array_returns_true_for_different_casing_with_OrdinalIgnoreCase_comparison() => Assert.That("abc".EqualsAny(new[] { "ABC" }, StringComparison.OrdinalIgnoreCase), Is.True);

        [Test]
        public static void EqualsAny_with_collection_expression_returns_false_for_null_value() => Assert.That(((string)null).EqualsAny(["abc"]), Is.False);

        [Test]
        public static void EqualsAny_with_collection_expression_returns_false_for_empty_value() => Assert.That(string.Empty.EqualsAny(["abc"]), Is.False);

        [Test]
        public static void EqualsAny_with_collection_expression_returns_false_for_empty_phrases() => Assert.That("abc".EqualsAny([]), Is.False);

        [Test]
        public static void EqualsAny_with_collection_expression_returns_false_when_no_phrase_matches() => Assert.That("abc".EqualsAny(["xyz", "def"]), Is.False);

        [Test]
        public static void EqualsAny_with_collection_expression_returns_true_when_first_phrase_matches() => Assert.That("abc".EqualsAny(["abc", "def"]), Is.True);

        [Test]
        public static void EqualsAny_with_collection_expression_returns_true_when_last_phrase_matches() => Assert.That("abc".EqualsAny(["xyz", "abc"]), Is.True);

        [Test]
        public static void EqualsAny_with_collection_expression_returns_false_for_different_casing_with_ordinal_comparison() => Assert.That("abc".EqualsAny(["ABC"]), Is.False);

        [Test]
        public static void EqualsAny_with_collection_expression_returns_true_for_different_casing_with_OrdinalIgnoreCase_comparison() => Assert.That("abc".EqualsAny(["ABC"], StringComparison.OrdinalIgnoreCase), Is.True);
    }
}