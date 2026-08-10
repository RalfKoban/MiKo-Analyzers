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

            using (Assert.EnterMultipleScope())
            {
                Assert.That(matcher.IsMatch("I have a cat".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("I have a dog".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("I have a bird".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("I have a fish".AsSpan()), Is.False);
            }
        }

        [Test]
        public static void IsMatch_finds_pattern_that_is_a_prefix_of_another_pattern()
        {
            // 'he' is a prefix of 'hers' and 'she' contains 'he' as well, this is the classic Aho-Corasick example
            var matcher = AhoCorasickMatcher.For(["he", "she", "his", "hers"]);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(matcher.IsMatch("he".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("she".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("his".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("hers".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("ushers".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("t".AsSpan()), Is.False);
            }
        }

        [Test]
        public static void IsMatch_finds_pattern_that_is_only_reachable_via_a_failure_link_after_a_mismatch()
        {
            // while walking "caab" against pattern "caax", matching succeeds for 'c', 'a', 'a' and then mismatches
            // on 'b' (since the trie only continues with 'x' at that point); a correct failure link must fall back to
            // the node representing "aa" (a valid prefix of pattern "aab") instead of resetting to the root, so that
            // 'b' still completes a match of "aab" (which is genuinely a contiguous substring of "caab" at index 1-3).
            // An implementation that resets to the root on any mismatch, without reprocessing 'b' from the fallback
            // state, would incorrectly report no match here.
            var matcher = AhoCorasickMatcher.For(["caax", "aab"]);

            Assert.That(matcher.IsMatch("caab".AsSpan()), Is.True);
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

            using (Assert.EnterMultipleScope())
            {
                Assert.That(matcher.IsMatch("abc".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("def".AsSpan()), Is.True);
            }
        }

        [Test]
        public static void IsMatch_ignores_empty_entries_within_the_patterns()
        {
            var matcher = AhoCorasickMatcher.For(["abc", string.Empty, "def"]);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(matcher.IsMatch("abc".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("def".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch(string.Empty.AsSpan()), Is.False);
            }
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

        [Test]
        public static void IsMatch_returns_false_for_an_empty_collection_of_patterns()
        {
            var matcher = AhoCorasickMatcher.For([]);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(matcher.IsMatch("any text".AsSpan()), Is.False);
                Assert.That(matcher.IsMatch(string.Empty.AsSpan()), Is.False);
            }
        }

        [Test]
        public static void IsMatch_finds_a_single_character_pattern()
        {
            var matcher = AhoCorasickMatcher.For(["x"]);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(matcher.IsMatch("x".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("abcxdef".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("abcdef".AsSpan()), Is.False);
            }
        }

        [Test]
        public static void IsMatch_finds_matches_when_the_same_pattern_is_specified_multiple_times()
        {
            var matcher = AhoCorasickMatcher.For(["abc", "abc", "abc"]);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(matcher.IsMatch("abc".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("xyz".AsSpan()), Is.False);
            }
        }

        [Test]
        public static void IsMatch_finds_patterns_that_contain_non_ascii_characters()
        {
            var matcher = AhoCorasickMatcher.For(["café", "日本語"]);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(matcher.IsMatch("I like café very much".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("speaking 日本語 fluently".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("no matching text here".AsSpan()), Is.False);
            }
        }

        [Test]
        public static void IsMatch_finds_every_pattern_when_many_states_share_an_identical_transition_row()
        {
            // every pattern only differs in its first character and shares the exact same single-character suffix,
            // so the intermediate nodes (reached right after the first character) all end up with an identical
            // transition row (real edge on '1' towards the terminal state, fallback to root for everything else);
            // this specifically exercises the per-state default/exception compression introduced to shrink the transition table,
            // where the shared fallback target becomes the default and only the '1' edge is stored as an exception.
            var patterns = Enumerable.Range(0, 10).Select(_ => $"{_}1").ToArray();

            var matcher = AhoCorasickMatcher.For(patterns);

            using (Assert.EnterMultipleScope())
            {
                foreach (var pattern in patterns)
                {
                    Assert.That(matcher.IsMatch($"contains {pattern} somewhere".AsSpan()), Is.True, $"Failed for pattern '{pattern}'");
                }

                Assert.That(matcher.IsMatch("contains no matching digits".AsSpan()), Is.False);
                Assert.That(matcher.IsMatch("12345".AsSpan()), Is.False); // shares digits with the patterns but never followed by the required '1' suffix at the right spot
            }
        }

        [Test]
        public static void IsMatch_finds_every_pattern_when_many_states_share_an_identical_transition_row_but_differ_in_terminal_state()
        {
            // 'a1', 'b1', ... are terminal only right after their second character, whereas the intermediate,
            // non-terminal node reached after the first character shares its transition row content with other,
            // unrelated terminal leaf states (e.g. the single-character pattern "z"); compressing per-state transitions
            // into a default plus exceptions must not accidentally also merge the (separately tracked) terminal flag.
            var matcher = AhoCorasickMatcher.For(["a1", "b1", "c1", "z"]);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(matcher.IsMatch("a1".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("b1".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("c1".AsSpan()), Is.True);
                Assert.That(matcher.IsMatch("z".AsSpan()), Is.True);

                // reaching the intermediate (non-terminal) state alone, without completing any full pattern, must not match
                Assert.That(matcher.IsMatch("a".AsSpan()), Is.False);
                Assert.That(matcher.IsMatch("b".AsSpan()), Is.False);
                Assert.That(matcher.IsMatch("c".AsSpan()), Is.False);
            }
        }

        [Test]
        public static void IsMatch_produces_correct_results_for_a_large_number_of_overlapping_patterns()
        {
            // simulates a realistic large phrase set (similar in spirit to the replacement phrases used by code fixes)
            // with lots of shared prefixes and suffixes, which is exactly the scenario that produces many states and,
            // consequently, many opportunities for states to share the same default transition target.
            var prefixes = new[] { "Gets ", "Sets ", "Gets or sets ", "gets ", "sets " };
            var suffixes = new[] { "a value ", "the value ", "an instance ", "a flag ", "the flag " };

            var patterns = prefixes.SelectMany(prefix => suffixes.Select(suffix => prefix + suffix)).ToArray();

            var matcher = AhoCorasickMatcher.For(patterns);

            using (Assert.EnterMultipleScope())
            {
                foreach (var pattern in patterns)
                {
                    Assert.That(matcher.IsMatch($"Summary: {pattern}indicating something.".AsSpan()), Is.True, $"Failed for pattern '{pattern}'");
                }

                Assert.That(matcher.IsMatch("Summary: Provides access to something.".AsSpan()), Is.False);
            }
        }
    }
}
