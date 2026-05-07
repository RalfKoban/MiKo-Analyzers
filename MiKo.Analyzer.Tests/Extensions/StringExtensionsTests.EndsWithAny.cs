using System;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Extensions
{
    [TestFixture]
    public static partial class StringExtensionsTests
    {
        [Test]
        public static void EndsWithAny_with_array_returns_false_for_null_value() => Assert.That(((string)null).EndsWithAny(new[] { "bc" }), Is.False);

        [Test]
        public static void EndsWithAny_with_array_returns_false_for_empty_value() => Assert.That(string.Empty.EndsWithAny(new[] { "bc" }), Is.False);

        [Test]
        public static void EndsWithAny_with_array_returns_false_for_empty_suffixes() => Assert.That("abc".EndsWithAny(Array.Empty<string>()), Is.False);

        [Test]
        public static void EndsWithAny_with_array_returns_false_when_no_suffix_matches() => Assert.That("abc".EndsWithAny(new[] { "xy", "de" }), Is.False);

        [Test]
        public static void EndsWithAny_with_array_returns_false_when_suffix_is_longer_than_value() => Assert.That("ab".EndsWithAny(new[] { "abc" }), Is.False);

        [Test]
        public static void EndsWithAny_with_array_returns_true_when_first_suffix_matches() => Assert.That("abc".EndsWithAny(new[] { "bc", "xy" }), Is.True);

        [Test]
        public static void EndsWithAny_with_array_returns_true_when_last_suffix_matches() => Assert.That("abc".EndsWithAny(new[] { "xy", "bc" }), Is.True);

        [Test]
        public static void EndsWithAny_with_array_returns_true_when_value_equals_suffix() => Assert.That("abc".EndsWithAny(new[] { "abc" }), Is.True);

        [Test]
        public static void EndsWithAny_with_array_returns_false_for_different_casing_with_ordinal_comparison() => Assert.That("abc".EndsWithAny(new[] { "BC" }), Is.False);

        [Test]
        public static void EndsWithAny_with_array_returns_true_for_different_casing_with_OrdinalIgnoreCase_comparison() => Assert.That("abc".EndsWithAny(new[] { "BC" }, StringComparison.OrdinalIgnoreCase), Is.True);

        [Test]
        public static void EndsWithAny_with_collection_expression_returns_false_for_null_value() => Assert.That(((string)null).EndsWithAny(["bc"]), Is.False);

        [Test]
        public static void EndsWithAny_with_collection_expression_returns_false_for_empty_value() => Assert.That(string.Empty.EndsWithAny(["bc"]), Is.False);

        [Test]
        public static void EndsWithAny_with_collection_expression_returns_false_for_empty_suffixes() => Assert.That("abc".EndsWithAny([]), Is.False);

        [Test]
        public static void EndsWithAny_with_collection_expression_returns_false_when_no_suffix_matches() => Assert.That("abc".EndsWithAny(["xy", "de"]), Is.False);

        [Test]
        public static void EndsWithAny_with_collection_expression_returns_false_when_suffix_is_longer_than_value() => Assert.That("ab".EndsWithAny(["abc"]), Is.False);

        [Test]
        public static void EndsWithAny_with_collection_expression_returns_true_when_first_suffix_matches() => Assert.That("abc".EndsWithAny(["bc", "xy"]), Is.True);

        [Test]
        public static void EndsWithAny_with_collection_expression_returns_true_when_last_suffix_matches() => Assert.That("abc".EndsWithAny(["xy", "bc"]), Is.True);

        [Test]
        public static void EndsWithAny_with_collection_expression_returns_true_when_value_equals_suffix() => Assert.That("abc".EndsWithAny(["abc"]), Is.True);

        [Test]
        public static void EndsWithAny_with_collection_expression_returns_false_for_different_casing_with_ordinal_comparison() => Assert.That("abc".EndsWithAny(["BC"]), Is.False);

        [Test]
        public static void EndsWithAny_with_collection_expression_returns_true_for_different_casing_with_OrdinalIgnoreCase_comparison() => Assert.That("abc".EndsWithAny(["BC"], StringComparison.OrdinalIgnoreCase), Is.True);

        [Test]
        public static void EndsWithAny_with_ReadOnlySpan_returns_false_for_null_value() => Assert.That(((string)null).EndsWithAny(new[] { "bc" }.AsSpan()), Is.False);

        [Test]
        public static void EndsWithAny_with_ReadOnlySpan_returns_false_for_empty_value() => Assert.That(string.Empty.EndsWithAny(new[] { "bc" }.AsSpan()), Is.False);

        [Test]
        public static void EndsWithAny_with_ReadOnlySpan_returns_false_for_empty_suffixes() => Assert.That("abc".EndsWithAny(Array.Empty<string>().AsSpan()), Is.False);

        [Test]
        public static void EndsWithAny_with_ReadOnlySpan_returns_false_when_no_suffix_matches() => Assert.That("abc".EndsWithAny(new[] { "xy", "de" }.AsSpan()), Is.False);

        [Test]
        public static void EndsWithAny_with_ReadOnlySpan_returns_false_when_suffix_is_longer_than_value() => Assert.That("ab".EndsWithAny(new[] { "abc" }.AsSpan()), Is.False);

        [Test]
        public static void EndsWithAny_with_ReadOnlySpan_returns_true_when_first_suffix_matches() => Assert.That("abc".EndsWithAny(new[] { "bc", "xy" }.AsSpan()), Is.True);

        [Test]
        public static void EndsWithAny_with_ReadOnlySpan_returns_true_when_last_suffix_matches() => Assert.That("abc".EndsWithAny(new[] { "xy", "bc" }.AsSpan()), Is.True);

        [Test]
        public static void EndsWithAny_with_ReadOnlySpan_returns_true_when_value_equals_suffix() => Assert.That("abc".EndsWithAny(new[] { "abc" }.AsSpan()), Is.True);

        [Test]
        public static void EndsWithAny_with_ReadOnlySpan_returns_false_for_different_casing_with_ordinal_comparison() => Assert.That("abc".EndsWithAny(new[] { "BC" }.AsSpan()), Is.False);

        [Test]
        public static void EndsWithAny_with_ReadOnlySpan_returns_true_for_different_casing_with_OrdinalIgnoreCase_comparison() => Assert.That("abc".EndsWithAny(new[] { "BC" }.AsSpan(), StringComparison.OrdinalIgnoreCase), Is.True);
    }
}