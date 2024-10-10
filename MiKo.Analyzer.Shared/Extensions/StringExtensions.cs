using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers;
using MiKoSolutions.Analyzers.Linguistics;

//// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace System
{
    internal static class StringExtensions
    {
        private const int QuickCompareLengthThreshold = 4;

        private const int DifferenceBetweenUpperAndLowerCaseAscii = 0x20; // valid for Roman ASCII characters ('A' ... 'Z')

        private static readonly char[] GenericTypeArgumentSeparator = { ',' };

        private static readonly Regex HyperlinkRegex = new Regex(@"(www|ftp:|ftps:|http:|https:)+[^\s]+[\w]", RegexOptions.Compiled, 100.Milliseconds());

        private static readonly Regex PascalCasingRegex = new Regex("[a-z]+[A-Z]+", RegexOptions.Compiled, 100.Milliseconds());

//// ncrunch: no coverage start

        public static bool HasFlag(FirstWordHandling value, FirstWordHandling flag) => (value & flag) == flag;

        public static string AdjustFirstWord(this string value, FirstWordHandling handling)
        {
            if (value.StartsWith('<'))
            {
                return value;
            }

            var valueSpan = value.AsSpan();

            string word;

            if (HasFlag(handling, FirstWordHandling.MakeLowerCase))
            {
                word = valueSpan.FirstWord().ToLowerCaseAt(0);
            }
            else if (HasFlag(handling, FirstWordHandling.MakeUpperCase))
            {
                word = valueSpan.FirstWord().ToUpperCaseAt(0);
            }
            else
            {
                word = valueSpan.FirstWord().ToString();
            }

            // build continuation here because the word length may change based on the infinite term
            var continuation = valueSpan.TrimStart().Slice(word.Length);

            if (HasFlag(handling, FirstWordHandling.MakeInfinite))
            {
                word = Verbalizer.MakeInfiniteVerb(word);
            }

            if (HasFlag(handling, FirstWordHandling.MakePlural))
            {
                word = Pluralizer.MakePluralName(word);
            }

            if (HasFlag(handling, FirstWordHandling.KeepLeadingSpace))
            {
                // only keep it if there is already a leading space (otherwise it may be on the same line without any leading space, and we would fix it in a wrong way)
                if (value.StartsWith(' '))
                {
                    return ' '.ConcatenatedWith(word, continuation);
                }
            }

            return word.ConcatenatedWith(continuation);
        }

        public static IReadOnlyList<int> AllIndicesOf(this string value, string finding, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return Array.Empty<int>();
            }

            var valueLength = value.Length;
            var findingLength = finding.Length;

            if (findingLength > valueLength)
            {
                return Array.Empty<int>();
            }

            List<int> indices = null;

            for (var index = 0; ; index += findingLength)
            {
                index = value.IndexOf(finding, index, comparison);

                if (index == -1)
                {
                    // nothing more to find
                    break;
                }

                if (indices is null)
                {
                    indices = new List<int>(1);
                }

                indices.Add(index);
            }

            if (indices is null)
            {
                return Array.Empty<int>();
            }

            return indices;
        }

//// ncrunch: no coverage end

        public static IReadOnlyList<int> AllIndicesOf(this ReadOnlySpan<char> value, string finding, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return Array.Empty<int>();
            }

            if (finding.Length > value.Length)
            {
                return Array.Empty<int>();
            }

            if (comparison == StringComparison.Ordinal)
            {
                // performance optimization for 'StringComparison.Ordinal' to avoid multiple strings from being created (see 'IndexOf' method inside 'MemoryExtensions')
                return AllIndicesOrdinal(value, finding.AsSpan());
            }

            return AllIndicesOf(value.ToString(), finding, comparison);

            IReadOnlyList<int> AllIndicesOrdinal(ReadOnlySpan<char> span, ReadOnlySpan<char> other)
            {
                var otherLength = other.Length;

                List<int> indices = null;

                for (var index = 0; ; index += otherLength)
                {
                    var newIndex = span.Slice(index).IndexOf(other, StringComparison.Ordinal);

                    if (newIndex == -1)
                    {
                        // nothing more to find
                        break;
                    }

                    index += newIndex;

                    if (indices is null)
                    {
                        indices = new List<int>(1);
                    }

                    indices.Add(index);
                }

                if (indices is null)
                {
                    return Array.Empty<int>();
                }

                return indices;
            }
        }

