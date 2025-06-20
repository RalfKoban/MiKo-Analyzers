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
//// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace System
{
    internal static class StringExtensions
    {
        private const int QuickSubstringProbeLengthThreshold = 4;

        private const int DifferenceBetweenUpperAndLowerCaseAscii = 0x20; // valid for Roman ASCII characters ('A' ... 'Z')

        private static readonly char[] GenericTypeArgumentSeparator = { ',' };

        private static readonly Regex HyperlinkRegex = new Regex(@"(www|ftp:|ftps:|http:|https:)+[^\s]+[\w]", RegexOptions.Compiled, 150.Milliseconds());

        private static readonly Regex PascalCasingRegex = new Regex("[a-z]+[A-Z]+", RegexOptions.Compiled, 100.Milliseconds());

        public static string AdjustFirstWord(this string value, in FirstWordHandling handling)
        {
            if (value.StartsWith('<'))
            {
                return value;
            }

            var valueSpan = value.AsSpan();

            string word;

            if (handling.HasSet(FirstWordHandling.StartLowerCase))
            {
                var firstWord = valueSpan.FirstWord();

                // only make lower case in case we have a word that is not all in upper case
                word = firstWord.Length > 1 && firstWord.IsAllUpperCase()
                       ? firstWord.ToString()
                       : firstWord.ToLowerCaseAt(0);
            }
            else
            {
                var firstWord = valueSpan.FirstWord();

                word = handling.HasSet(FirstWordHandling.StartUpperCase)
                       ? firstWord.ToUpperCaseAt(0)
                       : firstWord.ToString();
            }

            // build continuation here because the word length may change based on the infinite term
            var continuation = valueSpan.TrimStart().Slice(word.Length);

            if (handling.HasSet(FirstWordHandling.MakeInfinite))
            {
                word = Verbalizer.MakeInfiniteVerb(word);
            }
            else if (handling.HasSet(FirstWordHandling.MakePlural))
            {
                word = Pluralizer.MakePluralName(word);
            }
            else if (handling.HasSet(FirstWordHandling.MakeThirdPersonSingular))
            {
                word = Verbalizer.MakeThirdPersonSingularVerb(word);
            }

            if (handling.HasSet(FirstWordHandling.KeepSingleLeadingSpace))
            {
                // only keep it if there is already a leading space (otherwise it may be on the same line without any leading space, and we would fix it in a wrong way)
                if (value.StartsWith(' '))
                {
                    return ' '.ConcatenatedWith(word, continuation);
                }
            }

            return word.ConcatenatedWith(continuation);
        }

        public static int[] AllIndicesOf(this string value, string finding, in StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            // Perf: This code sits on the hot path and is invoked quite often (several million times), so we directly use 'string.IsNullOrWhiteSpace(value)'
            //       (otherwise, it would cost about 1/10 of the overall time here which is - given the several million times - recognizable)
            if (string.IsNullOrWhiteSpace(value))
            {
                return Array.Empty<int>();
            }

            if (finding.Length > value.Length)
            {
                return Array.Empty<int>();
            }

            if (comparison is StringComparison.Ordinal)
            {
                // Perf: about 1/3 the times the strings are compared ordinal, so splitting this up increases the overall performance significantly
                return AllIndicesOrdinal(value.AsSpan(), finding.AsSpan());
            }

            return AllIndicesNonOrdinal(value, finding, comparison);
        }

        public static int[] AllIndicesOf(this in ReadOnlySpan<char> value, string finding, in StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return Array.Empty<int>();
            }

            if (finding.Length > value.Length)
            {
                return Array.Empty<int>();
            }

            if (comparison is StringComparison.Ordinal)
            {
                // performance optimization for 'StringComparison.Ordinal' to avoid multiple strings from being created (see 'IndexOf' method inside 'MemoryExtensions')
                return AllIndicesOrdinal(value, finding.AsSpan());
            }

            return AllIndicesNonOrdinal(value.ToString(), finding, comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder AsCachedBuilder(this string value) => StringBuilderCache.Acquire(value.Length).Append(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToStringAndRelease(this StringBuilder value) => StringBuilderCache.GetStringAndRelease(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SyntaxToken AsToken(this string source, in SyntaxKind kind = SyntaxKind.StringLiteralToken)
        {
            if (kind is SyntaxKind.IdentifierToken)
            {
                return SyntaxFactory.Identifier(source);
            }

            return SyntaxFactory.Token(default, kind, source, source, default);
        }

        public static InterpolatedStringTextSyntax AsInterpolatedString(this in ReadOnlySpan<char> value) => value.ToString().AsInterpolatedString();

        public static InterpolatedStringTextSyntax AsInterpolatedString(this string value) => SyntaxFactory.InterpolatedStringText(value.AsToken(SyntaxKind.InterpolatedStringTextToken));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith(this IEnumerable<string> values) => string.Concat(values.WhereNotNull());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith(this IEnumerable<string> values, string separator) => string.Join(separator, values);

        public static string ConcatenatedWith(this in char value, string arg0)
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

        public static string ConcatenatedWith(this in char value, in ReadOnlySpan<char> arg0)
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

        public static string ConcatenatedWith(this string value, in ReadOnlySpan<char> span)
        {
            var spanLength = span.Length;

            if (spanLength is 0)
            {
                return value;
            }

            var valueLength = value?.Length ?? 0;

            if (valueLength is 0)
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

        public static string ConcatenatedWith(this string value, in char arg0)
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

        public static string ConcatenatedWith(this in ReadOnlySpan<char> value, in char arg0)
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

        public static string ConcatenatedWith(this in ReadOnlySpan<char> value, string arg0)
        {
            var spanLength = value.Length;

            if (spanLength is 0)
            {
                return arg0;
            }

            var arg0Length = arg0?.Length ?? 0;

            if (arg0Length is 0)
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

        public static string ConcatenatedWith(this in char value, string arg0, in char arg1)
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

        public static string ConcatenatedWith(this in char value, in ReadOnlySpan<char> arg0, in char arg1)
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

        public static string ConcatenatedWith(this in char value, string arg0, in ReadOnlySpan<char> arg1)
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

        public static string ConcatenatedWith(this in ReadOnlySpan<char> value, in char arg0, string arg1)
        {
            var valueLength = value.Length;

            if (valueLength is 0)
            {
                return string.Concat(arg0, arg1);
            }

            var arg1Length = arg1?.Length ?? 0;

            var length = valueLength + 1 + arg1Length;

            unsafe
            {
                var buffer = stackalloc char[length];
                var bufferSpan = new Span<char>(buffer, length);

                value.CopyTo(bufferSpan);
                buffer[valueLength] = arg0;
                arg1.AsSpan().CopyTo(bufferSpan.Slice(valueLength + 1));

                return new string(buffer, 0, length);
            }
        }

        public static string ConcatenatedWith(this in ReadOnlySpan<char> value, string arg0, string arg1)
        {
            var valueLength = value.Length;

            if (valueLength is 0)
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

        public static string ConcatenatedWith(this in ReadOnlySpan<char> value, string arg0, in ReadOnlySpan<char> arg1)
        {
            var valueLength = value.Length;

            if (valueLength is 0)
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

        public static string ConcatenatedWith(this string value, string arg0, in ReadOnlySpan<char> arg1)
        {
            if (value is null)
            {
                return arg0.ConcatenatedWith(arg1);
            }

            var valueLength = value.Length;

            if (value.Length is 0)
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

        public static string ConcatenatedWith(this string value, in ReadOnlySpan<char> arg0, in char arg1)
        {
            if (value is null)
            {
                return arg0.ConcatenatedWith(arg1);
            }

            var valueLength = value.Length;

            if (value.Length is 0)
            {
                return arg0.ConcatenatedWith(arg1);
            }

            var arg0Length = arg0.Length;

            var length = valueLength + arg0Length + 1;

            unsafe
            {
                var buffer = stackalloc char[length];
                buffer[length - 1] = arg1;

                var bufferSpan = new Span<char>(buffer, length);

                value.AsSpan().CopyTo(bufferSpan);
                arg0.CopyTo(bufferSpan.Slice(valueLength));

                return new string(buffer, 0, length);
            }
        }

        public static string ConcatenatedWith(this string value, in ReadOnlySpan<char> arg0, string arg1)
        {
            if (value is null)
            {
                return arg0.ConcatenatedWith(arg1);
            }

            var valueLength = value.Length;

            if (value.Length is 0)
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

        public static string ConcatenatedWith(this in ReadOnlySpan<char> value, in char arg0, string arg1, in char arg2)
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

                buffer[valueLength] = arg0;
                buffer[valueLength + arg1Length + 1] = arg2;

                arg1.AsSpan().CopyTo(bufferSpan.Slice(valueLength + 1));

                return new string(buffer, 0, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this string value, in char c) => value?.IndexOf(c) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this in ReadOnlySpan<char> value, in char c) => value.Length > 0 && value.IndexOf(c) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this in ReadOnlySpan<char> value, in ReadOnlySpan<char> finding)
        {
            if (finding.Length > value.Length)
            {
                return false;
            }

            return value.IndexOf(finding) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this in ReadOnlySpan<char> value, string finding)
        {
            if (finding is null)
            {
                return false;
            }

            return value.Contains(finding.AsSpan());
        }

        public static bool Contains(this string value, string finding, in StringComparison comparison)
        {
            var difference = finding.Length - value.Length;

            if (difference < 0)
            {
                if (comparison is StringComparison.Ordinal)
                {
                    return value.AsSpan().IndexOf(finding.AsSpan()) >= 0;
                }

                return value.IndexOf(finding, comparison) >= 0;
            }

            if (difference is 0)
            {
                return QuickEquals(comparison);

                bool QuickEquals(in StringComparison c)
                {
                    const int QuickInspectionChars = 2;

                    var valueLength = value.Length;

                    if (valueLength > QuickInspectionChars)
                    {
                        var valueSpan = value.AsSpan(valueLength - QuickInspectionChars, QuickInspectionChars);
                        var findingSpan = finding.AsSpan(finding.Length - QuickInspectionChars, QuickInspectionChars);

                        if (valueSpan.Equals(findingSpan, c) is false)
                        {
                            return false;
                        }
                    }

                    return value.Equals(finding, c);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this in ReadOnlySpan<char> value, string finding, Func<char, bool> nextCharValidationCallback, in StringComparison comparison) => value.Contains(finding.AsSpan(), nextCharValidationCallback, comparison);

        public static bool Contains(this in ReadOnlySpan<char> value, in ReadOnlySpan<char> finding, Func<char, bool> nextCharValidationCallback, in StringComparison comparison)
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

            do
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
            while (true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsAny(this string value, char[] characters) => value?.IndexOfAny(characters) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<char> characters) => value.Length > 0 && value.IndexOfAny(characters) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsAny(this string value, in ReadOnlySpan<string> phrases) => value.ContainsAny(phrases, StringComparison.OrdinalIgnoreCase);

        public static bool ContainsAny(this string value, in ReadOnlySpan<string> phrases, in StringComparison comparison)
        {
            if (value.HasCharacters())
            {
                var valueSpan = value.AsSpan();
                var phrasesLength = phrases.Length;
                var ordinalComparison = comparison is StringComparison.Ordinal;

                for (var index = 0; index < phrasesLength; index++)
                {
                    var phrase = phrases[index];
                    var phraseSpan = phrase.AsSpan();

                    // no separate handling for StringComparison.Ordinal here as that happens around 30 times out of 90_000_000 times; so almost never
                    if (QuickSubstringProbe(valueSpan, phraseSpan, comparison))
                    {
                        if (ordinalComparison)
                        {
                            if (valueSpan.Contains(phraseSpan))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (value.Contains(phrase, comparison))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static int CountLeadingWhitespaces(this string value, int start = 0)
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

        public static int CountTrailingWhitespaces(this string value, in int start = 0)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EndsWith(this string value, in char character) => value.HasCharacters() && value[value.Length - 1] == character;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EndsWith(this in ReadOnlySpan<char> value, in char character) => value.Length > 0 && value[value.Length - 1] == character;

        public static bool EndsWith(this in ReadOnlySpan<char> value, string characters, in StringComparison comparison)
        {
            var span = characters.AsSpan();

            return span.Length > 0 && value.EndsWith(span, comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EndsWithAny(this string value, in ReadOnlySpan<char> suffixCharacters) => value.HasCharacters() && suffixCharacters.Contains(value[value.Length - 1]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EndsWithAny(this string value, in ReadOnlySpan<string> suffixes) => value.EndsWithAny(suffixes, StringComparison.OrdinalIgnoreCase);

        public static bool EndsWithAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<char> suffixCharacters)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EndsWithAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> suffixes) => value.EndsWithAny(suffixes, StringComparison.OrdinalIgnoreCase);

        public static bool EndsWithAny(this string value, in ReadOnlySpan<string> suffixes, in StringComparison comparison)
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

        public static bool EndsWithAny(this string value, List<string> suffixes, in StringComparison comparison)
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

        public static bool EndsWithAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> suffixes, in StringComparison comparison)
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

        public static bool EndsWithCommonNumber(this in ReadOnlySpan<char> value) => value.EndsWithNumber() && value.EndsWithAny(Constants.Markers.OSBitNumbers) is false;

        public static bool EndsWithNumber(this string value) => value.HasCharacters() && value[value.Length - 1].IsNumber();

        public static bool EndsWithNumber(this in ReadOnlySpan<char> value) => value.Length > 0 && value[value.Length - 1].IsNumber();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this string value, in ReadOnlySpan<char> other, in StringComparison comparison) => value != null && value.AsSpan().Equals(other, comparison);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this in ReadOnlySpan<char> value, string other, in StringComparison comparison) => other != null && value.Equals(other.AsSpan(), comparison);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny(this string value, IEnumerable<string> phrases) => EqualsAny(value, phrases, StringComparison.OrdinalIgnoreCase);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> phrases) => EqualsAny(value, phrases, StringComparison.OrdinalIgnoreCase);

        public static bool EqualsAny(this string value, in ReadOnlySpan<string> phrases, in StringComparison comparison)
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

        public static bool EqualsAny(this string value, IEnumerable<string> phrases, in StringComparison comparison)
        {
            if (phrases is string[] array)
            {
                return value.EqualsAny(array.AsSpan(), comparison);
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

        public static bool EqualsAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> phrases, in StringComparison comparison)
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
            if (value.IsNullOrEmpty())
            {
                return value;
            }

            var span = value.AsSpan();
            var word = FirstWord(span);

            return word != span
                   ? word.ToString()
                   : value;
        }

        public static ReadOnlySpan<char> FirstWord(this in ReadOnlySpan<char> value)
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

        public static string SecondWord(this string value) => SecondWord(value.AsSpan()).ToString();

        public static ReadOnlySpan<char> SecondWord(this in ReadOnlySpan<char> value) => value.WithoutFirstWord().FirstWord();

        public static string ThirdWord(this string value) => ThirdWord(value.AsSpan()).ToString();

        public static ReadOnlySpan<char> ThirdWord(this in ReadOnlySpan<char> value) => value.WithoutFirstWord().WithoutFirstWord().FirstWord();

        public static string LastWord(this string value)
        {
            if (value.IsNullOrEmpty())
            {
                return value;
            }

            var span = value.AsSpan();
            var word = LastWord(span);

            return word != span
                   ? word.ToString()
                   : value;
        }

        public static ReadOnlySpan<char> LastWord(this in ReadOnlySpan<char> value)
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
        public static string FormatWith(this string format, ISymbol arg0) => string.Format(format, arg0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, string arg0) => string.Format(format, arg0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, string arg0, string arg1) => string.Format(format, arg0, arg1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, string arg0, string arg1, string arg2) => string.Format(format, arg0, arg1, arg2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, string arg0, string arg1, string arg2, string arg3) => string.Format(format, arg0, arg1, arg2, arg3);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetNameOnlyPart(this string value) => GetNameOnlyPart(value.AsSpan());

        public static string GetNameOnlyPart(this in ReadOnlySpan<char> value)
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

        public static string GetNameOnlyPartWithoutGeneric(this in ReadOnlySpan<char> value)
        {
            var genericIndexStart = value.IndexOf('<');

            var name = genericIndexStart > 0
                       ? value.Slice(0, genericIndexStart)
                       : value;

            return name.GetPartAfterLastDot().ToString();
        }

        public static ReadOnlySpan<char> GetPartAfterLastDot(this in ReadOnlySpan<char> value) => value.Slice(value.LastIndexOf('.') + 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasCollectionMarker(this string value) => value.EndsWithAny(Constants.Markers.Collections);

        public static bool HasEntityMarker(this string value)
        {
            var hasMarker = value.ContainsAny(Constants.Markers.Models, StringComparison.OrdinalIgnoreCase);

            if (hasMarker)
            {
                if (value.ContainsAny(Constants.Markers.ViewModels, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (value.ContainsAny(Constants.Markers.SpecialModels, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return hasMarker;
        }

        // Perf: As this method is invoked several million times, we directly use 'string.IsNullOrEmpty(value)' here instead of our own extension method helper
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasCharacters(this string value) => string.IsNullOrEmpty(value) is false;

        public static bool HasUpperCaseLettersAbove(this string value, in ushort limit) => value != null && HasUpperCaseLettersAbove(value.AsSpan(), limit);

        public static bool HasUpperCaseLettersAbove(this in ReadOnlySpan<char> value, in ushort limit)
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
                case 3:
                {
                    var item0 = items[0];
                    var item1 = items[1];
                    var item2 = items[2];

                    var builder = StringBuilderCache.Acquire(item0.Length + Separator.Length + item1.Length + separatorForLast.Length + item2.Length)
                                                    .Append(item0)
                                                    .Append(Separator)
                                                    .Append(item1)
                                                    .Append(separatorForLast)
                                                    .Append(item2);

                    return StringBuilderCache.GetStringAndRelease(builder);
                }

                case 4:
                {
                    var item0 = items[0];
                    var item1 = items[1];
                    var item2 = items[2];
                    var item3 = items[3];

                    var builder = StringBuilderCache.Acquire(item0.Length + Separator.Length + item1.Length + Separator.Length + item2.Length + separatorForLast.Length + item3.Length)
                                                    .Append(item0)
                                                    .Append(Separator)
                                                    .Append(item1)
                                                    .Append(Separator)
                                                    .Append(item2)
                                                    .Append(separatorForLast)
                                                    .Append(item3);

                    return StringBuilderCache.GetStringAndRelease(builder);
                }

                default: return string.Concat(items.Take(count - 1).ConcatenatedWith(Separator), separatorForLast, items[count - 1]);
            }
        }

        public static string HumanizedTakeFirst(this in ReadOnlySpan<char> value, in int max)
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

        public static int IndexOfAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> phrases, in StringComparison comparison)
        {
            if (value.Length > 0)
            {
                // performance optimization to avoid unnecessary 'ToString' calls on 'ReadOnlySpan' (see implementation inside MemoryExtensions)
                if (comparison is StringComparison.Ordinal)
                {
                    var phrasesLength = phrases.Length;

                    for (var i = 0; i < phrasesLength; i++)
                    {
                        var phrase = phrases[i];

                        var index = value.IndexOf(phrase.AsSpan()); // performs ordinal comparison

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

        public static int IndexOfAny(this string value, in ReadOnlySpan<string> phrases, in StringComparison comparison)
        {
            if (value.HasCharacters())
            {
                if (comparison is StringComparison.Ordinal)
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

        public static int LastIndexOfAny(this string value, in ReadOnlySpan<string> phrases, in StringComparison comparison)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        public static bool IsLetter(this in char value) => char.IsLetter(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowerCase(this in char value) => char.IsLower(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowerCaseLetter(this in char value) => value.IsLetter() && value.IsLowerCase();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this in ReadOnlySpan<char> value) => value.IsEmpty;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        public static bool IsNullOrWhiteSpace(this in ReadOnlySpan<char> value)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumber(this in char value) => char.IsNumber(value);

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
        public static bool IsSentenceEnding(this in char value)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSingleWord(this string value) => value != null && IsSingleWord(value.AsSpan());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSingleWord(this in ReadOnlySpan<char> value) => value.HasWhitespaces() is false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAllUpperCase(this string value) => value.AsSpan().IsAllUpperCase();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAllUpperCase(this in ReadOnlySpan<char> value)
        {
            var valueLength = value.Length;

            if (valueLength > 0)
            {
                for (var i = 0; i < valueLength; i++)
                {
                    if (value[i].IsUpperCase() is false)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUpperCase(this in char value)
        {
            if (value.IsAsciiLetterLower())
            {
                return false;
            }

            if (value.IsAsciiLetterUpper())
            {
                return true;
            }

            if ((uint)(value - '@') <= '@')
            {
                return false;
            }

            return IsUpperCaseWithSwitch(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUpperCaseLetter(this in char value) => value.IsUpperCase() && value.IsLetter();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteSpace(this in char value)
        {
            if (value > ' ')
            {
                return value >= 127 && char.IsWhiteSpace(value);
            }

            switch (value)
            {
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                case '\v':
                case '\f':
                    return true;

                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this string value, in char character) => value.HasCharacters() && value[0] == character;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this string value, in ReadOnlySpan<char> characters) => value.HasCharacters() && value.AsSpan().StartsWith(characters);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this in ReadOnlySpan<char> value, in char character) => value.Length > 0 && value[0] == character;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this in ReadOnlySpan<char> value, string characters) => characters.HasCharacters() && value.StartsWith(characters.AsSpan());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this string value, in ReadOnlySpan<char> characters, in StringComparison comparison)
        {
            // for the moment this does not need a different handling for 'StringComparison' as it is used too few times to have any benefit
            return value.HasCharacters() && value.AsSpan().StartsWith(characters, comparison);
        }

        public static bool StartsWith(this in ReadOnlySpan<char> value, string characters, in StringComparison comparison)
        {
            var others = characters.AsSpan();

            // perform quick check
            if (QuickSubstringProbe(value, others, comparison))
            {
                return value.StartsWith(others, comparison);
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWithAny(this string value, IEnumerable<char> characters) => value.HasCharacters() && characters.Contains(value[0]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWithAny(this in ReadOnlySpan<char> value, IEnumerable<char> characters) => value.Length > 0 && characters.Contains(value[0]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWithAny(this string value, in ReadOnlySpan<string> prefixes) => value.StartsWithAny(prefixes, StringComparison.OrdinalIgnoreCase);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWithAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> prefixes) => value.StartsWithAny(prefixes, StringComparison.OrdinalIgnoreCase);

        public static bool StartsWithAny(this string value, in ReadOnlySpan<string> prefixes, in StringComparison comparison)
        {
            if (value.HasCharacters())
            {
                var valueSpan = value.AsSpan();
                var prefixesCount = prefixes.Length;

                for (var index = 0; index < prefixesCount; index++)
                {
                    var prefix = prefixes[index];

                    if (QuickSubstringProbe(valueSpan, prefix.AsSpan(), comparison))
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

        public static bool StartsWithAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> prefixes, in StringComparison comparison)
        {
            if (value.Length > 0)
            {
                var prefixesLength = prefixes.Length;

                for (var index = 0; index < prefixesLength; index++)
                {
                    var prefix = prefixes[index];

                    if (value.StartsWith(prefix, comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWithNumber(this string value) => value.HasCharacters() && value[0].IsNumber();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWith(this string value, in char surrounding) => surrounding.ConcatenatedWith(value, surrounding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWithApostrophe(this string value) => value?.SurroundedWith('\'');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWithDoubleQuote(this string value) => value?.SurroundedWith('\"');

#pragma warning disable CA1308
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToLowerCase(this string source) => source?.ToLowerInvariant();
#pragma warning restore CA1308

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToLowerCase(this in char source)
        {
            if (source.IsAsciiLetterUpper())
            {
                return (char)(source + DifferenceBetweenUpperAndLowerCaseAscii);
            }

            if (source.IsAsciiLetterLower())
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
        public static string ToLowerCaseAt(this string source, in int index)
        {
            if (source is null || index >= source.Length || source[index].IsLowerCase())
            {
                return source;
            }

            return source.AsSpan().MakeLowerCaseAt(index);
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
        public static string ToLowerCaseAt(this in ReadOnlySpan<char> source, in int index)
        {
            if (index >= source.Length || source[index].IsLowerCase())
            {
                return source.ToString();
            }

            return MakeLowerCaseAt(source, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToUpperCase(this in char source)
        {
            if (source.IsAsciiLetterLower())
            {
                return (char)(source - DifferenceBetweenUpperAndLowerCaseAscii);
            }

            if (source.IsAsciiLetterUpper())
            {
                return source;
            }

            return char.ToUpperInvariant(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToUpperCase(this string source) => source?.ToUpper(CultureInfo.InvariantCulture);

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
        public static string ToUpperCaseAt(this string source, in int index)
        {
            if (source is null || index >= source.Length || source[index].IsUpperCase())
            {
                return source;
            }

            return source.AsSpan().MakeUpperCaseAt(index);
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
        public static string ToUpperCaseAt(this in ReadOnlySpan<char> source, in int index)
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
        public static string Without(this string value, in char character) => value.Without(character.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Without(this string value, string phrase) => value.Replace(phrase, string.Empty);

        public static string Without(this string value, in ReadOnlySpan<string> phrases) => value.AsCachedBuilder().Without(phrases).Trimmed().ToStringAndRelease();

        public static string WithoutFirstWord(this string value) => WithoutFirstWord(value.AsSpan()).ToString();

        public static ReadOnlySpan<char> WithoutFirstWord(this in ReadOnlySpan<char> value)
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

        public static ReadOnlySpan<char> WithoutFirstWords(this string value, in ReadOnlySpan<string> words) => WithoutFirstWords(value.AsSpan(), words);

        public static ReadOnlySpan<char> WithoutFirstWords(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> words)
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
            if (value.IsNullOrEmpty())
            {
                return value;
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

        public static ReadOnlySpan<char> WithoutNumberSuffix(this in ReadOnlySpan<char> value)
        {
            if (value.Length is 0)
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

        public static string WithoutSuffix(this string value, string suffix)
        {
            if (value.IsNullOrEmpty())
            {
                return value;
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

        public static ReadOnlySpan<char> WithoutSuffix(this in ReadOnlySpan<char> value, in char suffix)
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

        public static ReadOnlySpan<char> WithoutSuffix(this in ReadOnlySpan<char> value, string suffix, in StringComparison comparison = StringComparison.Ordinal)
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

        public static ReadOnlySpan<char> WithoutSuffixes(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> suffixes)
        {
            // do it twice to remove consecutive suffixes
            return value.RemoveSuffixes(suffixes)
                        .RemoveSuffixes(suffixes);
        }

        public static WordsReadOnlySpanEnumerator WordsAsSpan(this in ReadOnlySpan<char> value) => new WordsReadOnlySpanEnumerator(value);

        private static ReadOnlySpan<char> RemoveSuffixes(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> suffixes)
        {
            if (value.Length <= 0)
            {
                return ReadOnlySpan<char>.Empty;
            }

            var slice = value;
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

        private static bool HasWhitespaces(this in ReadOnlySpan<char> value, int start = 0)
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

        private static string MakeUpperCaseAt(this in ReadOnlySpan<char> source, in int index)
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

        private static string MakeLowerCaseAt(this in ReadOnlySpan<char> source, in int index)
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

        private static bool QuickSubstringProbe(in ReadOnlySpan<char> value, in ReadOnlySpan<char> other, in StringComparison comparison)
        {
            if (value.Length > other.Length)
            {
                // continue to check
                return true;
            }

            if (value.Length < other.Length)
            {
                // cannot match
                return false;
            }

            // both are same length, so perform a quick compare first
            if (value.Length > QuickSubstringProbeLengthThreshold)
            {
                switch (comparison)
                {
                    case StringComparison.Ordinal:
                        return QuickSubstringProbeOrdinal(value, other);

                    case StringComparison.OrdinalIgnoreCase:
                        return QuickSubstringProbeOrdinalIgnoreCase(value, other);
                }
            }

            // continue to check
            return true;
        }

        private static bool QuickSubstringProbeOrdinal(in ReadOnlySpan<char> value, in ReadOnlySpan<char> other)
        {
            var length = value.Length;

            if (length != other.Length && length < QuickSubstringProbeLengthThreshold)
            {
                return true;
            }

            return value[0] == other[0] && value[length - 1] == other[length - 1];
        }

        private static unsafe bool QuickSubstringProbeOrdinalIgnoreCase(in ReadOnlySpan<char> value, in ReadOnlySpan<char> other)
        {
            var length = value.Length;

            if (length != other.Length && length < QuickSubstringProbeLengthThreshold)
            {
                return true;
            }

            // compare in-memory for performance reasons
            fixed (char* ap = &MemoryMarshal.GetReference(value))
            {
                fixed (char* bp = &MemoryMarshal.GetReference(other))
                {
                    if (QuickDiff(ap, bp, 0)) // uppercase both chars - notice that we need just one compare per char
                    {
                        // they do not fit as characters do not match
                        return false;
                    }

                    if (QuickDiff(ap, bp, length - 1)) // uppercase both chars - notice that we need just one compare per char
                    {
                        // they do not fit as characters do not match
                        return false;
                    }

                    if (QuickDiff(ap, bp, length / 2)) // uppercase both chars - notice that we need just one compare per char
                    {
                        // they do not fit as characters do not match
                        return false;
                    }

                    var third = length / 3;

                    if (QuickDiff(ap, bp, third)) // uppercase both chars - notice that we need just one compare per char
                    {
                        // they do not fit as characters do not match
                        return false;
                    }

                    if (QuickDiff(ap, bp, 2 * third)) // uppercase both chars - notice that we need just one compare per char
                    {
                        // they do not fit as characters do not match
                        return false;
                    }
                }
            }

            // continue to check
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool QuickDiff(char* a, char* b, in int index)
        {
            int charA = *(a + index);

            if (charA.IsAsciiLetterLower())
            {
                charA -= DifferenceBetweenUpperAndLowerCaseAscii;
            }

            int charB = *(b + index);

            if (charB.IsAsciiLetterLower())
            {
                charB -= DifferenceBetweenUpperAndLowerCaseAscii;
            }

            return charA != charB;
        }

        private static bool IsUpperCaseWithSwitch(in char value)
        {
            switch (value)
            {
                case '[':
                case ']':
                case '\\':
                case Constants.Underscore:
                    return false;

                case 'Ö':
                case 'Ä':
                case 'Ü':
                    return true;

                default:
                    return char.IsUpper(value);
            }
        }

        private static int[] AllIndicesOrdinal(in ReadOnlySpan<char> span, in ReadOnlySpan<char> other)
        {
            var otherLength = other.Length;

            int[] indices = null;

            for (var index = 0; ; index += otherLength)
            {
                var newIndex = span.Slice(index).IndexOf(other); // performs ordinal comparison

                if (newIndex is -1)
                {
                    // nothing more to find
                    break;
                }

                index += newIndex;

                if (indices is null)
                {
                    indices = new int[1];
                }
                else
                {
                    Array.Resize(ref indices, indices.Length + 1);
                }

                indices[indices.Length - 1] = index;
            }

            return indices ?? Array.Empty<int>();
        }

        private static int[] AllIndicesNonOrdinal(string value, string finding, in StringComparison comparison)
        {
            var findingLength = finding.Length;

            int[] indices = null;

            for (var index = 0; ; index += findingLength)
            {
                index = value.IndexOf(finding, index, comparison);

                if (index is -1)
                {
                    // nothing more to find
                    break;
                }

                if (indices is null)
                {
                    indices = new int[1];
                }
                else
                {
                    Array.Resize(ref indices, indices.Length + 1);
                }

                indices[indices.Length - 1] = index;
            }

            return indices ?? Array.Empty<int>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAsciiLetterLower(this in char value) => IsAsciiLetterLower((int)value); // notice that we need just one compare per char

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAsciiLetterLower(this in int value) => (uint)(value - 'a') <= 'z' - 'a'; // notice that we need just one compare per char

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAsciiLetterUpper(this in char value) => IsAsciiLetterUpper((int)value); // notice that we need just one compare per char

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAsciiLetterUpper(this in int value) => (uint)(value - 'A') <= 'Z' - 'A'; // notice that we need just one compare per char
    }
}