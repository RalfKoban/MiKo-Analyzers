using System.Buffers;
using System.Runtime.CompilerServices;

using MiKoSolutions.Analyzers;
using MiKoSolutions.Analyzers.Linguistics;

// for performance reasons we switch of RDI and NCrunch instrumentation
//// ncrunch: rdi off
//// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace System.Text
{
    internal static class StringBuilderExtensions
    {
        private const int QuickSubstringProbeLengthThreshold = 6;
        private const int QuickSubstringProbeRentLengthThreshold = 24;

        public static StringBuilder AdjustFirstWord(this StringBuilder value, in FirstWordHandling handling)
        {
            if (value.IsNullOrEmpty() || value[0] == '<')
            {
                return value;
            }

            // only keep it if there is already a leading space (otherwise it may be on the same line without any leading space, and we would fix it in a wrong way)
            value.TrimLeadingSpacesTo(handling.HasSet(FirstWordHandling.KeepLeadingSpace) ? 1 : 0);

            if (handling.HasSet(FirstWordHandling.StartLowerCase))
            {
                value.StartLowerCase();
            }
            else if (handling.HasSet(FirstWordHandling.StartUpperCase))
            {
                value.StartUpperCase();
            }

            if (handling.HasSet(FirstWordHandling.MakeInfinite))
            {
                value.MakeInfinite();
            }

            if (handling.HasSet(FirstWordHandling.MakePlural))
            {
                value.MakePlural();
            }

            if (handling.HasSet(FirstWordHandling.MakeThirdPersonSingular))
            {
                value.MakeThirdPersonSingular();
            }

            return value;
        }

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

        public static string FirstWord(this StringBuilder value, out int whitespacesBefore)
        {
            if (value.IsNullOrEmpty())
            {
                whitespacesBefore = 0;

                return string.Empty;
            }

            var length = value.Length;

            // 1. Find word begin
            whitespacesBefore = value.CountLeadingWhitespaces();

            // 2. Find word end
            var wordLength = 0;

            for (var i = whitespacesBefore; i < length; i++)
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

        public static bool HasWhitespaces(this StringBuilder value, int start = 0)
        {
            if (value.IsNullOrEmpty())
            {
                return false;
            }

            var valueLength = value.Length;

            for (; start < valueLength; start++)
            {
                if (value[start].IsWhiteSpace())
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this StringBuilder value) => value is null || value.Length == 0;

        public static bool IsNullOrWhiteSpace(this StringBuilder value) => value is null || value.CountLeadingWhitespaces() == value.Length;

        public static bool IsSingleWord(this StringBuilder value) => value?.HasWhitespaces() is false;

        public static StringBuilder ReplaceAllWithProbe(this StringBuilder value, in ReadOnlySpan<Pair> replacementPairs)
        {
            var count = replacementPairs.Length;

            for (var index = 0; index < count; index++)
            {
                var pair = replacementPairs[index];
                var oldValue = pair.Key;

                if (QuickSubstringProbe(ref value, ref oldValue))
                {
                    // can be part in the replacement as value seems to fit
                    value.Replace(oldValue, pair.Value);
                }
            }

            return value;
        }

        public static StringBuilder ReplaceAllWithProbe(this StringBuilder value, in ReadOnlySpan<string> texts, string replacement)
        {
            var length = texts.Length;

            for (var index = 0; index < length; index++)
            {
                var oldValue = texts[index];

                if (QuickSubstringProbe(ref value, ref oldValue))
                {
                    // can be part in the replacement as value seems to fit
                    value.Replace(oldValue, replacement);
                }
            }

            return value;
        }

        public static StringBuilder ReplaceWithProbe(this StringBuilder value, string oldValue, string newValue)
        {
            if (oldValue.IsNullOrEmpty())
            {
                // cannot replace an empty value
                return value;
            }

            if (QuickSubstringProbe(ref value, ref oldValue))
            {
                return value.Replace(oldValue, newValue);
            }

            // cannot be part in the replacement as value does not fit
            return value;
        }

        public static StringBuilder SeparateWords(this StringBuilder value, in char separator, in FirstWordHandling firstWordHandling = FirstWordHandling.None)
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
                        if (previousC == 'I')
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
                    var isSpecialCharA = c == 'A';

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

            return value.AdjustFirstWord(firstWordHandling);
        }

        public static StringBuilder Trimmed(this StringBuilder value)
        {
            var length = value.Length;

            if (length == 0)
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

        public static string Trim(this StringBuilder value)
        {
            var length = value.Length;

            if (length == 0)
            {
                return string.Empty;
            }

            var start = value.CountLeadingWhitespaces();
            var end = value.CountTrailingWhitespaces(start);

            return value.ToString(start, length - start - end);
        }

        public static StringBuilder TrimmedStart(this StringBuilder value)
        {
            var length = value.Length;

            if (length == 0)
            {
                return value;
            }

            var start = value.CountLeadingWhitespaces();

            return value.Remove(0, start);
        }

        public static string TrimStart(this StringBuilder value)
        {
            var length = value.Length;

            if (length == 0)
            {
                return string.Empty;
            }

            var start = value.CountLeadingWhitespaces();

            return value.ToString(start, length - start);
        }

        public static StringBuilder TrimStart(this StringBuilder value, char[] characters)
        {
            var charactersLength = characters.Length;

            for (var index = 0; index < charactersLength; index++)
            {
                if (value.Length > 0 && value[0] == characters[index])
                {
                    value.Remove(0, 1);
                }
            }

            return value;
        }

        public static string TrimEnd(this StringBuilder value)
        {
            var length = value.Length;

            if (length == 0)
            {
                return string.Empty;
            }

            var end = value.CountTrailingWhitespaces();

            return value.ToString(0, length - end);
        }

        public static StringBuilder TrimEnd(this StringBuilder value, char[] characters)
        {
            var charactersLength = characters.Length;

            for (var index = 0; index < charactersLength; index++)
            {
                var i = value.Length - 1;

                if (i >= 0 && value[i] == characters[index])
                {
                    value.Remove(i, 1);
                }
            }

            return value;
        }

        public static StringBuilder TrimEndBy(this StringBuilder value, in int count)
        {
            if (count == 0)
            {
                return value;
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            var length = value.Length;

            if (count > length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return value.Remove(length - count, count);
        }

        public static StringBuilder TrimmedEnd(this StringBuilder value)
        {
            var length = value.Length;

            if (length == 0)
            {
                return value;
            }

            var end = value.CountTrailingWhitespaces();

            if (end == 0)
            {
                return value;
            }

            return value.Remove(length - end, end);
        }

        public static StringBuilder WithoutMultipleWhiteSpaces(this StringBuilder value) => value.ReplaceAllWithProbe(Constants.Comments.MultiWhitespaceStrings, Constants.Comments.SingleWhitespaceString);

        public static StringBuilder WithoutNewLines(this StringBuilder value) => value.Without('\r', '\n');

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

        public static StringBuilder Without(this StringBuilder value, string phrase) => value.ReplaceWithProbe(phrase, string.Empty);

        public static StringBuilder Without(this StringBuilder value, in ReadOnlySpan<string> phrases) => value.ReplaceAllWithProbe(phrases, string.Empty);

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

        public static StringBuilder WithoutAbbreviations(this StringBuilder value) => AbbreviationDetector.FindAndReplaceAllAbbreviations(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder WithoutParaTags(this StringBuilder value) => value.Without(Constants.ParaTags);

        private static bool QuickSubstringProbe(ref StringBuilder current, ref string other)
        {
            var otherValueLength = other.Length;
            var difference = current.Length - otherValueLength;

            if (difference < 0)
            {
                // cannot be part in the replacement as other value is too long and cannot fit current value
                return false;
            }

            if (difference > 0)
            {
                if (otherValueLength > QuickSubstringProbeLengthThreshold)
                {
                    var otherFirst = other[0];

                    var lastIndex = otherValueLength - 1;
                    var otherLast = other[lastIndex];

                    var length = difference + 1; // increased by 1 to have the complete difference investigated

                    if (difference >= QuickSubstringProbeRentLengthThreshold)
                    {
                        // use rented arrays here (if we have larger differences, the start and end may be in different chunks)
                        return QuickSubstringProbeAtIndicesWithRent(ref current, otherFirst, otherLast, length, lastIndex);
                    }

                    // could be part in the replacement only if characters match
                    return QuickSubstringProbeAtIndices(ref current, otherFirst, otherLast, length, lastIndex);
                }

                // can be part in the replacement as other value is smaller and could fit current value
                return true;
            }

            // if (difference == 0)
            {
                // both values have same length, so do the quick check on whether the characters fit the expected ones (do not limit length as there are only 3 values below length of 8)
                if (current[0] != other[0])
                {
                    return false;
                }

                var lastIndex = otherValueLength - 1;

                return current[lastIndex] == other[lastIndex];
            }
        }

        private static bool QuickSubstringProbeAtIndices(ref StringBuilder text, in char startChar, in char endChar, in int lengthToCompare, in int endIndex)
        {
            for (var position = 0; position < lengthToCompare; position++)
            {
                if (text[position] == startChar && text[endIndex + position] == endChar)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool QuickSubstringProbeAtIndicesWithRent(ref StringBuilder text, in char startChar, in char endChar, in int lengthToCompare, in int endIndex)
        {
            // Use a rented shared pool for performance reasons
            var pool = ArrayPool<char>.Shared;
            var startChars = pool.Rent(lengthToCompare);
            var endChars = pool.Rent(lengthToCompare);

            try
            {
                // Copy the start and end segments from the text
                text.CopyTo(0, startChars, 0, lengthToCompare);
                text.CopyTo(endIndex, endChars, 0, lengthToCompare);

                // Compare the segments
                for (var position = 0; position < lengthToCompare; position++)
                {
                    if (startChars[position] == startChar && endChars[position] == endChar)
                    {
                        return true;
                    }
                }

                return false;
            }
            finally
            {
                pool.Return(startChars);
                pool.Return(endChars);
            }
        }

        private static int CountLeadingWhitespaces(this StringBuilder value, int start = 0)
        {
            var whitespaces = 0;
            var valueLength = value.Length;

            for (; start < valueLength; start++)
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

        private static void StartLowerCase(this StringBuilder value)
        {
            var valueLength = value.Length;

            for (var i = 0; i < valueLength; i++)
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

        private static void StartUpperCase(this StringBuilder value)
        {
            var valueLength = value.Length;

            for (var i = 0; i < valueLength; i++)
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

        private static void MakeInfinite(this StringBuilder value)
        {
            var word = FirstWord(value, out var whitespacesBefore);

            value.Remove(whitespacesBefore, word.Length);

            var infiniteWord = Verbalizer.MakeInfiniteVerb(word);

            value.Insert(whitespacesBefore, infiniteWord);
        }

        private static void MakePlural(this StringBuilder value)
        {
            var word = FirstWord(value, out var whitespacesBefore);

            value.Remove(whitespacesBefore, word.Length);

            var pluralWord = Pluralizer.MakePluralName(word);

            value.Insert(whitespacesBefore, pluralWord);
        }

        private static void MakeThirdPersonSingular(this StringBuilder value)
        {
            var word = FirstWord(value, out var whitespacesBefore);

            value.Remove(whitespacesBefore, word.Length);

            var pluralWord = Verbalizer.MakeThirdPersonSingularVerb(word);

            value.Insert(whitespacesBefore, pluralWord);
        }

        private static void TrimLeadingSpacesTo(this StringBuilder value, in int count)
        {
            if (value[0] == ' ')
            {
                var whitespaces = value.CountLeadingWhitespaces();

                if (whitespaces > count)
                {
                    value.Remove(count, whitespaces - count);
                }
            }
        }

        // TODO RKN: StringReplace with StringComparison http://stackoverflow.com/a/244933/84852
    }
}