//// ncrunch: no coverage start

        public static SyntaxToken AsToken(this string source, SyntaxKind kind = SyntaxKind.StringLiteralToken)
        {
            if (kind == SyntaxKind.IdentifierToken)
            {
                return SyntaxFactory.Identifier(source);
            }

            return SyntaxFactory.Token(default, kind, source, source, default);
        }

        public static InterpolatedStringTextSyntax AsInterpolatedString(this ReadOnlySpan<char> value) => value.ToString().AsInterpolatedString();

        public static InterpolatedStringTextSyntax AsInterpolatedString(this string value) => SyntaxFactory.InterpolatedStringText(value.AsToken(SyntaxKind.InterpolatedStringTextToken));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith(this IEnumerable<string> values) => string.Concat(values.Where(_ => _ != null));

        public static StringBuilder ConcatenatedWith<T>(this IEnumerable<T> values) where T : class
        {
            var builder = new StringBuilder();

            foreach (var value in values)
            {
                if (value != null)
                {
                    builder.Append(value);
                }
            }

            return builder;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith(this IEnumerable<string> values, string separator) => string.Join(separator, values);

        public static string ConcatenatedWith(this char value, string arg0)
        {
            var arg0Length = arg0?.Length ?? 0;
            var length = arg0Length + 1;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                buffer[0] = value;

                if (arg0Length > 0)
                {
                    arg0.AsSpan().CopyTo(bufferSpan.Slice(1));
                }

                return new string(buffer, 0, length);
            }
        }

        public static string ConcatenatedWith(this char value, ReadOnlySpan<char> arg0)
        {
            var arg0Length = arg0.Length;
            var length = arg0Length + 1;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                buffer[0] = value;

                if (arg0Length > 0)
                {
                    arg0.CopyTo(bufferSpan.Slice(1));
                }

                return new string(buffer, 0, length);
            }
        }

        public static string ConcatenatedWith(this string value, ReadOnlySpan<char> span)
        {
            var spanLength = span.Length;

            if (spanLength == 0)
            {
                return value;
            }

            var valueLength = value?.Length ?? 0;

            if (valueLength == 0)
            {
                return span.ToString();
            }

            var length = valueLength + spanLength;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                span.CopyTo(bufferSpan.Slice(valueLength, spanLength));
                value.AsSpan().CopyTo(bufferSpan);

                return new string(buffer, 0, length);
            }
        }

        public static string ConcatenatedWith(this string value, char arg0)
        {
            var valueLength = value?.Length ?? 0;
            var length = valueLength + 1;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                if (valueLength > 0)
                {
                    value.AsSpan().CopyTo(bufferSpan);
                }

                buffer[valueLength] = arg0;

                return new string(buffer, 0, length);
            }
        }

        public static string ConcatenatedWith(this ReadOnlySpan<char> value, char arg0)
        {
            var valueLength = value.Length;
            var length = valueLength + 1;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                if (valueLength > 0)
                {
                    value.CopyTo(bufferSpan);
                }

                buffer[valueLength] = arg0;

                return new string(buffer, 0, length);
            }
        }

        public static string ConcatenatedWith(this ReadOnlySpan<char> value, string arg0)
        {
            var spanLength = value.Length;

            if (spanLength == 0)
            {
                return arg0;
            }

            var arg0Length = arg0?.Length ?? 0;

            if (arg0Length == 0)
            {
                return value.ToString();
            }

            var length = spanLength + arg0Length;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                value.CopyTo(bufferSpan);
                arg0.AsSpan().CopyTo(bufferSpan.Slice(spanLength, arg0Length));

                return new string(buffer, 0, length);
            }
        }

        public static string ConcatenatedWith(this char value, string arg0, char arg1)
        {
            var arg0Length = arg0?.Length ?? 0;

            var length = arg0Length + 2;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                buffer[0] = value;

                if (arg0Length > 0)
                {
                    arg0.AsSpan().CopyTo(bufferSpan.Slice(1));
                }

                buffer[arg0Length + 1] = arg1;

                return new string(buffer, 0, length);
            }
        }

        public static string ConcatenatedWith(this char value, ReadOnlySpan<char> arg0, char arg1)
        {
            var arg0Length = arg0.Length;

            var length = arg0Length + 2;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                buffer[0] = value;

                if (arg0Length > 0)
                {
                    arg0.CopyTo(bufferSpan.Slice(1));
                }

                buffer[arg0Length + 1] = arg1;

                return new string(buffer, 0, length);
            }
        }

        public static string ConcatenatedWith(this char value, string arg0, ReadOnlySpan<char> arg1)
        {
            var arg0Length = arg0?.Length ?? 0;
            var arg1Length = arg1.Length;

            var length = 1 + arg0Length + arg1Length;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                buffer[0] = value;

                arg0.AsSpan().CopyTo(bufferSpan.Slice(1));
                arg1.CopyTo(bufferSpan.Slice(1 + arg0Length, arg1Length));

                return new string(buffer, 0, length);
            }
        }

        public static string ConcatenatedWith(this ReadOnlySpan<char> value, string arg0, string arg1)
        {
            var valueLength = value.Length;

            if (valueLength == 0)
            {
                return string.Concat(arg0, arg1);
            }

            var arg0Length = arg0?.Length ?? 0;
            var arg1Length = arg1?.Length ?? 0;

            var length = valueLength + arg0Length + arg1Length;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                value.CopyTo(bufferSpan);
                arg0.AsSpan().CopyTo(bufferSpan.Slice(valueLength));
                arg1.AsSpan().CopyTo(bufferSpan.Slice(valueLength + arg0Length));

                return new string(buffer, 0, length);
            }
        }

        public static string ConcatenatedWith(this ReadOnlySpan<char> value, string arg0, ReadOnlySpan<char> arg1)
        {
            var valueLength = value.Length;

            if (valueLength == 0)
            {
                return arg0.ConcatenatedWith(arg1);
            }

            var arg0Length = arg0?.Length ?? 0;
            var arg1Length = arg1.Length;

            var length = valueLength + arg0Length + arg1Length;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                value.CopyTo(bufferSpan);
                arg0.AsSpan().CopyTo(bufferSpan.Slice(valueLength));
                arg1.CopyTo(bufferSpan.Slice(valueLength + arg0Length, arg1Length));

                return new string(buffer, 0, length);
            }
        }

        public static string ConcatenatedWith(this string value, string arg0, ReadOnlySpan<char> arg1)
        {
            if (value is null)
            {
                return arg0.ConcatenatedWith(arg1);
            }

            var valueLength = value.Length;

            if (value.Length == 0)
            {
                return arg0.ConcatenatedWith(arg1);
            }

            var arg0Length = arg0?.Length ?? 0;
            var arg1Length = arg1.Length;

            var length = valueLength + arg0Length + arg1Length;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                value.AsSpan().CopyTo(bufferSpan);
                arg0.AsSpan().CopyTo(bufferSpan.Slice(valueLength));
                arg1.CopyTo(bufferSpan.Slice(valueLength + arg0Length));

                return new string(buffer, 0, length);
            }
        }

        public static string ConcatenatedWith(this string value, ReadOnlySpan<char> arg0, string arg1)
        {
            if (value is null)
            {
                return arg0.ConcatenatedWith(arg1);
            }

            var valueLength = value.Length;

            if (value.Length == 0)
            {
                return arg0.ConcatenatedWith(arg1);
            }

            var arg0Length = arg0.Length;
            var arg1Length = arg1?.Length ?? 0;

            var length = valueLength + arg0Length + arg1Length;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                value.AsSpan().CopyTo(bufferSpan);
                arg0.CopyTo(bufferSpan.Slice(valueLength));
                arg1.AsSpan().CopyTo(bufferSpan.Slice(valueLength + arg0Length));

                return new string(buffer, 0, length);
            }
        }

        public static string ConcatenatedWith(this ReadOnlySpan<char> value, char arg0, string arg1, char arg2)
        {
            var valueLength = value.Length;
            var arg1Length = arg1?.Length ?? 0;

            var length = valueLength + arg1Length + 2;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                if (valueLength > 0)
                {
                    value.CopyTo(bufferSpan);
                }

                buffer[valueLength + 1] = arg0;
                buffer[valueLength + 1 + arg1Length] = arg2;

                arg1.AsSpan().CopyTo(bufferSpan.Slice(valueLength + 2));

                return new string(buffer, 0, length);
            }
        }

        public static bool Contains(this string value, char c) => value?.IndexOf(c) >= 0;

        public static bool Contains(this ReadOnlySpan<char> value, char c) => value.Length > 0 && value.IndexOf(c) >= 0;

        public static bool Contains(this ReadOnlySpan<char> value, ReadOnlySpan<char> finding)
        {
            if (finding.Length > value.Length)
            {
                return false;
            }

            return value.IndexOf(finding) >= 0;
        }

        public static bool Contains(this ReadOnlySpan<char> value, string finding)
        {
            if (finding is null)
            {
                return false;
            }

            return value.Contains(finding.AsSpan());
        }

        public static bool Contains(this string value, string finding, StringComparison comparison)
        {
            var valueLength = value.Length;
            var findingLength = finding.Length;

            var difference = findingLength - valueLength;

            if (difference < 0)
            {
                return value.IndexOf(finding, comparison) >= 0;
            }

            if (difference == 0)
            {
                return QuickEquals();

                bool QuickEquals()
                {
                    const int QuickInspectionChars = 2;

                    if (valueLength > QuickInspectionChars)
                    {
                        var valueSpan = value.AsSpan(valueLength - QuickInspectionChars, QuickInspectionChars);
                        var findingSpan = finding.AsSpan(findingLength - QuickInspectionChars, QuickInspectionChars);

                        if (valueSpan.CompareTo(findingSpan, comparison) != 0)
                        {
                            return false;
                        }
                    }

                    return value.Equals(finding, comparison);
                }
            }

            switch (comparison)
            {
                case StringComparison.Ordinal:
                case StringComparison.OrdinalIgnoreCase:
                    // cannot be contained as the item is longer than the string to search in
                    return false;
            }

            return value.IndexOf(finding, comparison) >= 0;
        }

