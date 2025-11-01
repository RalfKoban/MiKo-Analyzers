using System;

namespace MiKoSolutions.Analyzers.Linguistics
{
    /// <summary>
    /// Provides functionality to identify adjectives and adverbs, and to derive adjectives from nouns.
    /// </summary>
    internal static class AdjectiveFinder
    {
        private static readonly string[] AdjectivesOrAdverbs =
                                                               {
                                                                   "about",
                                                                   "afterwards",
                                                                   "already",
                                                                   "also",
                                                                   "always",
                                                                   "at",
                                                                   "before",
                                                                   "clear",
                                                                   "either",
                                                                   "first",
                                                                   "however",
                                                                   "in",
                                                                   "insane",
                                                                   "just",
                                                                   "later",
                                                                   "longer",
                                                                   "no",
                                                                   "not",
                                                                   "now",
                                                                   "off",
                                                                   "on",
                                                                   "out",
                                                                   "than",
                                                                   "then",
                                                                   "therefore",
                                                                   "to",
                                                                   "turn",
                                                                   "vain",
                                                               };

        /// <summary>
        /// Determines whether the specified value is an adjective or adverb.
        /// </summary>
        /// <param name="value">
        /// The value to check.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the comparison type to use when comparing the value.
        /// The default is <see cref="StringComparison.OrdinalIgnoreCase"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the value is an adjective or adverb; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsAdjectiveOrAdverb(in ReadOnlySpan<char> value, in StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value.EndsWith("y", comparison))
            {
                if (value.EndsWith("ly", comparison))
                {
                    if (value.EndsWith("ply", comparison))
                    {
                        return value.Equals("simply", comparison);
                    }

                    return true;
                }

                return value.EndsWith("dy", comparison);
            }

            if (value.EndsWith("ble", comparison))
            {
                return true;
            }

            if (value.EndsWith("mple", comparison))
            {
                return true;
            }

            if (value.EndsWith("ive", comparison))
            {
                return value.EndsWith("hive", comparison) is false;
            }

            if (value.EndsWith("ile", comparison))
            {
                if (value.EndsWith("gile", comparison))
                {
                    return true;
                }

                if (value.EndsWith("tile", comparison))
                {
                    return true;
                }

                if (value.EndsWith("bile", comparison))
                {
                    return true;
                }

                return false; // "file" or "mile"
            }

            if (value.EndsWith("ous", comparison))
            {
                return true;
            }

            if (value.EndsWith("al", comparison)) // "cal", "nal" or "tal"
            {
                return true;
            }

            if (value.EndsWith("ic", comparison)) // "tic" or "tric"
            {
                return true;
            }

            if (value.EndsWith("iar", comparison))
            {
                return true;
            }

            if (value.EndsWith("lar", comparison))
            {
                return true;
            }

            if (value.EndsWith("plex", comparison))
            {
                return true;
            }

            if (value.EndsWith("ure", comparison))
            {
                return value.EndsWith("cure", comparison) is false;
            }

            if (value.EndsWith("nse", comparison))
            {
                return value.EndsWith("mmense", comparison) || value.EndsWith("intense", comparison);
            }

            return value.EqualsAny(AdjectivesOrAdverbs, comparison);
        }

        /// <summary>
        /// Gets the adjective form for the specified noun.
        /// </summary>
        /// <param name="value">
        /// The noun value to convert.
        /// </param>
        /// <param name="handling">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first character of the result.
        /// The default is <see cref="FirstWordAdjustment.StartUpperCase"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the adjective form of the noun, or the <see cref="string.Empty"/> string ("") if no conversion is available.
        /// </returns>
        public static string GetAdjectiveForNoun(in ReadOnlySpan<char> value, in FirstWordAdjustment handling = FirstWordAdjustment.StartUpperCase)
        {
            var result = GetAdjectiveForNounCore(value);

            return handling is FirstWordAdjustment.StartUpperCase
                   ? result.ToUpperCaseAt(0)
                   : result.ToLowerCaseAt(0);
        }

        /// <summary>
        /// Gets the adjective form for the specified noun.
        /// </summary>
        /// <param name="value">
        /// The noun value to convert.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the adjective form of the noun, or the <see cref="string.Empty"/> string ("") if no conversion is available.
        /// </returns>
        private static string GetAdjectiveForNounCore(in ReadOnlySpan<char> value)
        {
            if (value.EndsWith("ty"))
            {
                if (value.EndsWith("ity"))
                {
                    if (value.EndsWith("ility"))
                    {
                        if (value.EndsWith("bility"))
                        {
                            if (value.EndsWith("obility") is false || value.EndsWith("nobility", StringComparison.OrdinalIgnoreCase))
                            {
                                return value.Slice(0, value.Length - 5).ConcatenatedWith("le");
                            }
                        }

                        return value.Slice(0, value.Length - 3).ConcatenatedWith('e');
                    }

                    if (value.EndsWith("pitality"))
                    {
                        return value.Slice(0, value.Length - 4).ConcatenatedWith("ble");
                    }

                    if (value.EndsWith("plicity"))
                    {
                        return value.Slice(0, value.Length - 5).ConcatenatedWith('e');
                    }

                    if (value.EndsWith("tricity") || value.EndsWith("ticity"))
                    {
                        return value.Slice(0, value.Length - 3).ToString();
                    }

                    if (value.EndsWith("ivity") || value.EndsWith("sanity"))
                    {
                        return value.Slice(0, value.Length - 3).ConcatenatedWith('e');
                    }

                    if (value.EndsWith("ality"))
                    {
                        if (value.StartsWith("In"))
                        {
                            return 'U'.ConcatenatedWith(value.Slice(1, value.Length - 4));
                        }

                        if (value.StartsWith("in"))
                        {
                            return 'u'.ConcatenatedWith(value.Slice(1, value.Length - 4));
                        }

                        return value.Slice(0, value.Length - 3).ToString();
                    }

                    if (value.EndsWith("larity"))
                    {
                        if (value.StartsWith('C') || value.StartsWith('c'))
                        {
                            return value.Slice(0, value.Length - 5).ConcatenatedWith("ear");
                        }
                    }

                    if (value.EndsWith("arity") || value.EndsWith("exity"))
                    {
                        return value.Slice(0, value.Length - 3).ToString();
                    }

                    if (value.EndsWith("osity"))
                    {
                        return value.Slice(0, value.Length - 4).ConcatenatedWith("us");
                    }

                    if (value.EndsWith("olity") || value.EndsWith("erity"))
                    {
                        return value.Slice(0, value.Length - 3).ConcatenatedWith("ous");
                    }

                    if (value.EndsWith("anity"))
                    {
                        return value.Slice(0, value.Length - 4).ConcatenatedWith("in");
                    }

                    if (value.EndsWith("ority"))
                    {
                        return value.Slice(0, value.Length - 2).ConcatenatedWith("tative");
                    }

                    return value.Slice(0, value.Length - 3).ConcatenatedWith('e');
                }

                return value.Slice(0, value.Length - 2).ToString();
            }

            if (value.EndsWith("iness"))
            {
                return value.Slice(0, value.Length - 5).ConcatenatedWith('y');
            }

            return string.Empty;
        }
    }
}