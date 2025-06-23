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
        private const int QuickSubstringProbeLengthThreshold = 8;

        private static readonly ArrayPool<char> SharedPool = ArrayPool<char>.Shared;

        public static StringBuilder AdjustFirstWord(this StringBuilder value, in FirstWordHandling handling)
        {
            if (value.IsNullOrEmpty() || value[0] is '<')
            {
                return value;
            }

            // only keep it if there is already a leading space (otherwise it may be on the same line without any leading space, and we would fix it in a wrong way)
            value.TrimLeadingSpacesTo(handling.HasSet(FirstWordHandling.KeepSingleLeadingSpace) ? 1 : 0);

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
            else if (handling.HasSet(FirstWordHandling.MakePlural))
            {
                value.MakePlural();
            }
            else if (handling.HasSet(FirstWordHandling.MakeThirdPersonSingular))
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this StringBuilder value) => value is null || value.Length is 0;

        public static bool IsNullOrWhiteSpace(this StringBuilder value) => value is null || value.CountLeadingWhitespaces() == value.Length;

        public static bool IsSingleWord(this StringBuilder value) => value?.HasWhitespaces() is false;

        public static StringBuilder ReplaceAllWithProbe(this StringBuilder value, in ReadOnlySpan<Pair> replacementPairs)
        {
            char[] text = null;
            var textSpan = GetTextAsRentedArray(value, ref text, SharedPool);

            for (int index = 0, count = replacementPairs.Length; index < count; index++)
            {
                var pair = replacementPairs[index];
                var oldValue = pair.Key;

                var replaceStartIndex = QuickSubstringProbe(textSpan, oldValue.AsSpan());

                if (replaceStartIndex is -1)
                {
                    continue;
                }

                // can be part in the replacement as value seems to fit
                value.Replace(oldValue, pair.Value, replaceStartIndex, value.Length - replaceStartIndex);

                // re-assign text as we might have replaced the string, but use a different array
                textSpan = GetTextAsRentedArray(value, ref text, SharedPool);
            }

            if (text != null)
            {
                SharedPool.Return(text);
            }

            return value;
        }

        public static StringBuilder ReplaceAllWithProbe(this StringBuilder value, in ReadOnlySpan<string> texts, string replacement)
        {
            char[] text = null;
            var textSpan = GetTextAsRentedArray(value, ref text, SharedPool);

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
                textSpan = GetTextAsRentedArray(value, ref text, SharedPool);
            }

            if (text != null)
            {
                SharedPool.Return(text);
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

            char[] text = null;
            var textSpan = GetTextAsRentedArray(value, ref text, SharedPool);

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

            return value.AdjustFirstWord(firstWordHandling);
        }

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

        public static StringBuilder TrimEndBy(this StringBuilder value, in int count)
        {
            if (count is 0)
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

        private static ReadOnlySpan<char> GetTextAsRentedArray(StringBuilder value, ref char[] text, ArrayPool<char> pool)
        {
            if (text != null)
            {
                pool.Return(text);
            }

            var size = value.Length;

            text = pool.Rent(size);

            value.CopyTo(0, text, 0, size);

            return text.AsSpan(0, size); // use the length of the string builder as the rented array itself may be longer than the requested size, thus leading to unexpected behavior
        }

        private static int QuickSubstringProbe(in ReadOnlySpan<char> current, in ReadOnlySpan<char> other)
        {
            if (current.Length < other.Length)
            {
                // cannot be part in the replacement as other value is too long and cannot fit current value
                return -1;
            }

            if (other.Length < QuickSubstringProbeLengthThreshold)
            {
                // can be part in the replacement as other value is smaller and could fit current value
                return 0;
            }

            // Performance-Note:
            // - do not use a separate if condition for the delta being zero as that may not happen often and the conditional check therefore is too costly
            var lastIndex = other.Length - 1;
            var startChar = other[0];
            var endChar = other[lastIndex];

            for (int position = 0, delta = current.Length - other.Length; position <= delta; position++)
            {
                // Performance-Note:
                // - do not split or re-calculate last index position each time as this gets invoked millions of time and re-calculation is too costly in such situation
                if (current[lastIndex + position] == endChar && current[position] == startChar)
                {
                    // could be part in the replacement as characters match both at start and end
                    return position;
                }
            }

            return -1;
        }

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
            if (value[0] is ' ')
            {
                var whitespaces = value.CountLeadingWhitespaces();

                if (whitespaces > count)
                {
                    value.Remove(count, whitespaces - count);
                }
            }
        }
    }
}