//// ncrunch: no coverage end

        public static bool Contains(this ReadOnlySpan<char> value, string finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison) => value.Contains(finding.AsSpan(), nextCharValidationCallback, comparison);

        public static bool Contains(this ReadOnlySpan<char> value, ReadOnlySpan<char> finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison)
        {
            var index = 0;
            var valueLength = value.Length;
            var findingLength = finding.Length;

            if (findingLength > valueLength)
            {
                switch (comparison)
                {
                    case StringComparison.Ordinal:
                    case StringComparison.OrdinalIgnoreCase:
                        // cannot be contained as the item is longer than the string to search in
                        return false;
                }
            }

            while (true)
            {
                var newIndex = value.Slice(index).IndexOf(finding, comparison);

                if (newIndex <= -1)
                {
                    return false;
                }

                index += newIndex;

                var positionAfterCharacter = index + findingLength;

                if (positionAfterCharacter >= valueLength)
                {
                    return true;
                }

                var nextChar = value[positionAfterCharacter];

                if (nextCharValidationCallback(nextChar))
                {
                    return true;
                }

                index = positionAfterCharacter;
            }
        }

        public static bool ContainsAny(this string value, char[] characters) => value?.IndexOfAny(characters) >= 0;

        public static bool ContainsAny(this ReadOnlySpan<char> value, ReadOnlySpan<char> characters) => value.Length > 0 && value.IndexOfAny(characters) >= 0;

        public static bool ContainsAny(this string value, string[] phrases) => value.ContainsAny(phrases, StringComparison.OrdinalIgnoreCase); // ncrunch: no coverage

        //// ncrunch: no coverage start

        public static bool ContainsAny(this string value, string[] phrases, StringComparison comparison)
        {
            if (value.HasCharacters())
            {
                var valueSpan = value.AsSpan();
                var phrasesLength = phrases.Length;

                for (var index = 0; index < phrasesLength; index++)
                {
                    var phrase = phrases[index];
                    var phraseSpan = phrase.AsSpan();

                    // no separate handling for StringComparison.Ordinal here as that happens around 30 times out of 90_000_000 times; so almost never
                    if (QuickCompare(valueSpan, phraseSpan, comparison))
                    {
                        if (value.Contains(phrase, comparison))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

//// ncrunch: no coverage end

        public static bool EndsWith(this string value, char character) => value.HasCharacters() && value[value.Length - 1] == character;

        public static bool EndsWith(this ReadOnlySpan<char> value, char character) => value.Length > 0 && value[value.Length - 1] == character;

        public static bool EndsWith(this ReadOnlySpan<char> value, string characters, StringComparison comparison) => characters.HasCharacters() && value.EndsWith(characters.AsSpan(), comparison); // ncrunch: no coverage

        public static bool EndsWithAny(this string value, ReadOnlySpan<char> suffixCharacters) => value.HasCharacters() && suffixCharacters.Contains(value[value.Length - 1]);

        public static bool EndsWithAny(this string value, string[] suffixes) => value.EndsWithAny(suffixes, StringComparison.OrdinalIgnoreCase);

        public static bool EndsWithAny(this ReadOnlySpan<char> value, ReadOnlySpan<char> suffixCharacters)
        {
            if (value.Length > 0)
            {
                var lastChar = value[value.Length - 1];

                var length = suffixCharacters.Length;

                for (var index = 0; index < length; index++)
                {
                    if (lastChar == suffixCharacters[index])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool EndsWithAny(this ReadOnlySpan<char> value, string[] suffixes) => value.EndsWithAny(suffixes, StringComparison.OrdinalIgnoreCase);

//// ncrunch: no coverage start

        public static bool EndsWithAny(this string value, string[] suffixes, StringComparison comparison)
        {
            if (value.HasCharacters())
            {
                var valueLength = value.Length;
                var suffixesLength = suffixes.Length;

                for (var index = 0; index < suffixesLength; index++)
                {
                    var suffix = suffixes[index];

                    if (suffix.Length > valueLength)
                    {
                        continue;
                    }

                    if (value.EndsWith(suffix, comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

//// ncrunch: no coverage end

        public static bool EndsWithAny(this string value, List<string> suffixes, StringComparison comparison)
        {
            if (value.HasCharacters())
            {
                var valueLength = value.Length;
                var suffixesLength = suffixes.Count;

                for (var index = 0; index < suffixesLength; index++)
                {
                    var suffix = suffixes[index];

                    if (suffix.Length > valueLength)
                    {
                        continue;
                    }

                    if (value.EndsWith(suffix, comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool EndsWithAny(this ReadOnlySpan<char> value, string[] suffixes, StringComparison comparison)
        {
            var valueLength = value.Length;
            var suffixesLength = suffixes.Length;

            if (valueLength > 0 && suffixesLength > 0)
            {
                for (var index = 0; index < suffixesLength; index++)
                {
                    var suffix = suffixes[index];

                    if (suffix.Length > valueLength)
                    {
                        continue;
                    }

                    if (value.EndsWith(suffix, comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool EndsWithCommonNumber(this string value) => value.EndsWithNumber() && value.EndsWithAny(Constants.Markers.OSBitNumbers) is false;

        public static bool EndsWithCommonNumber(this ReadOnlySpan<char> value) => value.EndsWithNumber() && value.EndsWithAny(Constants.Markers.OSBitNumbers) is false;

        public static bool EndsWithNumber(this string value) => value.HasCharacters() && value[value.Length - 1].IsNumber();

        public static bool EndsWithNumber(this ReadOnlySpan<char> value) => value.Length > 0 && value[value.Length - 1].IsNumber();

        public static bool Equals(this string value, ReadOnlySpan<char> other, StringComparison comparison) => value != null && value.AsSpan().Equals(other, comparison);

        public static bool Equals(this ReadOnlySpan<char> value, string other, StringComparison comparison) => other != null && value.Equals(other.AsSpan(), comparison);

        public static bool EqualsAny(this string value, IEnumerable<string> phrases) => EqualsAny(value, phrases, StringComparison.OrdinalIgnoreCase);

        public static bool EqualsAny(this ReadOnlySpan<char> value, string[] phrases) => EqualsAny(value, phrases, StringComparison.OrdinalIgnoreCase);

//// ncrunch: no coverage start

        public static bool EqualsAny(this string value, string[] phrases, StringComparison comparison)
        {
            if (value.HasCharacters())
            {
                var phrasesLength = phrases.Length;

                for (var index = 0; index < phrasesLength; index++)
                {
                    var phrase = phrases[index];

                    if (value.Equals(phrase, comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

//// ncrunch: no coverage end

        public static bool EqualsAny(this string value, IEnumerable<string> phrases, StringComparison comparison)
        {
            if (phrases is string[] array)
            {
                return value.EqualsAny(array, comparison);
            }

            if (value.HasCharacters())
            {
                foreach (var phrase in phrases)
                {
                    if (value.Equals(phrase, comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool EqualsAny(this ReadOnlySpan<char> value, string[] phrases, StringComparison comparison)
        {
            if (value.Length > 0)
            {
                var phrasesLength = phrases.Length;

                for (var index = 0; index < phrasesLength; index++)
                {
                    var phrase = phrases[index];

                    if (value.Equals(phrase, comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static string FirstWord(this string value)
        {
            if (value is null)
            {
                return null;
            }

            var span = value.AsSpan();
            var word = FirstWord(span);

            return word != span
                   ? word.ToString()
                   : value;
        }

//// ncrunch: no coverage start

        public static ReadOnlySpan<char> FirstWord(this ReadOnlySpan<char> value)
        {
            var text = value.TrimStart();
            var textLength = text.Length;

            if (textLength > 0)
            {
                for (var index = 0; index < textLength; index++)
                {
                    var c = text[index];

                    if (c.IsWhiteSpace() || c.IsSentenceEnding())
                    {
                        return text.Slice(0, index);
                    }
                }

                // start at index 1 to skip first upper case character (and avoid return of empty word)
                for (var index = 1; index < textLength; index++)
                {
                    var c = text[index];

                    if (c.IsUpperCase())
                    {
                        return text.Slice(0, index);
                    }
                }
            }

            return text;
        }

//// ncrunch: no coverage end

        public static string SecondWord(this string value) => SecondWord(value.AsSpan()).ToString();

        public static ReadOnlySpan<char> SecondWord(this ReadOnlySpan<char> value) => value.WithoutFirstWord().FirstWord();

        public static string ThirdWord(this string value) => ThirdWord(value.AsSpan()).ToString();

        public static ReadOnlySpan<char> ThirdWord(this ReadOnlySpan<char> value) => value.WithoutFirstWord().WithoutFirstWord().FirstWord();

        public static string LastWord(this string value)
        {
            if (value is null)
            {
                return null;
            }

            var span = value.AsSpan();
            var word = LastWord(span);

            return word != span
                   ? word.ToString()
                   : value;
        }

        public static ReadOnlySpan<char> LastWord(this ReadOnlySpan<char> value)
        {
            var text = value.TrimEnd();

            var lastSpace = text.LastIndexOfAny(Constants.WhiteSpaceCharacters) + 1;

            if (lastSpace <= 0)
            {
                return text;
            }

            return text.Slice(lastSpace);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, char arg0) => string.Format(format, arg0.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, object arg0) => string.Format(format, arg0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, object arg0, object arg1) => string.Format(format, arg0, arg1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, string arg0, char arg1) => string.Format(format, arg0, arg1.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, object arg0, object arg1, object arg2) => string.Format(format, arg0, arg1, arg2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, object arg0, object arg1, object arg2, object arg3) => string.Format(format, arg0, arg1, arg2, arg3);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, char arg0, char arg1, char arg2, char arg3) => string.Format(format, arg0.ToString(), arg1.ToString(), arg2.ToString(), arg3.ToString());

        public static string GetNameOnlyPart(this string value) => GetNameOnlyPart(value.AsSpan());

        public static string GetNameOnlyPart(this ReadOnlySpan<char> value)
        {
            var genericIndexStart = value.IndexOf('<');
            var genericIndexEnd = value.LastIndexOf('>');

            if (genericIndexStart > 0 && genericIndexEnd > 0)
            {
                var indexAfterGenericStart = genericIndexStart + 1;

                var namePart = value.Slice(0, genericIndexStart).GetPartAfterLastDot().ToString();
                var genericParts = value.Slice(indexAfterGenericStart, genericIndexEnd - indexAfterGenericStart)
                                        .SplitBy(GenericTypeArgumentSeparator, StringSplitOptions.RemoveEmptyEntries);
                var count = genericParts.Count();

                if (count > 0)
                {
                    var i = 0;

                    var genericNameParts = new string[count];

                    foreach (ReadOnlySpan<char> part in genericParts)
                    {
                        genericNameParts[i++] = part.GetPartAfterLastDot().ToString();
                    }

                    var genericPart = string.Join(",", genericNameParts);

                    return string.Concat(namePart, "<", genericPart, ">");
                }
            }

            return value.GetPartAfterLastDot().ToString();
        }

        public static string GetNameOnlyPartWithoutGeneric(this ReadOnlySpan<char> value)
        {
            var genericIndexStart = value.IndexOf('<');

            var name = genericIndexStart > 0
                       ? value.Slice(0, genericIndexStart)
                       : value;

            return name.GetPartAfterLastDot().ToString();
        }

        public static string GetPartAfterLastDot(this string value)
        {
            if (value.HasCharacters())
            {
                return GetPartAfterLastDot(value.AsSpan()).ToString();
            }

            return null;
        }

        public static ReadOnlySpan<char> GetPartAfterLastDot(this ReadOnlySpan<char> value) => value.Slice(value.LastIndexOf('.') + 1);

        public static bool HasCollectionMarker(this string value) => value.EndsWithAny(Constants.Markers.Collections);

        public static bool HasEntityMarker(this string value) => HasEntityMarker(value.AsSpan());

        public static bool HasEntityMarker(this ReadOnlySpan<char> value)
        {
            var s = value.ToString();

            var hasMarker = s.ContainsAny(Constants.Markers.Models);

            if (hasMarker)
            {
                if (s.ContainsAny(Constants.Markers.ViewModels))
                {
                    return false;
                }

                if (s.ContainsAny(Constants.Markers.SpecialModels))
                {
                    return false;
                }
            }

            return hasMarker;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasCharacters(this string value) => value.IsNullOrEmpty() is false; // ncrunch: no coverage

        public static bool HasUpperCaseLettersAbove(this string value, ushort limit) => value != null && HasUpperCaseLettersAbove(value.AsSpan(), limit);

        public static bool HasUpperCaseLettersAbove(this ReadOnlySpan<char> value, ushort limit)
        {
            var count = 0;
            var length = value.Length;

            for (var index = 0; index < length; index++)
            {
                if (value[index].IsUpperCase())
                {
                    if (count == limit)
                    {
                        return true;
                    }

                    count++;
                }
            }

            return count > limit;
        }

        public static string HumanizedConcatenated(this IEnumerable<string> values, string lastSeparator = "or")
        {
            var items = values.ToArray(_ => _.SurroundedWithApostrophe());

            var count = items.Length;

            switch (count)
            {
                case 0: return string.Empty;
                case 1: return items[0];
            }

            const string Separator = ", ";

            var separatorForLast = ' '.ConcatenatedWith(lastSeparator, ' ');

            switch (count)
            {
                case 2: return string.Concat(items[0], separatorForLast, items[1]);
                case 3: return new StringBuilder(items[0]).Append(Separator).Append(items[1]).Append(separatorForLast).Append(items[2]).ToString();
                case 4: return new StringBuilder(items[0]).Append(Separator).Append(items[1]).Append(Separator).Append(items[2]).Append(separatorForLast).Append(items[3]).ToString();
                default: return string.Concat(items.Take(count - 1).ConcatenatedWith(Separator), separatorForLast, items[count - 1]);
            }
        }

        public static string HumanizedTakeFirst(this ReadOnlySpan<char> value, int max)
        {
            var minimumLength = Math.Min(max, value.Length);

            if (minimumLength <= 0 || minimumLength == value.Length)
            {
                return value.TrimEnd().ToString();
            }

            var span = value.Slice(0, minimumLength).TrimEnd();
            minimumLength = span.Length;

            var length = minimumLength + 3;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                span.CopyTo(bufferSpan);

                buffer[minimumLength] = '.';
                buffer[minimumLength + 1] = '.';
                buffer[minimumLength + 2] = '.';

                return new string(buffer, 0, length);
            }
        }

        public static int IndexOfAny(this ReadOnlySpan<char> value, string[] phrases, StringComparison comparison)
        {
            if (value.Length > 0)
            {
                // performance optimization to avoid unnecessary 'ToString' calls on 'ReadOnlySpan' (see implementation inside MemoryExtensions)
                if (comparison == StringComparison.Ordinal)
                {
                    var phrasesLength = phrases.Length;

                    for (var i = 0; i < phrasesLength; i++)
                    {
                        var phrase = phrases[i];

                        var index = value.IndexOf(phrase.AsSpan(), StringComparison.Ordinal);

                        if (index > -1)
                        {
                            return index;
                        }
                    }
                }
                else
                {
                    // use string here to avoid unnecessary 'ToString' calls on 'ReadOnlySpan' (see implementation inside MemoryExtensions)
                    return value.ToString().IndexOfAny(phrases, comparison);
                }
            }

            return -1;
        }

        public static int IndexOfAny(this string value, string[] phrases, StringComparison comparison)
        {
            if (value.HasCharacters())
            {
                if (comparison == StringComparison.Ordinal)
                {
                    return IndexOfAny(value.AsSpan(), phrases, comparison);
                }

                var phrasesLength = phrases.Length;

                for (var i = 0; i < phrasesLength; i++)
                {
                    var phrase = phrases[i];

                    var index = value.IndexOf(phrase, comparison);

                    if (index > -1)
                    {
                        return index;
                    }
                }
            }

            return -1;
        }

        public static int LastIndexOfAny(this string value, string[] phrases, StringComparison comparison)
        {
            if (value is null)
            {
                return -1;
            }

            if (value.Length > 0)
            {
                var phrasesLength = phrases.Length;

                for (var i = 0; i < phrasesLength; i++)
                {
                    var phrase = phrases[i];

                    var index = value.LastIndexOf(phrase, comparison);

                    if (index > -1)
                    {
                        return index;
                    }
                }
            }

            return -1;
        }

        public static bool IsAcronym(this string value) => value.HasCharacters() && value.None(_ => _.IsLowerCaseLetter());

        public static bool IsHyperlink(this string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return false;
            }

            try
            {
                return HyperlinkRegex.IsMatch(value);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLetter(this char value) => char.IsLetter(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowerCase(this char value) => char.IsLower(value); // ncrunch: no coverage

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowerCaseLetter(this char value) => value.IsLetter() && value.IsLowerCase();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value); // ncrunch: no coverage

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this ReadOnlySpan<char> value) => value.IsEmpty; // ncrunch: no coverage

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value); // ncrunch: no coverage

//// ncrunch: no coverage start
        public static bool IsNullOrWhiteSpace(this ReadOnlySpan<char> value)
        {
            var length = value.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    if (value[index].IsWhiteSpace() is false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
//// ncrunch: no coverage end

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumber(this char value) => char.IsNumber(value);

        public static bool IsPascalCasing(this string value)
        {
            try
            {
                return PascalCasingRegex.IsMatch(value);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSentenceEnding(this char value)
        {
            switch (value)
            {
                case '.':
                case '?':
                case '!':
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsSingleWord(this string value) => value != null && IsSingleWord(value.AsSpan());

        public static bool IsSingleWord(this ReadOnlySpan<char> value) => value.HasWhitespaces() is false;

        //// ncrunch: no coverage start

        public static bool IsUpperCase(this char value)
        {
            if ((uint)(value - 'A') <= 'Z' - 'A')
            {
                return true;
            }

            if ((uint)(value - 'a') <= 'z' - 'a')
            {
                return false;
            }

            if (value == ' ')
            {
                return false;
            }

            return char.IsUpper(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUpperCaseLetter(this char value) => value.IsLetter() && value.IsUpperCase();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteSpace(this char value)
        {
            switch (value)
            {
                case ' ':
                case '\t':
                    return true;

                default:
                    return char.IsWhiteSpace(value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this string value, char character) => value.HasCharacters() && value[0] == character;

        public static bool StartsWith(this string value, ReadOnlySpan<char> characters) => value.HasCharacters() && value.AsSpan().StartsWith(characters);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this ReadOnlySpan<char> value, char character) => value.Length > 0 && value[0] == character;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this ReadOnlySpan<char> value, string characters) => characters.HasCharacters() && value.StartsWith(characters.AsSpan());

        public static bool StartsWith(this string value, ReadOnlySpan<char> characters, StringComparison comparison) => value.HasCharacters() && value.AsSpan().StartsWith(characters, comparison);

        public static bool StartsWith(this ReadOnlySpan<char> value, string characters, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(characters))
            {
                return false;
            }

            var others = characters.AsSpan();

            // perform quick check
            if (QuickCompare(value, others, comparison))
            {
                return value.StartsWith(others, comparison);
            }

            return false;
        }

//// ncrunch: no coverage end

        public static bool StartsWithAny(this string value, IEnumerable<char> characters) => value.HasCharacters() && characters.Contains(value[0]);

        public static bool StartsWithAny(this ReadOnlySpan<char> value, IEnumerable<char> characters) => value.Length > 0 && characters.Contains(value[0]);

        public static bool StartsWithAny(this string value, string[] prefixes) => value.StartsWithAny(prefixes, StringComparison.OrdinalIgnoreCase);

        public static bool StartsWithAny(this ReadOnlySpan<char> value, string[] prefixes) => value.StartsWithAny(prefixes, StringComparison.OrdinalIgnoreCase);

//// ncrunch: no coverage start

        public static bool StartsWithAny(this string value, string[] prefixes, StringComparison comparison)
        {
            if (value.HasCharacters())
            {
                var valueSpan = value.AsSpan();
                var prefixesCount = prefixes.Length;

                for (var index = 0; index < prefixesCount; index++)
                {
                    var prefix = prefixes[index];

                    if (QuickCompare(valueSpan, prefix.AsSpan(), comparison))
                    {
                        if (value.StartsWith(prefix, comparison))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool StartsWithAny(this ReadOnlySpan<char> value, string[] prefixes, StringComparison comparison)
        {
            if (value.Length > 0)
            {
                var prefixesLength = prefixes.Length;

                for (var index = 0; index < prefixesLength; index++)
                {
                    var prefix = prefixes[index];

                    // TODO RKN: Add Quick compare ?
                    if (value.StartsWith(prefix, comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

//// ncrunch: no coverage end

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWithNumber(this string value) => value.HasCharacters() && value[0].IsNumber();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWith(this string value, char surrounding) => surrounding.ConcatenatedWith(value, surrounding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWithApostrophe(this string value) => value?.SurroundedWith('\'');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWithDoubleQuote(this string value) => value?.SurroundedWith('\"');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToLowerCase(this string source) => source?.ToLower(CultureInfo.InvariantCulture);

//// ncrunch: no coverage start

        public static char ToLowerCase(this char source)
        {
            if ((uint)(source - 'A') <= 'Z' - 'A')
            {
                return (char)(source + DifferenceBetweenUpperAndLowerCaseAscii);
            }

            if ((uint)(source - 'a') <= 'z' - 'a')
            {
                return source;
            }

            return char.ToLowerInvariant(source);
        }

        /// <summary>
        /// Gets a <see cref="string"/> where the specified character is lower-case.
        /// </summary>
        /// <param name="source">
        /// The original text.
        /// </param>
        /// <param name="index">
        /// The zero-based index inside <paramref name="source"/> that shall be changed into lower-case.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> where the specified character at <paramref name="index"/> is lower-case.
        /// </returns>
        public static string ToLowerCaseAt(this string source, int index)
        {
            if (source is null)
            {
                return null;
            }

            if (index >= source.Length)
            {
                return source;
            }

            var character = source[index];

            if (character.IsLowerCase())
            {
                return source;
            }

            return MakeLowerCaseAt(source.AsSpan(), index);
        }

        /// <summary>
        /// Gets a <see cref="string"/> where the specified character is lower-case.
        /// </summary>
        /// <param name="source">
        /// The original text.
        /// </param>
        /// <param name="index">
        /// The zero-based index inside <paramref name="source"/> that shall be changed into lower-case.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> where the specified character at <paramref name="index"/> is lower-case.
        /// </returns>
        public static string ToLowerCaseAt(this ReadOnlySpan<char> source, int index)
        {
            if (index >= source.Length || source[index].IsLowerCase())
            {
                return source.ToString();
            }

            return MakeLowerCaseAt(source, index);
        }

        public static char ToUpperCase(this char source)
        {
            if ((uint)(source - 'a') <= 'z' - 'a')
            {
                return (char)(source - DifferenceBetweenUpperAndLowerCaseAscii);
            }

            if ((uint)(source - 'A') <= 'Z' - 'A')
            {
                return source;
            }

            return char.ToUpperInvariant(source);
        }

        /// <summary>
        /// Gets a <see cref="string"/> where the specified character is upper-case.
        /// </summary>
        /// <param name="source">
        /// The original text.
        /// </param>
        /// <param name="index">
        /// The zero-based index inside <paramref name="source"/> that shall be changed into upper-case.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> where the specified character at <paramref name="index"/> is upper-case.
        /// </returns>
        public static string ToUpperCaseAt(this string source, int index)
        {
            if (source is null)
            {
                return null;
            }

            if (index >= source.Length)
            {
                return source;
            }

            var character = source[index];

            if (character.IsUpperCase())
            {
                return source;
            }

            return MakeUpperCaseAt(source.AsSpan(), index);
        }

        //// ncrunch: no coverage end

        /// <summary>
        /// Gets a <see cref="string"/> where the specified character is upper-case.
        /// </summary>
        /// <param name="source">
        /// The original text.
        /// </param>
        /// <param name="index">
        /// The zero-based index inside <paramref name="source"/> that shall be changed into upper-case.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> where the specified character at <paramref name="index"/> is upper-case.
        /// </returns>
        public static string ToUpperCaseAt(this ReadOnlySpan<char> source, int index)
        {
            if (index >= source.Length || source[index].IsUpperCase())
            {
                return source.ToString();
            }

            return MakeUpperCaseAt(source, index);
        }

        /// <summary>
        /// Encapsulates the given term with a space or parenthesis before and a delimiter character behind.
        /// </summary>
        /// <param name="value">
        /// The term to place a space or parenthesis before and a delimiter character behind each single item.
        /// </param>
        /// <returns>
        /// An array of encapsulated terms.
        /// </returns>
        public static string[] WithDelimiters(this string value) => WithDelimiters(new[] { value });

        /// <summary>
        /// Encapsulates the given terms with a space or parenthesis before and a delimiter character behind.
        /// </summary>
        /// <param name="values">
        /// The terms to place a space or parenthesis before and a delimiter character behind each single item.
        /// </param>
        /// <returns>
        /// An array of encapsulated terms.
        /// </returns>
        public static string[] WithDelimiters(this string[] values)
        {
            var delimiters = Constants.Comments.Delimiters;
            var result = new string[2 * delimiters.Length * values.Length];

            var resultIndex = 0;

            var delimitersLength = delimiters.Length;
            var valuesLength = values.Length;

            for (var delimitersIndex = 0; delimitersIndex < delimitersLength; delimitersIndex++)
            {
                var delimiter = delimiters[delimitersIndex];

                for (var valuesIndex = 0; valuesIndex < valuesLength; valuesIndex++)
                {
                    var value = values[valuesIndex];

                    result[resultIndex++] = ' '.ConcatenatedWith(value, delimiter);
                    result[resultIndex++] = '('.ConcatenatedWith(value, delimiter);
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Without(this string value, char character) => value.Without(character.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Without(this string value, string phrase) => value.Replace(phrase, string.Empty);

        public static string Without(this string value, string[] phrases) => new StringBuilder(value).Without(phrases).Trim(); // ncrunch: no coverage

        public static StringBuilder Without(this StringBuilder value, string phrase) => value.ReplaceWithCheck(phrase, string.Empty); // ncrunch: no coverage

        public static StringBuilder Without(this StringBuilder value, string[] phrases) => value.ReplaceAllWithCheck(phrases, string.Empty); // ncrunch: no coverage

        public static string WithoutFirstWord(this string value) => WithoutFirstWord(value.AsSpan()).ToString();

        public static ReadOnlySpan<char> WithoutFirstWord(this ReadOnlySpan<char> value)
        {
            var text = value.TrimStart();

            var firstSpace = text.IndexOfAny(Constants.WhiteSpaceCharacters);

            if (firstSpace < 0)
            {
                // might happen if the text contains a <see> or some other XML element as second word; therefore we only return a space
                return " ".AsSpan();
            }

            return text.Slice(firstSpace);
        }

        public static ReadOnlySpan<char> WithoutFirstWords(this string value, params string[] words) => WithoutFirstWords(value.AsSpan(), words);

        public static ReadOnlySpan<char> WithoutFirstWords(this ReadOnlySpan<char> value, params string[] words)
        {
            var text = value.TrimStart();

            var wordsLength = words.Length;

            for (var index = 0; index < wordsLength; index++)
            {
                var word = words[index];

                if (text.FirstWord().Equals(word, StringComparison.OrdinalIgnoreCase))
                {
                    text = text.WithoutFirstWord().TrimStart();
                }
            }

            return text.TrimStart();
        }

        public static string WithoutNumberSuffix(this string value)
        {
            if (value is null)
            {
                return null;
            }

            var totalLength = value.Length - 1;
            var end = totalLength;

            while (end >= 0)
            {
                if (value[end].IsNumber())
                {
                    end--;
                }
                else
                {
                    end++; // fix last character

                    break;
                }
            }

            return end >= 0 && end <= totalLength
                   ? value.Substring(0, end)
                   : value;
        }

        public static ReadOnlySpan<char> WithoutNumberSuffix(this ReadOnlySpan<char> value)
        {
            if (value.Length == 0)
            {
                return ReadOnlySpan<char>.Empty;
            }

            var totalLength = value.Length - 1;
            var end = totalLength;

            while (end >= 0)
            {
                if (value[end].IsNumber())
                {
                    end--;
                }
                else
                {
                    end++; // fix last character

                    break;
                }
            }

            return end >= 0 && end <= totalLength
                   ? value.Slice(0, end)
                   : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string WithoutQuotes(this string value) => value.Without(@"""");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder WithoutParaTags(this StringBuilder value) => value.Without(Constants.ParaTags); // ncrunch: no coverage

        public static IEnumerable<StringBuilder> WithoutParaTags(this IEnumerable<string> values) => values.Select(_ => new StringBuilder(_).WithoutParaTags()); // ncrunch: no coverage

//// ncrunch: no coverage start

        public static string WithoutSuffix(this string value, string suffix)
        {
            if (value is null)
            {
                return null;
            }

            if (value.EndsWith(suffix, StringComparison.Ordinal))
            {
                var length = value.Length - suffix.Length;

                if (length <= 0)
                {
                    return string.Empty;
                }

                return value.Substring(0, length);
            }

            return value;
        }

        public static ReadOnlySpan<char> WithoutSuffix(this ReadOnlySpan<char> value, char suffix)
        {
            if (value.EndsWith(suffix))
            {
                var length = value.Length - 1;

                return length <= 0
                       ? ReadOnlySpan<char>.Empty
                       : value.Slice(0, length);
            }

            return value;
        }

        public static ReadOnlySpan<char> WithoutSuffix(this ReadOnlySpan<char> value, string suffix, StringComparison comparison = StringComparison.Ordinal)
        {
            if (suffix != null)
            {
                var length = value.Length - suffix.Length;

                if (length >= 0 && value.EndsWith(suffix, comparison))
                {
                    return length > 0
                           ? value.Slice(0, length)
                           : ReadOnlySpan<char>.Empty;
                }
            }

            return value;
        }

//// ncrunch: no coverage end

        public static ReadOnlySpan<char> WithoutSuffixes(this ReadOnlySpan<char> value, string[] suffixes)
        {
            return RemoveSuffixes(RemoveSuffixes(value)); // do it twice to remove consecutive suffixes

            ReadOnlySpan<char> RemoveSuffixes(ReadOnlySpan<char> slice)
            {
                var suffixesLength = suffixes.Length;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var index = 0; index < suffixesLength; index++)
                {
                    var suffix = suffixes[index].AsSpan();

                    if (slice.EndsWith(suffix))
                    {
                        var length = slice.Length - suffix.Length;

                        if (length <= 0)
                        {
                            return ReadOnlySpan<char>.Empty;
                        }

                        slice = slice.Slice(0, length);
                    }
                }

                return slice;
            }
        }

        public static WordsReadOnlySpanEnumerator WordsAsSpan(this ReadOnlySpan<char> value) => new WordsReadOnlySpanEnumerator(value);

//// ncrunch: no coverage start

        private static bool HasWhitespaces(this ReadOnlySpan<char> value, int start = 0)
        {
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

        private static string MakeUpperCaseAt(ReadOnlySpan<char> source, int index)
        {
            unsafe
            {
                var length = source.Length;

                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                source.CopyTo(bufferSpan);

                buffer[index] = buffer[index].ToUpperCase();

                return new string(buffer, 0, length);
            }
        }

        private static string MakeLowerCaseAt(ReadOnlySpan<char> source, int index)
        {
            unsafe
            {
                var length = source.Length;

                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                source.CopyTo(bufferSpan);

                buffer[index] = buffer[index].ToLowerCase();

                return new string(buffer, 0, length);
            }
        }

        private static bool QuickCompare(ReadOnlySpan<char> value, ReadOnlySpan<char> others, StringComparison comparison)
        {
            var valueLength = value.Length;
            var othersLength = others.Length;

            if (valueLength > othersLength)
            {
                // continue to check
                return true;
            }

            if (valueLength < othersLength)
            {
                // cannot match
                return false;
            }

            // both are same length, so perform a quick compare first
            if (valueLength > QuickCompareLengthThreshold)
            {
                switch (comparison)
                {
                    case StringComparison.Ordinal:
                        return QuickCompareOrdinal(value, others);

                    case StringComparison.OrdinalIgnoreCase:
                        return QuickCompareOrdinalIgnoreCase(value, others);
                }
            }

            // continue to check
            return true;
        }

        private static bool QuickCompareOrdinal(ReadOnlySpan<char> value, ReadOnlySpan<char> others)
        {
            var length = value.Length;

            if (length != others.Length && length < 2)
            {
                return true;
            }

            if (value[0] != others[0])
            {
                // they do not fit as characters do not match
                return false;
            }

            var lastIndex = length - 1;

            if (value[lastIndex] != others[lastIndex])
            {
                // they do not fit as characters do not match
                return false;
            }

            // continue to check
            return true;
        }

        private static unsafe bool QuickCompareOrdinalIgnoreCase(ReadOnlySpan<char> value, ReadOnlySpan<char> others)
        {
            var length = value.Length;

            if (length != others.Length && length < QuickCompareLengthThreshold)
            {
                return true;
            }

            // compare in-memory for performance reasons
            fixed (char* ap = &MemoryMarshal.GetReference(value))
            {
                fixed (char* bp = &MemoryMarshal.GetReference(others))
                {
                    var a = ap;
                    var b = bp;

                    int charA = *a;
                    int charB = *b;

                    // uppercase both chars - notice that we need just one compare per char
                    if ((uint)(charA - 'a') <= 'z' - 'a')
                    {
                        charA -= DifferenceBetweenUpperAndLowerCaseAscii;
                    }

                    if ((uint)(charB - 'a') <= 'z' - 'a')
                    {
                        charB -= DifferenceBetweenUpperAndLowerCaseAscii;
                    }

                    if (charA != charB)
                    {
                        // they do not fit as characters do not match
                        return false;
                    }

                    var lastIndex = length - 1;
                    charA = *(a + lastIndex);
                    charB = *(b + lastIndex);

                    // uppercase both chars - notice that we need just one compare per char
                    if ((uint)(charA - 'a') <= 'z' - 'a')
                    {
                        charA -= DifferenceBetweenUpperAndLowerCaseAscii;
                    }

                    if ((uint)(charB - 'a') <= 'z' - 'a')
                    {
                        charB -= DifferenceBetweenUpperAndLowerCaseAscii;
                    }

                    if (charA != charB)
                    {
                        // they do not fit as characters do not match
                        return false;
                    }

                    var middleIndex = length / 2;
                    charA = *(a + middleIndex);
                    charB = *(b + middleIndex);

                    // uppercase both chars - notice that we need just one compare per char
                    if ((uint)(charA - 'a') <= 'z' - 'a')
                    {
                        charA -= DifferenceBetweenUpperAndLowerCaseAscii;
                    }

                    if ((uint)(charB - 'a') <= 'z' - 'a')
                    {
                        charB -= DifferenceBetweenUpperAndLowerCaseAscii;
                    }

                    if (charA != charB)
                    {
                        // they do not fit as characters do not match
                        return false;
                    }

                    var indexPart1 = length / 3;
                    charA = *(a + indexPart1);
                    charB = *(b + indexPart1);

                    // uppercase both chars - notice that we need just one compare per char
                    if ((uint)(charA - 'a') <= 'z' - 'a')
                    {
                        charA -= DifferenceBetweenUpperAndLowerCaseAscii;
                    }

                    if ((uint)(charB - 'a') <= 'z' - 'a')
                    {
                        charB -= DifferenceBetweenUpperAndLowerCaseAscii;
                    }

                    if (charA != charB)
                    {
                        // they do not fit as characters do not match
                        return false;
                    }

                    var indexPart2 = 2 * indexPart1;
                    charA = *(a + indexPart2);
                    charB = *(b + indexPart2);

                    // uppercase both chars - notice that we need just one compare per char
                    if ((uint)(charA - 'a') <= 'z' - 'a')
                    {
                        charA -= DifferenceBetweenUpperAndLowerCaseAscii;
                    }

                    if ((uint)(charB - 'a') <= 'z' - 'a')
                    {
                        charB -= DifferenceBetweenUpperAndLowerCaseAscii;
                    }

                    if (charA != charB)
                    {
                        // they do not fit as characters do not match
                        return false;
                    }
                }
            }

            // continue to check
            return true;
        }

//// ncrunch: no coverage end
    }
}