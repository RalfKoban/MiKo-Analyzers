using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

using MiKoSolutions.Analyzers.Linguistics;

// for performance reasons we switch of RDI and NCrunch instrumentation
//// ncrunch: rdi off
//// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="StringBuilder"/>s.
    /// </summary>
    internal static class StringBuilderExtensions
    {
        /// <summary>
        /// The minimum length threshold used for optimized substring probing operations.
        /// </summary>
        private const int QuickSubstringProbeLengthThreshold = 8;

        /// <summary>
        /// The shared character array pool used for temporary buffer allocations to improve performance and reduce memory allocations.
        /// </summary>
        private static readonly ArrayPool<char> SharedPool = ArrayPool<char>.Shared;

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where the first word is adjusted according to the specified adjustment.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="adjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where the first word is adjusted according to the specified adjustment.
        /// </returns>
        public static StringBuilder AdjustFirstWord(this StringBuilder value, in FirstWordAdjustment adjustment)
        {
            if (value.IsNullOrEmpty() || value[0] is '<')
            {
                return value;
            }

            // only keep it if there is already a leading space (otherwise it may be on the same line without any leading space, and we would fix it in a wrong way)
            value.TrimLeadingSpacesTo(adjustment.HasSet(FirstWordAdjustment.KeepSingleLeadingSpace) ? 1 : 0);

            if (adjustment.HasSet(FirstWordAdjustment.StartLowerCase))
            {
                value.StartLowerCase();
            }
            else if (adjustment.HasSet(FirstWordAdjustment.StartUpperCase))
            {
                value.StartUpperCase();
            }

            if (adjustment.HasSet(FirstWordAdjustment.MakeInfinite))
            {
                value.MakeInfinite();
            }
            else if (adjustment.HasSet(FirstWordAdjustment.MakePlural))
            {
                value.MakePlural();
            }
            else if (adjustment.HasSet(FirstWordAdjustment.MakeThirdPersonSingular))
            {
                value.MakeThirdPersonSingular();
            }

            return value;
        }

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where the word following the specified phrase is adjusted according to the specified adjustment.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="phrase">
        /// The phrase after which the word gets adjusted.
        /// </param>
        /// <param name="adjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the word following the phrase.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where the word following the specified phrase is adjusted according to the specified adjustment.
        /// </returns>
        public static StringBuilder AdjustWordAfter(this StringBuilder value, string phrase, in FirstWordAdjustment adjustment)
        {
            if (phrase.IsNullOrEmpty())
            {
                return value;
            }

            var phraseStartCharacter = phrase[0];

            var phraseStartIndex = IndexOf(value, phraseStartCharacter);

            if (phraseStartIndex <= -1)
            {
                return value;
            }

            var phraseLength = phrase.Length;
            var phraseEndCharacter = phrase[phraseLength - 1];

            for (int index = phraseStartIndex, length = value.Length; index < length; index++)
            {
                if (value[index] != phraseStartCharacter)
                {
                    continue;
                }

                var phraseEndIndex = index + phraseLength - 1;

                if (phraseEndIndex >= length || value[phraseEndIndex] != phraseEndCharacter)
                {
                    continue;
                }

                if (value.ToString(index, phraseLength).Equals(phrase, StringComparison.Ordinal))
                {
                    // get next word (separated by '_', or by ' ' for sentences)
                    var nextWordStartIndex = phraseEndIndex + 1;
                    var nextWordEndIndex = value.IndexOf('_', ' ', nextWordStartIndex) - 1;

                    if (nextWordEndIndex > 0)
                    {
                        var nextWordLength = nextWordEndIndex - nextWordStartIndex + 1;

                        var nextWord = value.ToString(nextWordStartIndex, nextWordLength);
                        var adjustedWord = nextWord.AdjustFirstWord(adjustment);

                        // cut it out
                        value.Remove(nextWordStartIndex, nextWordLength);

                        // insert word adjusted by 'handling' using 'AdjustFirstWord'
                        value.Insert(nextWordStartIndex, adjustedWord);

                        return value;
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Determines whether the text ends with the specified ending.
        /// </summary>
        /// <param name="value">
        /// The text to check.
        /// </param>
        /// <param name="ending">
        /// The ending to check for.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the comparison rules to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the text ends with the specified ending; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool EndsWith(this StringBuilder value, string ending, in StringComparison comparison = StringComparison.Ordinal)
        {
            if (ending.IsNullOrEmpty())
            {
                return false;
            }

            var valueLength = value.Length;
            var endingLength = ending.Length;

            if (valueLength >= endingLength)
            {
                var end = value.ToString(valueLength - endingLength, endingLength);

                return end.AsSpan().Equals(ending.AsSpan(), comparison);
            }

            return false;
        }

        /// <summary>
        /// Gets the first word from the text.
        /// </summary>
        /// <param name="value">
        /// The text to get the first word from.
        /// </param>
        /// <param name="whitespacesBefore">
        /// On successful return, contains the number of whitespaces before the first word.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the first word from the text.
        /// </returns>
        public static string FirstWord(this StringBuilder value, out int whitespacesBefore)
        {
            if (value.IsNullOrEmpty())
            {
                whitespacesBefore = 0;

                return string.Empty;
            }

            // 1. Find word begin
            whitespacesBefore = value.CountLeadingWhitespaces();

            // 2. Find word end
            var wordLength = 0;

            for (int i = whitespacesBefore, length = value.Length; i < length; i++)
            {
                var c = value[i];

                if (c.IsWhiteSpace() || c.IsSentenceEnding())
                {
                    break;
                }

                wordLength++;
            }

            // 3. Cut word
            return value.ToString(whitespacesBefore, wordLength);
        }

        /// <summary>
        /// Determines whether the text contains any whitespace characters.
        /// </summary>
        /// <param name="value">
        /// The text to check.
        /// </param>
        /// <param name="start">
        /// The zero-based starting position to begin checking.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the text contains any whitespace characters; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasWhitespaces(this StringBuilder value, int start = 0)
        {
            if (value.IsNullOrEmpty())
            {
                return false;
            }

            for (var length = value.Length; start < length; start++)
            {
                if (value[start].IsWhiteSpace())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the text is <see langword="null"/> or empty.
        /// </summary>
        /// <param name="value">
        /// The text to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the text is <see langword="null"/> or empty; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this StringBuilder value) => value is null || value.Length is 0;

        /// <summary>
        /// Determines whether the text is <see langword="null"/>, empty, or consists only of whitespace characters.
        /// </summary>
        /// <param name="value">
        /// The text to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the text is <see langword="null"/>, empty, or consists only of whitespace characters; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsNullOrWhiteSpace(this StringBuilder value) => value is null || value.CountLeadingWhitespaces() == value.Length;

        /// <summary>
        /// Determines whether the text consists of a single word without any whitespace characters.
        /// </summary>
        /// <param name="value">
        /// The text to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the text consists of a single word without any whitespace characters; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsSingleWord(this StringBuilder value) => value?.HasWhitespaces() is false;

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where all occurrences of the specified keys are replaced with their corresponding values.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="replacementPairs">
        /// The key-value pairs for replacement.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where all occurrences of the specified keys are replaced with their corresponding values.
        /// </returns>
        public static StringBuilder ReplaceAllWithProbe(this StringBuilder value, in ReadOnlySpan<Pair> replacementPairs)
        {
            char[] text = null;
            var textSpan = GetTextAsRentedArray(ref value, ref text);

            for (int index = 0, count = replacementPairs.Length; index < count; index++)
            {
                var pair = replacementPairs[index];

                if (textSpan.Length < pair.Key.Length)
                {
                    // cannot be part in the replacement as other value is too long and cannot fit current value
                    continue;
                }

                var replaceStartIndex = QuickSubstringProbe(textSpan, pair.Key.AsSpan());

                if (replaceStartIndex is -1)
                {
                    continue;
                }

                // can be part in the replacement as value seems to fit
                value.Replace(pair.Key, pair.Value, replaceStartIndex, value.Length - replaceStartIndex);

                // re-assign text as we might have replaced the string, but use a different array
                textSpan = GetTextAsRentedArray(ref value, ref text);
            }

            if (text != null)
            {
                SharedPool.Return(text);
            }

            return value;
        }

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where all occurrences of the specified texts are replaced with the specified replacement.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="texts">
        /// The texts to replace.
        /// </param>
        /// <param name="replacement">
        /// The replacement text.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where all occurrences of the specified texts are replaced with the specified replacement.
        /// </returns>
        public static StringBuilder ReplaceAllWithProbe(this StringBuilder value, in ReadOnlySpan<string> texts, string replacement)
        {
            char[] text = null;
            var textSpan = GetTextAsRentedArray(ref value, ref text);

            for (int index = 0, length = texts.Length; index < length; index++)
            {
                var oldValue = texts[index];

                var replaceStartIndex = QuickSubstringProbe(textSpan, oldValue.AsSpan());

                if (replaceStartIndex is -1)
                {
                    continue;
                }

                // can be part in the replacement as value seems to fit
                value.Replace(oldValue, replacement, replaceStartIndex, value.Length - replaceStartIndex);

                // re-assign text as we might have replaced the string, but use a different array
                textSpan = GetTextAsRentedArray(ref value, ref text);
            }

            if (text != null)
            {
                SharedPool.Return(text);
            }

            return value;
        }

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where the first occurrence of the specified value is replaced with the specified new value.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="oldValue">
        /// The text to replace.
        /// </param>
        /// <param name="newValue">
        /// The replacement text.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where the first occurrence of the specified value is replaced with the specified new value.
        /// </returns>
        public static StringBuilder ReplaceWithProbe(this StringBuilder value, string oldValue, string newValue)
        {
            if (oldValue.IsNullOrEmpty())
            {
                // cannot replace an empty value
                return value;
            }

            char[] text = null;
            var textSpan = GetTextAsRentedArray(ref value, ref text);

            var replaceStartIndex = QuickSubstringProbe(textSpan, oldValue.AsSpan());

            // we do not need the text anymore
            if (text != null)
            {
                SharedPool.Return(text);
            }

            if (replaceStartIndex is -1)
            {
                // cannot be part in the replacement as value does not fit
                return value;
            }

            return value.Replace(oldValue, newValue, replaceStartIndex, value.Length - replaceStartIndex);
        }

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where words are separated by the specified separator character.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="separator">
        /// The separator character to use between words.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where words are separated by the specified separator character.
        /// </returns>
        public static StringBuilder SeparateWords(this StringBuilder value, in char separator, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.None)
        {
            if (value.IsNullOrEmpty())
            {
                return value;
            }

            var multipleUpperCases = false;

            const int CharacterToStartWith = 1;

            for (var index = CharacterToStartWith; index < value.Length; index++)
            {
                var c = value[index];

                if (c == separator)
                {
                    // keep the existing separator
                    continue;
                }

                var isUpperCaseC = c.IsUpperCase();

                if (isUpperCaseC || c.IsNumber())
                {
                    if (index == CharacterToStartWith)
                    {
                        // multiple upper cases in a line at beginning of the name, so do not flip
                        multipleUpperCases = true;
                    }

                    var previousC = value[index - 1];

                    if (multipleUpperCases)
                    {
                        // let's see if we start with an IXyz interface
                        if (previousC is 'I')
                        {
                            // seems we are in an IXyz interface
                            multipleUpperCases = false;

                            continue;
                        }

                        if (isUpperCaseC && previousC.IsNumber())
                        {
                            // nothing to do here
                        }
                        else
                        {
                            // multiple upper cases in a line, so do not flip
                            continue;
                        }
                    }

                    // let's consider an upper-case 'A' as a special situation as that is a single word
                    var isSpecialCharA = c is 'A';

                    multipleUpperCases = isSpecialCharA is false;

                    var nextC = c.ToLowerCase();

                    var nextIndex = index + 1;

                    if ((nextIndex >= value.Length || (nextIndex < value.Length && value[nextIndex].IsUpperCaseLetter())) && isSpecialCharA is false)
                    {
                        // multiple upper cases in a line, so do not flip
                        nextC = c;
                    }

                    if (previousC == separator)
                    {
                        value[index] = nextC;
                    }
                    else
                    {
                        // only add an underline if we not already have one
                        value[index] = separator;
                        index++;

                        value.Insert(index, nextC);
                    }
                }
                else
                {
                    if (multipleUpperCases)
                    {
                        if (value[index - 1].IsUpperCaseLetter())
                        {
                            // we are behind multiple upper cases in a line, so add an underline
                            value[index] = separator;
                            index++;

                            value.Insert(index, c);
                        }
                    }

                    multipleUpperCases = false;
                }
            }

            return value.AdjustFirstWord(firstWordAdjustment);
        }

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where the specified character is lower-case.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="index">
        /// The zero-based index inside <paramref name="value"/> that is changed to lower-case.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where the specified character at <paramref name="index"/> is lower-case.
        /// </returns>
        public static StringBuilder ToLowerCaseAt(this StringBuilder value, in int index)
        {
            if (value != null && index >= 0 && index < value.Length)
            {
                value[index] = value[index].ToLowerCase();
            }

            return value;
        }

        /// <summary>
        /// Converts the text to a string and releases the <see cref="StringBuilder"/> back to the cache.
        /// </summary>
        /// <param name="value">
        /// The text to convert.
        /// </param>
        /// <returns>
        /// A string representation of the text.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToStringAndRelease(this StringBuilder value) => StringBuilderCache.GetStringAndRelease(value);

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where the specified character is upper-case.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="index">
        /// The zero-based index inside <paramref name="value"/> that is changed to upper-case.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where the specified character at <paramref name="index"/> is upper-case.
        /// </returns>
        public static StringBuilder ToUpperCaseAt(this StringBuilder value, in int index)
        {
            if (value != null && index >= 0 && index < value.Length)
            {
                value[index] = value[index].ToUpperCase();
            }

            return value;
        }

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where all leading and trailing whitespace characters are removed.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where all leading and trailing whitespace characters are removed.
        /// </returns>
        public static StringBuilder Trimmed(this StringBuilder value)
        {
            var length = value.Length;

            if (length is 0)
            {
                return value;
            }

            var start = value.CountLeadingWhitespaces();
            var end = value.CountTrailingWhitespaces(start);

            if (end > 0)
            {
                value.Remove(length - end, end);
            }

            if (start > 0)
            {
                value.Remove(0, start);
            }

            return value;
        }

        /// <summary>
        /// Gets a string where all leading and trailing whitespace characters are removed.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text where all leading and trailing whitespace characters are removed.
        /// </returns>
        public static string Trim(this StringBuilder value)
        {
            var length = value.Length;

            if (length is 0)
            {
                return string.Empty;
            }

            var start = value.CountLeadingWhitespaces();
            var end = value.CountTrailingWhitespaces(start);

            return value.ToString(start, length - start - end);
        }

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where all leading whitespace characters are removed.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where all leading whitespace characters are removed.
        /// </returns>
        public static StringBuilder TrimmedStart(this StringBuilder value)
        {
            var length = value.Length;

            if (length is 0)
            {
                return value;
            }

            var start = value.CountLeadingWhitespaces();

            return value.Remove(0, start);
        }

        /// <summary>
        /// Gets a string where all leading whitespace characters are removed.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text where all leading whitespace characters are removed.
        /// </returns>
        public static string TrimStart(this StringBuilder value)
        {
            var length = value.Length;

            if (length is 0)
            {
                return string.Empty;
            }

            var start = value.CountLeadingWhitespaces();

            return value.ToString(start, length - start);
        }

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where all leading occurrences of the specified characters are removed.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="characters">
        /// The characters to remove from the start.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where all leading occurrences of the specified characters are removed.
        /// </returns>
        public static StringBuilder TrimStart(this StringBuilder value, char[] characters)
        {
            for (int index = 0, length = characters.Length; index < length; index++)
            {
                if (value.Length > 0 && value[0] == characters[index])
                {
                    value.Remove(0, 1);
                }
            }

            return value;
        }

        /// <summary>
        /// Gets a string where all trailing whitespace characters are removed.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text where all trailing whitespace characters are removed.
        /// </returns>
        public static string TrimEnd(this StringBuilder value)
        {
            var length = value.Length;

            if (length is 0)
            {
                return string.Empty;
            }

            var end = value.CountTrailingWhitespaces();

            return value.ToString(0, length - end);
        }

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where all trailing occurrences of the specified characters are removed.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="characters">
        /// The characters to remove from the end.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where all trailing occurrences of the specified characters are removed.
        /// </returns>
        public static StringBuilder TrimEnd(this StringBuilder value, char[] characters)
        {
            for (int index = 0, length = characters.Length; index < length; index++)
            {
                var i = value.Length - 1;

                if (i >= 0 && value[i] == characters[index])
                {
                    value.Remove(i, 1);
                }
            }

            return value;
        }

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where the specified number of characters are removed from the end.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="count">
        /// The number of characters to remove from the end.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where the specified number of characters are removed from the end.
        /// </returns>
        public static StringBuilder TrimEndBy(this StringBuilder value, in int count)
        {
            if (count is 0)
            {
                return value;
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Should be greater than zero.");
            }

            var length = value.Length;

            if (count > length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, $"Should be less than {length}.");
            }

            return value.Remove(length - count, count);
        }

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where all trailing whitespace characters are removed.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where all trailing whitespace characters are removed.
        /// </returns>
        public static StringBuilder TrimmedEnd(this StringBuilder value)
        {
            var length = value.Length;

            if (length is 0)
            {
                return value;
            }

            var end = value.CountTrailingWhitespaces();

            if (end is 0)
            {
                return value;
            }

            return value.Remove(length - end, end);
        }

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where multiple consecutive whitespace characters are replaced with a single whitespace character.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where multiple consecutive whitespace characters are replaced with a single whitespace character.
        /// </returns>
        public static StringBuilder WithoutMultipleWhiteSpaces(this StringBuilder value) => value.ReplaceAllWithProbe(Constants.Comments.MultiWhitespaceStrings, Constants.Comments.SingleWhitespaceString);

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where all new line characters are removed.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where all new line characters are removed.
        /// </returns>
        public static StringBuilder WithoutNewLines(this StringBuilder value) => value.Without('\r', '\n');

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where all occurrences of the specified character are removed.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="c">
        /// The character to remove.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where all occurrences of the specified character are removed.
        /// </returns>
        public static StringBuilder Without(this StringBuilder value, in char c)
        {
            for (var position = value.Length - 1; position > -1; position--)
            {
                if (value[position] == c)
                {
                    value.Remove(position, 1);
                }
            }

            return value;
        }

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where all occurrences of the specified phrase are removed.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="phrase">
        /// The phrase to remove.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where all occurrences of the specified phrase are removed.
        /// </returns>
        public static StringBuilder Without(this StringBuilder value, string phrase) => value.ReplaceWithProbe(phrase, string.Empty);

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where all occurrences of the specified phrases are removed.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="phrases">
        /// The phrases to remove.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where all occurrences of the specified phrases are removed.
        /// </returns>
        public static StringBuilder Without(this StringBuilder value, in ReadOnlySpan<string> phrases) => value.ReplaceAllWithProbe(phrases, string.Empty);

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where all occurrences of the specified characters are removed.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="c1">
        /// The first character to remove.
        /// </param>
        /// <param name="c2">
        /// The second character to remove.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where all occurrences of the specified characters are removed.
        /// </returns>
        public static StringBuilder Without(this StringBuilder value, in char c1, in char c2)
        {
            for (var position = value.Length - 1; position > -1; position--)
            {
                var c = value[position];

                if (c == c1 || c == c2)
                {
                    value.Remove(position, 1);
                }
            }

            return value;
        }

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where leading occurrences of the specified phrases are removed.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="phrases">
        /// The phrases to remove from the start.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the comparison rules to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where leading occurrences of the specified phrases are removed.
        /// </returns>
        public static StringBuilder WithoutStarting(this StringBuilder value, in ReadOnlySpan<string> phrases, in StringComparison comparison = StringComparison.Ordinal)
        {
            foreach (var phrase in phrases)
            {
                var length = phrase.Length;

                if (value.Length > length)
                {
                    if (value.ToString(0, length).Equals(phrase, comparison))
                    {
                        value.Remove(0, length);
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where all abbreviations are replaced with their expanded forms.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where all abbreviations are replaced with their expanded forms.
        /// </returns>
        public static StringBuilder WithoutAbbreviations(this StringBuilder value) => AbbreviationFinder.FindAndReplaceAllAbbreviations(value);

        /// <summary>
        /// Gets a <see cref="StringBuilder"/> where all paragraph tags are removed.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> where all paragraph tags are removed.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder WithoutParaTags(this StringBuilder value) => value.Without(Constants.ParaTags);

        /// <summary>
        /// Gets the text as a rented character array from the shared pool.
        /// </summary>
        /// <param name="value">
        /// The text to convert.
        /// </param>
        /// <param name="text">
        /// A reference to the rented character array to use, or <see langword="null"/> if a new array should be rented.
        /// This parameter may be updated with a newly rented array if the existing one is too small.
        /// </param>
        /// <returns>
        /// A read-only span of characters representing the text.
        /// </returns>
        private static ReadOnlySpan<char> GetTextAsRentedArray(ref StringBuilder value, ref char[] text)
        {
            var size = value.Length;

            if (text is null)
            {
                text = SharedPool.Rent(size);
            }
            else
            {
                if (text.Length < size)
                {
                    SharedPool.Return(text);

                    text = SharedPool.Rent(size);
                }
            }

            value.CopyTo(0, text, 0, size);

            return text.AsSpan(0, size); // use the length of the string builder as the rented array itself may be longer than the requested size, thus leading to unexpected behavior
        }

        /// <summary>
        /// Determines the starting position where the specified text could potentially occur within the current text, using a quick probe optimization.
        /// </summary>
        /// <param name="current">
        /// The current text to search within.
        /// </param>
        /// <param name="other">
        /// The text to search for.
        /// </param>
        /// <returns>
        /// The zero-based starting position where the specified text might occur, or -1 if it does not occur.
        /// </returns>
        private static int QuickSubstringProbe(in ReadOnlySpan<char> current, in ReadOnlySpan<char> other)
        {
            var delta = current.Length - other.Length;

            if (delta < 0)
            {
                // cannot be part in the replacement as other value is too long and cannot fit current value
                return -1;
            }

            if (other.Length < QuickSubstringProbeLengthThreshold)
            {
                // can be part in the replacement as other value is smaller and could fit current value
                return 0;
            }

            var lastIndex = other.Length - 1;
            var startChar = other[0];
            var endChar = other[lastIndex];

            if (lastIndex < QuickSubstringProbeLengthThreshold)
            {
                // Performance-Note:
                // - do not use a separate if condition for the delta being zero as that may not happen often and the conditional check therefore is too costly
                for (var position = 0; position <= delta; position++)
                {
                    // Performance-Note:
                    // - do not split or re-calculate last index position each time as this gets invoked millions of time and re-calculation is too costly in such situation
                    if (current[position + lastIndex] == endChar && current[position /* + 0 */] == startChar)
                    {
                        // could be part in the replacement as characters match both at start and end
                        return position;
                    }
                }
            }
            else
            {
                var middleChar = other[QuickSubstringProbeLengthThreshold];

                // Performance-Note:
                // - do not use a separate if condition for the delta being zero as that may not happen often and the conditional check therefore is too costly
                for (var position = 0; position <= delta; position++)
                {
                    // Performance-Note:
                    // - do not split or re-calculate last index position each time as this gets invoked millions of time and re-calculation is too costly in such situation
                    if (current[position + lastIndex] == endChar && current[position /* + 0 */] == startChar && current[position + QuickSubstringProbeLengthThreshold] == middleChar)
                    {
                        // could be part in the replacement as characters match both at start and end
                        return position;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Counts the number of leading whitespace characters in the text.
        /// </summary>
        /// <param name="value">
        /// The text to check.
        /// </param>
        /// <param name="start">
        /// The zero-based starting position to begin counting.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// The number of leading whitespace characters in the text.
        /// </returns>
        private static int CountLeadingWhitespaces(this StringBuilder value, int start = 0)
        {
            var whitespaces = 0;

            for (var valueLength = value.Length; start < valueLength; start++)
            {
                if (value[start].IsWhiteSpace())
                {
                    whitespaces++;
                }
                else
                {
                    break;
                }
            }

            return whitespaces;
        }

        /// <summary>
        /// Counts the number of trailing whitespace characters in the text.
        /// </summary>
        /// <param name="value">
        /// The text to check.
        /// </param>
        /// <param name="start">
        /// The zero-based position up to which trailing whitespaces are counted (counts backwards from the end).
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// The number of trailing whitespace characters in the text.
        /// </returns>
        private static int CountTrailingWhitespaces(this StringBuilder value, in int start = 0)
        {
            var whitespaces = 0;

            for (var i = value.Length - 1; i >= start; i--)
            {
                if (value[i].IsWhiteSpace())
                {
                    whitespaces++;
                }
                else
                {
                    break;
                }
            }

            return whitespaces;
        }

        /// <summary>
        /// Converts the first non-whitespace character in the text to lower-case.
        /// </summary>
        /// <param name="value">
        /// The text to modify.
        /// </param>
        private static void StartLowerCase(this StringBuilder value)
        {
            for (int i = 0, length = value.Length; i < length; i++)
            {
                var c = value[i];

                if (c.IsWhiteSpace())
                {
                    continue;
                }

                if (c.IsUpperCase())
                {
                    value[i] = c.ToLowerCase();
                }

                // we found it, so nothing more to do
                return;
            }
        }

        /// <summary>
        /// Converts the first non-whitespace character in the text to upper-case.
        /// </summary>
        /// <param name="value">
        /// The text to modify.
        /// </param>
        private static void StartUpperCase(this StringBuilder value)
        {
            for (int i = 0, length = value.Length; i < length; i++)
            {
                var c = value[i];

                if (c.IsWhiteSpace())
                {
                    continue;
                }

                if (c.IsLowerCase())
                {
                    value[i] = c.ToUpperCase();
                }

                // we found it, so nothing more to do
                return;
            }
        }

        /// <summary>
        /// Converts the first word in the text to its infinite verb form.
        /// </summary>
        /// <param name="value">
        /// The text to modify.
        /// </param>
        private static void MakeInfinite(this StringBuilder value)
        {
            var word = FirstWord(value, out var whitespacesBefore);

            value.Remove(whitespacesBefore, word.Length);

            var infiniteWord = Verbalizer.MakeInfiniteVerb(word);

            value.Insert(whitespacesBefore, infiniteWord);
        }

        /// <summary>
        /// Converts the first word in the text to its plural form.
        /// </summary>
        /// <param name="value">
        /// The text to modify.
        /// </param>
        private static void MakePlural(this StringBuilder value)
        {
            var word = FirstWord(value, out var whitespacesBefore);

            value.Remove(whitespacesBefore, word.Length);

            var pluralWord = Pluralizer.MakePluralName(word);

            value.Insert(whitespacesBefore, pluralWord);
        }

        /// <summary>
        /// Converts the first word in the text to its third person singular verb form.
        /// </summary>
        /// <param name="value">
        /// The text to modify.
        /// </param>
        private static void MakeThirdPersonSingular(this StringBuilder value)
        {
            var word = FirstWord(value, out var whitespacesBefore);

            value.Remove(whitespacesBefore, word.Length);

            var pluralWord = Verbalizer.MakeThirdPersonSingularVerb(word);

            value.Insert(whitespacesBefore, pluralWord);
        }

        /// <summary>
        /// Trims the leading spaces in the text to the specified count.
        /// </summary>
        /// <param name="value">
        /// The text to modify.
        /// </param>
        /// <param name="count">
        /// The number of leading spaces to keep. If fewer spaces exist, nothing is done.
        /// </param>
        private static void TrimLeadingSpacesTo(this StringBuilder value, in int count)
        {
            if (value[0] is ' ')
            {
                var whitespaces = value.CountLeadingWhitespaces();

                if (whitespaces > count)
                {
                    value.Remove(count, whitespaces - count);
                }
            }
        }

        /// <summary>
        /// Finds the first occurrence of the specified character in the text.
        /// </summary>
        /// <param name="value">
        /// The text to search.
        /// </param>
        /// <param name="c">
        /// The character to search for.
        /// </param>
        /// <param name="start">
        /// The zero-based starting position to begin searching.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of the specified character, or -1 if it is not found.
        /// </returns>
        private static int IndexOf(this StringBuilder value, in char c, in int start = 0)
        {
            for (int index = start, length = value.Length; index < length; index++)
            {
                if (value[index] == c)
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds the first occurrence of either of the specified characters in the text.
        /// </summary>
        /// <param name="value">
        /// The text to search.
        /// </param>
        /// <param name="c1">
        /// The first character to search for.
        /// </param>
        /// <param name="c2">
        /// The second character to search for.
        /// </param>
        /// <param name="start">
        /// The zero-based starting position to begin searching.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of either specified character, or -1 if neither is found.
        /// </returns>
        private static int IndexOf(this StringBuilder value, in char c1, in char c2, in int start = 0)
        {
            for (int index = start, length = value.Length; index < length; index++)
            {
                var c = value[index];

                if (c == c1 || c == c2)
                {
                    return index;
                }
            }

            return -1;
        }
    }
}