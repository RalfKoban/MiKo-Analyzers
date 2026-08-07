using System;
using System.Linq;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers
{
    [TestFixture]
    public static class AhoCorasickMatcherTests
    {
        [Test]
        public static void IsMatch_returns_false_for_empty_text()
        {
            var matcher = AhoCorasickMatcher.For(["abc"]);

            Assert.That(matcher.IsMatch(string.Empty.AsSpan()), Is.False);
        }

        [Test]
        public static void IsMatch_returns_false_when_text_does_not_contain_any_pattern()
        {
            var matcher = AhoCorasickMatcher.For(["abc", "xyz"]);

            Assert.That(matcher.IsMatch("some other text".AsSpan()), Is.False);
        }

        [Test]
        public static void IsMatch_returns_true_when_text_contains_a_single_pattern()
        {
            var matcher = AhoCorasickMatcher.For(["abc"]);

            Assert.That(matcher.IsMatch("some abc text".AsSpan()), Is.True);
        }

        [Test]
        public static void IsMatch_returns_true_when_pattern_is_at_the_start_of_the_text()
        {
            var matcher = AhoCorasickMatcher.For(["abc"]);

            Assert.That(matcher.IsMatch("abc some text".AsSpan()), Is.True);
        }

        [Test]
        public static void IsMatch_returns_true_when_pattern_is_at_the_end_of_the_text()
        {
            var matcher = AhoCorasickMatcher.For(["abc"]);

            Assert.That(matcher.IsMatch("some text abc".AsSpan()), Is.True);
        }

        [Test]
        public static void IsMatch_returns_true_when_pattern_equals_the_whole_text()
        {
            var matcher = AhoCorasickMatcher.For(["abc"]);

            Assert.That(matcher.IsMatch("abc".AsSpan()), Is.True);
        }

        [Test]
        public static void IsMatch_returns_false_when_text_is_shorter_than_the_pattern()
        {
            var matcher = AhoCorasickMatcher.For(["abcdef"]);

            Assert.That(matcher.IsMatch("abc".AsSpan()), Is.False);
        }

        [Test]
        public static void IsMatch_finds_any_matching_pattern_out_of_multiple_patterns()
        {
            var matcher = AhoCorasickMatcher.For(["cat", "dog", "bird"]);

            Assert.Multiple(() =>
                                 {
                                     Assert.That(matcher.IsMatch("I have a cat".AsSpan()), Is.True);
                                     Assert.That(matcher.IsMatch("I have a dog".AsSpan()), Is.True);
                                     Assert.That(matcher.IsMatch("I have a bird".AsSpan()), Is.True);
                                     Assert.That(matcher.IsMatch("I have a fish".AsSpan()), Is.False);
                                 });
        }

        [Test]
        public static void IsMatch_finds_pattern_that_is_a_prefix_of_another_pattern()
        {
            // 'he' is a prefix of 'hers' and 'she' contains 'he' as well, this is the classic Aho-Corasick example
            var matcher = AhoCorasickMatcher.For(["he", "she", "his", "hers"]);

            Assert.Multiple(() =>
                                 {
                                     Assert.That(matcher.IsMatch("he".AsSpan()), Is.True);
                                     Assert.That(matcher.IsMatch("she".AsSpan()), Is.True);
                                     Assert.That(matcher.IsMatch("his".AsSpan()), Is.True);
                                     Assert.That(matcher.IsMatch("hers".AsSpan()), Is.True);
                                     Assert.That(matcher.IsMatch("ushers".AsSpan()), Is.True);
                                     Assert.That(matcher.IsMatch("t".AsSpan()), Is.False);
                                 });
        }

        [Test]
        public static void IsMatch_finds_pattern_that_only_matches_via_a_failure_link_at_the_seam_of_two_patterns()
        {
            // the combined text does not contain 'abc' as a contiguous substring of either single pattern alone,
            // but the seam between 'xab' and 'cx' does contain it, exercising the failure-link fallback
            var matcher = AhoCorasickMatcher.For(["abc"]);

            Assert.That(matcher.IsMatch("xabcx".AsSpan()), Is.True);
        }

        [Test]
        public static void IsMatch_finds_overlapping_patterns_where_one_pattern_is_a_suffix_of_another()
        {
            var matcher = AhoCorasickMatcher.For(["aaa", "aa"]);

            Assert.That(matcher.IsMatch("aaa".AsSpan()), Is.True);
        }

        [Test]
        public static void IsMatch_finds_shorter_pattern_hidden_inside_a_longer_non_matching_context()
        {
            var matcher = AhoCorasickMatcher.For(["needle"]);

            Assert.That(matcher.IsMatch("a haystack containing a needle somewhere".AsSpan()), Is.True);
        }

        [Test]
        public static void IsMatch_ignores_null_entries_within_the_patterns()
        {
            var matcher = AhoCorasickMatcher.For(["abc", null, "def"]);

            Assert.Multiple(() =>
                                 {
                                     Assert.That(matcher.IsMatch("abc".AsSpan()), Is.True);
                                     Assert.That(matcher.IsMatch("def".AsSpan()), Is.True);
                                 });
        }

        [Test]
        public static void IsMatch_ignores_empty_entries_within_the_patterns()
        {
            var matcher = AhoCorasickMatcher.For(["abc", string.Empty, "def"]);

            Assert.Multiple(() =>
                                 {
                                     Assert.That(matcher.IsMatch("abc".AsSpan()), Is.True);
                                     Assert.That(matcher.IsMatch("def".AsSpan()), Is.True);
                                     Assert.That(matcher.IsMatch(string.Empty.AsSpan()), Is.False);
                                 });
        }

        [Test]
        public static void IsMatch_returns_false_when_only_empty_and_null_patterns_are_provided()
        {
            var matcher = AhoCorasickMatcher.For([string.Empty, null]);

            Assert.That(matcher.IsMatch("any text".AsSpan()), Is.False);
        }

        [Test]
        public static void IsMatch_is_case_sensitive_and_uses_ordinal_comparison()
        {
            var matcher = AhoCorasickMatcher.For(["ABC"]);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(matcher.IsMatch("ABC".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("abc".AsSpan()), Is.False);
            }
        }

        [Test]
        public static void IsMatch_returns_false_for_text_consisting_only_of_characters_outside_the_pattern_alphabet()
        {
            var matcher = AhoCorasickMatcher.For(["abc"]);

            Assert.That(matcher.IsMatch("xyz123".AsSpan()), Is.False);
        }

        [Test]
        public static void IsMatch_can_be_queried_repeatedly_with_consistent_results()
        {
            var matcher = AhoCorasickMatcher.For(["abc", "def"]);

            foreach (var attempt in Enumerable.Range(0, 5))
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(matcher.IsMatch("contains abc".AsSpan()), Is.True, $"Failed in attempt {attempt}");
                    Assert.That(matcher.IsMatch("contains nothing".AsSpan()), Is.False, $"Failed in attempt {attempt}");
                }
            }
        }

        [Test]
        public static void For_throws_an_exception_when_patterns_is_null() => Assert.That(() => AhoCorasickMatcher.For(null), Throws.ArgumentNullException);
    }
}
