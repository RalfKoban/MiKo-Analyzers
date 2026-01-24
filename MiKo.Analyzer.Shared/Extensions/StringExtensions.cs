using System;
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

using MiKoSolutions.Analyzers.Linguistics;

//// ncrunch: rdi off
//// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="string"/>s.
    /// </summary>
    internal static class StringExtensions
    {
        private const int QuickSubstringProbeLengthThreshold = 4;

        private const int DifferenceBetweenUpperAndLowerCaseAscii = 0x20; // valid for Roman ASCII characters ('A' ... 'Z')

        private const string NumberRegexPattern = @"
                                                       (?<!\w)                    # no word character before
                                                       [+-]?                      # optional sign
                                                       (
                                                           \d{1,3}                # first digit group
                                                           (?: [,_\.] \d{3} )*    # thousands groups
                                                           |
                                                           \d+                    # or plain digits
                                                       )
                                                       (?: [.,] \d+ )?            # optional decimal part
                                                       (?!\w)                     # no word character after
                                                   ";

        private static readonly char[] GenericTypeArgumentSeparator = { ',' };

        private static readonly TimeSpan RegexTimeout = 250.Milliseconds();

        private static readonly Regex HyperlinkRegex = new Regex(@"(www|ftp:|ftps:|http:|https:)+[^\s]+[\w]", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace, RegexTimeout);

        private static readonly Regex PascalCasingRegex = new Regex("[a-z]+[A-Z]+", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace, RegexTimeout);

        private static readonly Regex NumberRegex = new Regex(NumberRegexPattern, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace, RegexTimeout);

        private static readonly Regex OnlyNumberRegex = new Regex("^" + NumberRegexPattern + "$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace, RegexTimeout);

        /// <summary>
        /// Adjusts the first word of the value according to the specified adjustment options.
        /// </summary>
        /// <param name="value">
        /// The original <see cref="string"/> value.
        /// </param>
        /// <param name="adjustment">
        /// A bitwise combination of enumeration values that specifies the adjustment options for the first word.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the adjusted first word.
        /// </returns>
        public static string AdjustFirstWord(this string value, in FirstWordAdjustment adjustment)
        {
            if (value.StartsWith('<'))
            {
                return value;
            }

            var valueSpan = value.AsSpan();

            string word;

            if (adjustment.HasSet(FirstWordAdjustment.StartLowerCase))
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

                word = adjustment.HasSet(FirstWordAdjustment.StartUpperCase)
                       ? firstWord.ToUpperCaseAt(0)
                       : firstWord.ToString();
            }

            // build continuation here because the word length may change based on the infinite term
            var continuation = valueSpan.TrimStart().Slice(word.Length);

            if (adjustment.HasSet(FirstWordAdjustment.MakeInfinite))
            {
                word = Verbalizer.MakeInfiniteVerb(word);
            }
            else if (adjustment.HasSet(FirstWordAdjustment.MakePlural))
            {
                word = Pluralizer.MakePluralName(word);
            }
            else if (adjustment.HasSet(FirstWordAdjustment.MakeThirdPersonSingular))
            {
                word = Verbalizer.MakeThirdPersonSingularVerb(word);
            }

            if (adjustment.HasSet(FirstWordAdjustment.KeepSingleLeadingSpace))
            {
                // only keep it if there is already a leading space (otherwise it may be on the same line without any leading space, and we would fix it in a wrong way)
                if (value.StartsWith(' '))
                {
                    return ' '.ConcatenatedWith(word, continuation);
                }
            }

            return word.ConcatenatedWith(continuation);
        }

        /// <summary>
        /// Finds all indices of the specified <see cref="string"/> within the current <see cref="string"/>, using the specified comparison option.
        /// </summary>
        /// <param name="value">
        /// The original <see cref="string"/> where the finds are performed.
        /// </param>
        /// <param name="finding">
        /// The <see cref="string"/> to seek within the current <see cref="string"/>.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the comparison option to use when finding the <see cref="string"/> (for example, ordinal, ignore case).
        /// The default is <see cref="StringComparison.OrdinalIgnoreCase"/>.
        /// </param>
        /// <returns>
        /// An array of integers representing the zero-based indices of each occurrence of the specified <see cref="string"/>.
        /// </returns>
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

            return AllIndicesNonOrdinal(value, finding);
        }

        /// <summary>
        /// Finds all indices of the specified <see cref="string"/> within the current <see cref="string"/>, using the specified comparison option.
        /// </summary>
        /// <param name="value">
        /// The span of characters where the finds are performed.
        /// </param>
        /// <param name="finding">
        /// The <see cref="string"/> to seek within the current span.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the comparison option to use when finding the <see cref="string"/> (for example, ordinal, ignore case).
        /// The default is <see cref="StringComparison.OrdinalIgnoreCase"/>.
        /// </param>
        /// <returns>
        /// An array of integers representing the zero-based indices of each occurrence of the specified <see cref="string"/>.
        /// </returns>
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

            return AllIndicesNonOrdinal(value.ToString(), finding);
        }

        /// <summary>
        /// Determines whether all characters in the span are uppercase.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if all characters in the span are uppercase; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool AllUpper(this in ReadOnlySpan<char> value)
        {
            var valueLength = value.Length;

            if (valueLength > 0)
            {
                for (var index = 0; index < valueLength; index++)
                {
                    if (char.IsUpper(value[index]) is false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified character exists in the character span.
        /// </summary>
        /// <param name="value">
        /// The span of characters to search in.
        /// </param>
        /// <param name="c">
        /// The character to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character is found; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool Any(this in ReadOnlySpan<char> value, in char c)
        {
            var valueLength = value.Length;

            if (valueLength > 0)
            {
                for (var index = 0; index < valueLength; index++)
                {
                    if (value[index] == c)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="ReadOnlySpan{T}"/> contains any element that satisfies the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the span.
        /// </typeparam>
        /// <param name="source">
        /// The span to evaluate.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if any element satisfies the condition; otherwise, <see langword="false"/>.
        /// </returns>
        /// <seealso cref="None{T}"/>
        public static bool Any<T>(this in ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            var sourceLength = source.Length;

            if (sourceLength > 0)
            {
                for (var index = 0; index < sourceLength; index++)
                {
                    if (predicate(source[index]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="ReadOnlySpan{T}"/> contains no elements that satisfy the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the span.
        /// </typeparam>
        /// <param name="source">
        /// The span to evaluate.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if no elements satisfy the condition; otherwise, <see langword="false"/>.
        /// </returns>
        /// <seealso cref="Any{T}"/>
        public static bool None<T>(this in ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            var sourceLength = source.Length;

            if (sourceLength > 0)
            {
                for (var index = 0; index < sourceLength; index++)
                {
                    if (predicate(source[index]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Creates a cached <see cref="StringBuilder"/> initialized with the specified <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> value to initialize the builder with.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilder"/> from the cache initialized with the specified <see cref="string"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder AsCachedBuilder(this string value) => StringBuilderCache.Acquire(value.Length).Append(value);

        /// <summary>
        /// Converts the <see cref="string"/> to a <see cref="SyntaxToken"/> with the specified kind.
        /// </summary>
        /// <param name="source">
        /// The <see cref="string"/> to convert.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind for the token.
        /// The default is <see cref="SyntaxKind.StringLiteralToken"/>.
        /// </param>
        /// <returns>
        /// A syntax token created from the <see cref="string"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SyntaxToken AsToken(this string source, in SyntaxKind kind = SyntaxKind.StringLiteralToken)
        {
            if (kind is SyntaxKind.IdentifierToken)
            {
                return SyntaxFactory.Identifier(source);
            }

            return SyntaxFactory.Token(default, kind, source, source, default);
        }

        /// <summary>
        /// Converts a character span to an interpolated <see cref="string"/> text syntax node.
        /// </summary>
        /// <param name="value">
        /// The span of characters to convert.
        /// </param>
        /// <returns>
        /// An interpolated <see cref="string"/> text syntax node.
        /// </returns>
        public static InterpolatedStringTextSyntax AsInterpolatedString(this in ReadOnlySpan<char> value) => value.ToString().AsInterpolatedString();

        /// <summary>
        /// Converts a <see cref="string"/> to an interpolated <see cref="string"/> text syntax node.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to convert.
        /// </param>
        /// <returns>
        /// An interpolated <see cref="string"/> text syntax node.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InterpolatedStringTextSyntax AsInterpolatedString(this string value) => SyntaxFactory.InterpolatedStringText(value.AsToken(SyntaxKind.InterpolatedStringTextToken));

        /// <summary>
        /// Converts a <see cref="string"/> to a type syntax node.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to convert.
        /// </param>
        /// <returns>
        /// A type syntax node representing the <see cref="string"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TypeSyntax AsTypeSyntax(this string value) => SyntaxFactory.ParseTypeName(value);

        /// <summary>
        /// Converts a <see cref="string"/> to an XML text syntax node.
        /// </summary>
        /// <param name="value">
        /// The text to convert.
        /// </param>
        /// <returns>
        /// An XML text syntax node.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static XmlTextSyntax AsXmlText(this string value) => SyntaxFactory.XmlText(value);

        /// <summary>
        /// Concatenates a collection of <see cref="string"/>s into a single <see cref="string"/>.
        /// </summary>
        /// <param name="values">
        /// The collection of strings to concatenate.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains all non-null values.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith(this IEnumerable<string> values) => string.Concat(values.WhereNotNull());

        /// <summary>
        /// Joins a collection of strings with the specified separator.
        /// </summary>
        /// <param name="values">
        /// The collection of strings to join.
        /// </param>
        /// <param name="separator">
        /// The separator to use between strings.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains all values separated by the specified separator.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith(this IEnumerable<string> values, string separator) => string.Join(separator, values);

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a character with a <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The character to place at the beginning.
        /// </param>
        /// <param name="arg0">
        /// The <see cref="string"/> to append.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the character followed by the <see cref="string"/>.
        /// </returns>
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

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a character with a span of characters.
        /// </summary>
        /// <param name="value">
        /// The character to place at the beginning.
        /// </param>
        /// <param name="arg0">
        /// The span of characters to append.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the character followed by the characters in the span.
        /// </returns>
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

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a <see cref="string"/> with a span of characters.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to place at the beginning.
        /// </param>
        /// <param name="span">
        /// The span of characters to append.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the <see cref="string"/> followed by the characters in the span.
        /// </returns>
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

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a <see cref="string"/> with a character.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to place at the beginning.
        /// </param>
        /// <param name="arg0">
        /// The character to append.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the <see cref="string"/> followed by the character.
        /// </returns>
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

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a span of characters with a character.
        /// </summary>
        /// <param name="value">
        /// The span of characters to place at the beginning.
        /// </param>
        /// <param name="arg0">
        /// The character to append.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the characters in the span followed by the character.
        /// </returns>
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

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a span of characters with a <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The span of characters to place at the beginning.
        /// </param>
        /// <param name="arg0">
        /// The <see cref="string"/> to append.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the characters in the span followed by the <see cref="string"/>.
        /// </returns>
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

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a character, a <see cref="string"/>, and another character.
        /// </summary>
        /// <param name="value">
        /// The character to place at the beginning.
        /// </param>
        /// <param name="arg0">
        /// The <see cref="string"/> to append after the first character.
        /// </param>
        /// <param name="arg1">
        /// The character to append at the end.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the first character, followed by the <see cref="string"/>, followed by the second character.
        /// </returns>
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

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a character, a span of characters, and another character.
        /// </summary>
        /// <param name="value">
        /// The character to place at the beginning.
        /// </param>
        /// <param name="arg0">
        /// The span of characters to append after the first character.
        /// </param>
        /// <param name="arg1">
        /// The character to append at the end.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the first character, followed by the span of characters, followed by the second character.
        /// </returns>
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

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a character, a <see cref="string"/>, and a span of characters.
        /// </summary>
        /// <param name="value">
        /// The character to place at the beginning.
        /// </param>
        /// <param name="arg0">
        /// The <see cref="string"/> to append after the character.
        /// </param>
        /// <param name="arg1">
        /// The span of characters to append at the end.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the character, followed by the <see cref="string"/>, followed by the span of characters.
        /// </returns>
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

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a span of characters, a character, and a <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The span of characters to place at the beginning.
        /// </param>
        /// <param name="arg0">
        /// The character to append after the span.
        /// </param>
        /// <param name="arg1">
        /// The <see cref="string"/> to append at the end.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the span of characters, followed by the character, followed by the <see cref="string"/>.
        /// </returns>
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

                buffer[valueLength] = arg0;

                value.CopyTo(bufferSpan);
                arg1.AsSpan().CopyTo(bufferSpan.Slice(valueLength + 1));

                return new string(buffer, 0, length);
            }
        }

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a span of characters with two <see cref="string"/>s.
        /// </summary>
        /// <param name="value">
        /// The span of characters to place at the beginning.
        /// </param>
        /// <param name="arg0">
        /// The first <see cref="string"/> to append.
        /// </param>
        /// <param name="arg1">
        /// The second <see cref="string"/> to append.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the span of characters, followed by the first <see cref="string"/>, followed by the second <see cref="string"/>.
        /// </returns>
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

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a span of characters, a <see cref="string"/>, and another span of characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to place at the beginning.
        /// </param>
        /// <param name="arg0">
        /// The <see cref="string"/> to append after the span.
        /// </param>
        /// <param name="arg1">
        /// The span of characters to append at the end.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the first span, followed by the <see cref="string"/>, followed by the second span.
        /// </returns>
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

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a <see cref="string"/>, another <see cref="string"/>, and a span of characters.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to place at the beginning.
        /// </param>
        /// <param name="arg0">
        /// The second <see cref="string"/> to append.
        /// </param>
        /// <param name="arg1">
        /// The span of characters to append at the end.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the first <see cref="string"/>, followed by the second <see cref="string"/>, followed by the span of characters.
        /// </returns>
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

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a <see cref="string"/>, a span of characters, and a character.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to place at the beginning.
        /// </param>
        /// <param name="arg0">
        /// The span of characters to append.
        /// </param>
        /// <param name="arg1">
        /// The character to append at the end.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the <see cref="string"/>, followed by the span of characters, followed by the character.
        /// </returns>
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

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a <see cref="string"/>, a span of characters, and another <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to place at the beginning.
        /// </param>
        /// <param name="arg0">
        /// The span of characters to append.
        /// </param>
        /// <param name="arg1">
        /// The second <see cref="string"/> to append at the end.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the first <see cref="string"/>, followed by the span of characters, followed by the second <see cref="string"/>.
        /// </returns>
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

        /// <summary>
        /// Creates a new <see cref="string"/> by concatenating a span of characters, a character, a <see cref="string"/>, and another character.
        /// </summary>
        /// <param name="value">
        /// The span of characters to place at the beginning.
        /// </param>
        /// <param name="arg0">
        /// The first character to append.
        /// </param>
        /// <param name="arg1">
        /// The <see cref="string"/> to append.
        /// </param>
        /// <param name="arg2">
        /// The second character to append at the end.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the span, the first character, the <see cref="string"/>, and the second character.
        /// </returns>
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

        /// <summary>
        /// Determines whether the <see cref="string"/> contains the specified character.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to search in.
        /// </param>
        /// <param name="c">
        /// The character to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character is found; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this string value, in char c) => value?.IndexOf(c) >= 0;

        /// <summary>
        /// Determines whether the span of characters contains the specified character.
        /// </summary>
        /// <param name="value">
        /// The span of characters to search in.
        /// </param>
        /// <param name="c">
        /// The character to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character is found; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this in ReadOnlySpan<char> value, in char c) => value.Length > 0 && value.IndexOf(c) >= 0;

        /// <summary>
        /// Determines whether the span of characters contains the specified sequence of characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to search in.
        /// </param>
        /// <param name="finding">
        /// The sequence of characters to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the sequence is found; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this in ReadOnlySpan<char> value, in ReadOnlySpan<char> finding) => value.IndexOf(finding) >= 0;

        /// <summary>
        /// Determines whether the span of characters contains the specified sequence of characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to search in.
        /// </param>
        /// <param name="finding">
        /// The sequence of characters to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the sequence is found; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this in ReadOnlySpan<char> value, in ReadOnlySpan<char> finding, in StringComparison comparison) => value.IndexOf(finding, comparison) >= 0;

        /// <summary>
        /// Determines whether the span of characters contains the specified substring.
        /// </summary>
        /// <param name="value">
        /// The span of characters to search in.
        /// </param>
        /// <param name="finding">
        /// The substring to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the substring is found; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this in ReadOnlySpan<char> value, string finding)
        {
            if (finding is null)
            {
                return false;
            }

            return value.Contains(finding.AsSpan());
        }

        /// <summary>
        /// Determines whether the span of characters contains the specified substring using the given <see cref="string"/> comparison.
        /// </summary>
        /// <param name="value">
        /// The span of characters to search in.
        /// </param>
        /// <param name="finding">
        /// The substring to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the substring is found; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this in ReadOnlySpan<char> value, string finding, in StringComparison comparison)
        {
            if (finding is null)
            {
                return false;
            }

            return value.Contains(finding.AsSpan(), comparison);
        }

        /// <summary>
        /// Determines whether the <see cref="string"/> contains the specified substring using the given <see cref="string"/> comparison.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to search in.
        /// </param>
        /// <param name="finding">
        /// The substring to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the substring is found; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool Contains(this string value, string finding, in StringComparison comparison) // TODO RKN: Use default value 'StringComparison.Ordinal'
        {
            if (finding.Length < value.Length)
            {
                if (comparison is StringComparison.Ordinal)
                {
                    return value.AsSpan().IndexOf(finding.AsSpan()) >= 0;
                }

                return value.IndexOf(finding, comparison) >= 0;
            }

            if (finding.Length == value.Length)
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

        /// <summary>
        /// Determines whether the span of characters contains the specified substring with validation of the following character.
        /// </summary>
        /// <param name="value">
        /// The span of characters to search in.
        /// </param>
        /// <param name="finding">
        /// The substring to seek.
        /// </param>
        /// <param name="nextCharValidationCallback">
        /// The validation callback for the character that follows the found <see cref="string"/>.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the substring is found and the following character passes validation; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this in ReadOnlySpan<char> value, string finding, Func<char, bool> nextCharValidationCallback, in StringComparison comparison = StringComparison.Ordinal) => value.Contains(finding.AsSpan(), nextCharValidationCallback, comparison);

        /// <summary>
        /// Determines whether the span of characters contains the specified span with validation of the following character.
        /// </summary>
        /// <param name="value">
        /// The span of characters to search in.
        /// </param>
        /// <param name="finding">
        /// The span of characters to seek.
        /// </param>
        /// <param name="nextCharValidationCallback">
        /// The validation callback for the character that follows the found span.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span is found and the following character passes validation; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool Contains(this in ReadOnlySpan<char> value, in ReadOnlySpan<char> finding, Func<char, bool> nextCharValidationCallback, in StringComparison comparison = StringComparison.Ordinal)
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

        /// <summary>
        /// Determines whether the <see cref="string"/> contains any of the specified characters.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to search in.
        /// </param>
        /// <param name="characters">
        /// The characters to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if any of the characters are found; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsAny(this string value, char[] characters) => value?.IndexOfAny(characters) >= 0;

        /// <summary>
        /// Determines whether the span of characters contains any of the specified characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to search in.
        /// </param>
        /// <param name="characters">
        /// The characters to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if any of the characters are found; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<char> characters) => value.Length > 0 && value.IndexOfAny(characters) >= 0;

        /// <summary>
        /// Determines whether the <see cref="string"/> contains any of the specified phrases.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to search in.
        /// </param>
        /// <param name="phrases">
        /// The phrases to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if any of the phrases are found; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool ContainsAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> phrases, in StringComparison comparison = StringComparison.Ordinal)
        {
            if (value.Length > 0)
            {
                var ordinalComparison = comparison is StringComparison.Ordinal;

                for (int index = 0, length = phrases.Length; index < length; index++)
                {
                    var phrase = phrases[index];
                    var phraseSpan = phrase.AsSpan();

                    if (QuickContainsSubstringProbe(value, phraseSpan, comparison))
                    {
                        if (ordinalComparison)
                        {
                            if (value.Contains(phraseSpan))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (value.Contains(phraseSpan, comparison)) // Perf: when compared other than with Ordinal comparison, a string gets created internally
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the <see cref="string"/> contains any of the specified phrases.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to search in.
        /// </param>
        /// <param name="phrases">
        /// The phrases to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if any of the phrases are found; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool ContainsAny(this string value, in ReadOnlySpan<string> phrases, in StringComparison comparison = StringComparison.Ordinal)
        {
            if (comparison is StringComparison.Ordinal)
            {
                return ContainsAny(value.AsSpan(), phrases);
            }

            var span = value.AsSpan();

            for (int index = 0, length = phrases.Length; index < length; index++)
            {
                var phrase = phrases[index];

                if (QuickContainsSubstringProbe(span, phrase.AsSpan(), comparison))
                {
                    if (value.Contains(phrase, comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the <see cref="string"/> contains XML markup.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> contains XML markup; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool ContainsXml(this string value) => value.Contains('<') && value.Contains("/>");

        /// <summary>
        /// Counts the number of leading whitespace characters in the <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <param name="start">
        /// The starting index to begin counting from.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// The number of consecutive whitespace characters at the beginning of the <see cref="string"/>.
        /// </returns>
        public static int CountLeadingWhitespaces(this string value, int start = 0)
        {
            var whitespaces = 0;

            for (var length = value.Length; start < length; start++)
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
        /// Counts the number of trailing whitespace characters in the <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <param name="start">
        /// The starting index to begin counting from.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// The number of consecutive whitespace characters at the end of the <see cref="string"/>.
        /// </returns>
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

        /// <summary>
        /// Determines whether the <see cref="string"/> ends with the specified character.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <param name="character">
        /// The character to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> ends with the character; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EndsWith(this string value, in char character) => value.HasCharacters() && value[value.Length - 1] == character;

        /// <summary>
        /// Determines whether the span of characters ends with the specified character.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <param name="character">
        /// The character to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span ends with the character; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EndsWith(this in ReadOnlySpan<char> value, in char character) => value.Length > 0 && value[value.Length - 1] == character;

        /// <summary>
        /// Determines whether the span of characters ends with the specified <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <param name="characters">
        /// The <see cref="string"/> to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span ends with the <see cref="string"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool EndsWith(this in ReadOnlySpan<char> value, string characters, in StringComparison comparison = StringComparison.Ordinal)
        {
            var span = characters.AsSpan();

            return span.Length > 0 && value.EndsWith(span, comparison);
        }

        /// <summary>
        /// Determines whether the <see cref="string"/> ends with any of the specified characters.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <param name="suffixCharacters">
        /// The characters to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> ends with any of the characters; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EndsWithAny(this string value, in ReadOnlySpan<char> suffixCharacters) => value.HasCharacters() && suffixCharacters.Contains(value[value.Length - 1]);

        /// <summary>
        /// Determines whether the span of characters ends with any of the specified characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <param name="suffixCharacters">
        /// The characters to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span ends with any of the characters; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool EndsWithAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<char> suffixCharacters)
        {
            if (value.Length > 0)
            {
                var lastChar = value[value.Length - 1];

                for (int index = 0, length = suffixCharacters.Length; index < length; index++)
                {
                    if (lastChar == suffixCharacters[index])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the <see cref="string"/> ends with any of the specified suffixes.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <param name="suffixes">
        /// The suffixes to check for.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> ends with any of the suffixes; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool EndsWithAny(this string value, in ReadOnlySpan<string> suffixes, in StringComparison comparison = StringComparison.Ordinal)
        {
            if (value.HasCharacters())
            {
                var valueLength = value.Length;

                for (int index = 0, length = suffixes.Length; index < length; index++)
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

        /// <summary>
        /// Determines whether the <see cref="string"/> ends with any of the specified suffixes.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <param name="suffixes">
        /// The enumerable of suffixes to check for.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> ends with any of the suffixes; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool EndsWithAny(this string value, IEnumerable<string> suffixes, in StringComparison comparison = StringComparison.Ordinal)
        {
            if (value.HasCharacters())
            {
                var valueLength = value.Length;

                foreach (var suffix in suffixes)
                {
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

        /// <summary>
        /// Determines whether the <see cref="string"/> ends with any of the specified suffixes.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <param name="suffixes">
        /// The list of suffixes to check for.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> ends with any of the suffixes; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool EndsWithAny(this string value, List<string> suffixes, in StringComparison comparison = StringComparison.Ordinal)
        {
            if (value.HasCharacters())
            {
                var valueLength = value.Length;

                for (int index = 0, count = suffixes.Count; index < count; index++)
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

        /// <summary>
        /// Determines whether the span of characters ends with any of the specified suffixes.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <param name="suffixes">
        /// The suffixes to check for.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span ends with any of the suffixes; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool EndsWithAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> suffixes, in StringComparison comparison = StringComparison.Ordinal)
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

        /// <summary>
        /// Determines whether the <see cref="string"/> ends with a common number (excluding bit numbers).
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> ends with a common number; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool EndsWithCommonNumber(this string value) => value.EndsWithNumber() && value.EndsWithAny(Constants.Markers.BitNumbers, StringComparison.OrdinalIgnoreCase) is false;

        /// <summary>
        /// Determines whether the span of characters ends with a common number (excluding bit numbers).
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span ends with a common number; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool EndsWithCommonNumber(this in ReadOnlySpan<char> value) => value.EndsWithNumber() && value.EndsWithAny(Constants.Markers.BitNumbers, StringComparison.OrdinalIgnoreCase) is false;

        /// <summary>
        /// Determines whether the <see cref="string"/> ends with a number.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> ends with a number; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool EndsWithNumber(this string value) => value.HasCharacters() && value[value.Length - 1].IsNumber();

        /// <summary>
        /// Determines whether the span of characters ends with a number.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span ends with a number; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool EndsWithNumber(this in ReadOnlySpan<char> value) => value.Length > 0 && value[value.Length - 1].IsNumber();

        /// <summary>
        /// Determines whether the <see cref="string"/> equals the specified span of characters using the specified comparison.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to compare.
        /// </param>
        /// <param name="other">
        /// The span of characters to compare with.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> equals the span; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this string value, in ReadOnlySpan<char> other, in StringComparison comparison = StringComparison.Ordinal) => value != null && value.AsSpan().Equals(other, comparison);

        /// <summary>
        /// Determines whether the span of characters equals the specified <see cref="string"/> using the specified comparison.
        /// </summary>
        /// <param name="value">
        /// The span of characters to compare.
        /// </param>
        /// <param name="other">
        /// The <see cref="string"/> to compare with.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span equals the <see cref="string"/>; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this in ReadOnlySpan<char> value, string other, in StringComparison comparison = StringComparison.Ordinal) => other != null && value.Equals(other.AsSpan(), comparison);

        /// <summary>
        /// Determines whether the <see cref="string"/> equals any of the specified phrases.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to compare.
        /// </param>
        /// <param name="phrases">
        /// The phrases to compare with.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> equals any of the phrases; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool EqualsAny(this string value, in ReadOnlySpan<string> phrases, in StringComparison comparison = StringComparison.Ordinal)
        {
            if (value.HasCharacters())
            {
                for (int index = 0, length = phrases.Length; index < length; index++)
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

        /// <summary>
        /// Determines whether the <see cref="string"/> equals any of the specified phrases.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to compare.
        /// </param>
        /// <param name="phrases">
        /// The enumerable of phrases to compare with.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> equals any of the phrases; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool EqualsAny(this string value, IEnumerable<string> phrases, in StringComparison comparison = StringComparison.Ordinal)
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

        /// <summary>
        /// Determines whether the span of characters equals any of the specified phrases.
        /// </summary>
        /// <param name="value">
        /// The span of characters to compare.
        /// </param>
        /// <param name="phrases">
        /// The phrases to compare with.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span equals any of the phrases; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool EqualsAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> phrases, in StringComparison comparison = StringComparison.Ordinal)
        {
            if (value.Length > 0)
            {
                for (int index = 0, length = phrases.Length; index < length; index++)
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

        /// <summary>
        /// Gets the first sentence from the <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to process.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the first sentence from the <see cref="string"/>.
        /// </returns>
        public static string FirstSentence(this string value)
        {
            if (value.IsNullOrEmpty())
            {
                return value;
            }

            var span = value.AsSpan();
            var word = FirstSentence(span);

            return word != span
                   ? word.ToString()
                   : value;
        }

        /// <summary>
        /// Gets the first sentence from the span of characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <returns>
        /// The first sentence from the span.
        /// </returns>
        public static ReadOnlySpan<char> FirstSentence(this in ReadOnlySpan<char> value)
        {
            var text = value.TrimStart();
            var textLength = text.Length;

            if (textLength > 0)
            {
                // start at index 1 to skip first upper case character (and avoid return of empty word)
                for (var index = 1; index < textLength; index++)
                {
                    var c = text[index];

                    if (c.IsUpperCase() || c.IsSentenceEnding())
                    {
                        return text.Slice(0, index);
                    }
                }
            }

            return text;
        }

        /// <summary>
        /// Gets the first word from the <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to process.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the first word from the <see cref="string"/>.
        /// </returns>
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

        /// <summary>
        /// Gets the first word from the span of characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <returns>
        /// The first word from the span.
        /// </returns>
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

        /// <summary>
        /// Gets the second word from the <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to process.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the second word from the <see cref="string"/>.
        /// </returns>
        public static string SecondWord(this string value) => SecondWord(value.AsSpan()).ToString();

        /// <summary>
        /// Gets the second word from the span of characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <returns>
        /// The second word from the span.
        /// </returns>
        public static ReadOnlySpan<char> SecondWord(this in ReadOnlySpan<char> value) => value.WithoutFirstWord().FirstWord();

        /// <summary>
        /// Gets the third word from the <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to process.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the third word from the <see cref="string"/>.
        /// </returns>
        public static string ThirdWord(this string value) => ThirdWord(value.AsSpan()).ToString();

        /// <summary>
        /// Gets the third word from the span of characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <returns>
        /// The third word from the span.
        /// </returns>
        public static ReadOnlySpan<char> ThirdWord(this in ReadOnlySpan<char> value) => value.WithoutFirstWord().WithoutFirstWord().FirstWord();

        /// <summary>
        /// Gets the last word from the <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to process.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the last word from the <see cref="string"/>.
        /// </returns>
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

        /// <summary>
        /// Gets the last word from the span of characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <returns>
        /// The last word from the span.
        /// </returns>
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

        /// <summary>
        /// Formats a <see cref="string"/> by replacing the format item with the symbol.
        /// </summary>
        /// <param name="format">
        /// The format <see cref="string"/>.
        /// </param>
        /// <param name="arg0">
        /// The symbol to format.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the formatted result.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, ISymbol arg0) => string.Format(format, arg0);

        /// <summary>
        /// Formats a <see cref="string"/> by replacing the format item with the <see cref="string"/>.
        /// </summary>
        /// <param name="format">
        /// The format <see cref="string"/>.
        /// </param>
        /// <param name="arg0">
        /// The <see cref="string"/> to format.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the formatted result.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, string arg0) => string.Format(format, arg0);

        /// <summary>
        /// Formats a <see cref="string"/> by replacing the format items with the <see cref="string"/>s.
        /// </summary>
        /// <param name="format">
        /// The format <see cref="string"/>.
        /// </param>
        /// <param name="arg0">
        /// The first <see cref="string"/> to format.
        /// </param>
        /// <param name="arg1">
        /// The second <see cref="string"/> to format.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the formatted result.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, string arg0, string arg1) => string.Format(format, arg0, arg1);

        /// <summary>
        /// Formats a <see cref="string"/> by replacing the format items with the <see cref="string"/>s.
        /// </summary>
        /// <param name="format">
        /// The format <see cref="string"/>.
        /// </param>
        /// <param name="arg0">
        /// The first <see cref="string"/> to format.
        /// </param>
        /// <param name="arg1">
        /// The second <see cref="string"/> to format.
        /// </param>
        /// <param name="arg2">
        /// The third <see cref="string"/> to format.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the formatted result.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, string arg0, string arg1, string arg2) => string.Format(format, arg0, arg1, arg2);

        /// <summary>
        /// Formats a <see cref="string"/> by replacing the format items with the <see cref="string"/>s.
        /// </summary>
        /// <param name="format">
        /// The format <see cref="string"/>.
        /// </param>
        /// <param name="arg0">
        /// The first <see cref="string"/> to format.
        /// </param>
        /// <param name="arg1">
        /// The second <see cref="string"/> to format.
        /// </param>
        /// <param name="arg2">
        /// The third <see cref="string"/> to format.
        /// </param>
        /// <param name="arg3">
        /// The fourth <see cref="string"/> to format.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the formatted result.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, string arg0, string arg1, string arg2, string arg3) => string.Format(format, arg0, arg1, arg2, arg3);

        /// <summary>
        /// Gets the name-only part of the <see cref="string"/>, handling generic type names.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to process.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name-only part of the <see cref="string"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetNameOnlyPart(this string value) => GetNameOnlyPart(value.AsSpan());

        /// <summary>
        /// Gets the name-only part of the span of characters, handling generic type names.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name-only part of the span.
        /// </returns>
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

        /// <summary>
        /// Gets the name-only part of the span of characters without generic type information.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name-only part of the span without generic type information.
        /// </returns>
        public static string GetNameOnlyPartWithoutGeneric(this in ReadOnlySpan<char> value)
        {
            var genericIndexStart = value.IndexOf('<');

            var name = genericIndexStart > 0
                       ? value.Slice(0, genericIndexStart)
                       : value;

            return name.GetPartAfterLastDot().ToString();
        }

        /// <summary>
        /// Gets all numbers contained in the <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to search for numbers.
        /// </param>
        /// <returns>
        /// An array of <see cref="string"/>s representing each number found in the <see cref="string"/>, or an empty array if no numbers are found.
        /// </returns>
        public static string[] GetNumbers(this string value)
        {
            var matches = NumberRegex.Matches(value);

            if (matches.Count is 0)
            {
                return Array.Empty<string>();
            }

            return matches.Cast<Match>().ToArray(_ => _.Value);
        }

        /// <summary>
        /// Gets the part of the span after the last dot.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <returns>
        /// The part of the span after the last dot.
        /// </returns>
        public static ReadOnlySpan<char> GetPartAfterLastDot(this in ReadOnlySpan<char> value) => value.Slice(value.LastIndexOf('.') + 1);

        /// <summary>
        /// Determines whether the <see cref="string"/> has a collection marker suffix.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> has a collection marker suffix; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasCollectionMarker(this string value) => value.EndsWithAny(Constants.Markers.Collections, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Determines whether the <see cref="string"/> has an entity marker.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> has an entity marker; otherwise, <see langword="false"/>.
        /// </returns>
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

        /// <summary>
        /// Determines whether the <see cref="string"/> has characters (is not <see langword="null"/> or empty).
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> has characters; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasCharacters(this string value)
        {
            // Perf: As this method is invoked several million times, we directly use 'string.IsNullOrEmpty(value)' here instead of our own extension method helper
            return string.IsNullOrEmpty(value) is false;
        }

        /// <summary>
        /// Determines whether the <see cref="string"/> has more uppercase letters than the specified limit.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <param name="limit">
        /// The limit to compare against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> has more uppercase letters than the limit; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasUpperCaseLettersAbove(this string value, in ushort limit) => value != null && HasUpperCaseLettersAbove(value.AsSpan(), limit);

        /// <summary>
        /// Determines whether the span of characters has more uppercase letters than the specified limit.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <param name="limit">
        /// The limit to compare against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span has more uppercase letters than the limit; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasUpperCaseLettersAbove(this in ReadOnlySpan<char> value, in ushort limit)
        {
            var count = 0;

            for (int index = 0, length = value.Length; index < length; index++)
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

        /// <summary>
        /// Creates a humanized list of the strings using the specified separator for the last item.
        /// </summary>
        /// <param name="values">
        /// The collection of strings to concatenate.
        /// </param>
        /// <param name="lastSeparator">
        /// The separator to use before the last item.
        /// The default is <c>"or"</c>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the humanized concatenation of the strings.
        /// </returns>
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

        /// <summary>
        /// Takes the first part of the span up to the maximum length and adds ellipsis if needed.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <param name="maximum">
        /// The maximum length to take.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the first part of the span with ellipsis if truncated.
        /// </returns>
        public static string HumanizedTakeFirst(this in ReadOnlySpan<char> value, in int maximum)
        {
            var minimumLength = Math.Min(maximum, value.Length);

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

        /// <summary>
        /// Gets the index of the first occurrence of the specified phrases in the span of characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to search in.
        /// </param>
        /// <param name="phrases">
        /// The phrases to search for.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// The index of the first occurrence of any phrase, or <c>-1</c> if none are found.
        /// </returns>
        public static int IndexOfAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> phrases, in StringComparison comparison = StringComparison.Ordinal)
        {
            if (value.Length > 0)
            {
                // performance optimization to avoid unnecessary 'ToString' calls on 'ReadOnlySpan' (see implementation inside MemoryExtensions)
                if (comparison is StringComparison.Ordinal)
                {
                    for (int i = 0, length = phrases.Length; i < length; i++)
                    {
                        var index = value.IndexOf(phrases[i].AsSpan()); // performs ordinal comparison

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

        /// <summary>
        /// Gets the index of the first occurrence of the specified phrases in the <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to search in.
        /// </param>
        /// <param name="phrases">
        /// The phrases to search for.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// The index of the first occurrence of any phrase, or <c>-1</c> if none are found.
        /// </returns>
        public static int IndexOfAny(this string value, in ReadOnlySpan<string> phrases, in StringComparison comparison = StringComparison.Ordinal)
        {
            if (value.HasCharacters())
            {
                if (comparison is StringComparison.Ordinal)
                {
                    return IndexOfAny(value.AsSpan(), phrases, comparison);
                }

                for (int i = 0, length = phrases.Length; i < length; i++)
                {
                    var index = value.IndexOf(phrases[i], comparison);

                    if (index > -1)
                    {
                        return index;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the index of the last occurrence of the specified phrases in the <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to search in.
        /// </param>
        /// <param name="phrases">
        /// The phrases to search for.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// The index of the last occurrence of any phrase, or <c>-1</c> if none are found.
        /// </returns>
        public static int LastIndexOfAny(this string value, in ReadOnlySpan<string> phrases, in StringComparison comparison = StringComparison.Ordinal)
        {
            if (value is null)
            {
                return -1;
            }

            if (value.Length > 0)
            {
                for (int i = 0, length = phrases.Length; i < length; i++)
                {
                    var index = value.LastIndexOf(phrases[i], comparison);

                    if (index > -1)
                    {
                        return index;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Determines whether the <see cref="string"/> is an acronym (contains only uppercase letters).
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> is an acronym; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAcronym(this string value) => value.HasCharacters() && value.None(_ => _.IsLowerCaseLetter());

        /// <summary>
        /// Determines whether the <see cref="string"/> is a hyperlink.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> is a hyperlink; otherwise, <see langword="false"/>.
        /// </returns>
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

        /// <summary>
        /// Determines whether the character is a letter.
        /// </summary>
        /// <param name="value">
        /// The character to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character is a letter; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLetter(this in char value) => char.IsLetter(value);

        /// <summary>
        /// Determines whether the character is lowercase.
        /// </summary>
        /// <param name="value">
        /// The character to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character is lowercase; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowerCase(this in char value) => char.IsLower(value);

        /// <summary>
        /// Determines whether the character is a lowercase letter.
        /// </summary>
        /// <param name="value">
        /// The character to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character is a lowercase letter; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowerCaseLetter(this in char value) => value.IsLetter() && value.IsLowerCase();

        /// <summary>
        /// Determines whether the <see cref="string"/> is <see langword="null"/> or empty.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> is <see langword="null"/> or empty; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        /// <summary>
        /// Determines whether the span of characters is empty.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span is empty; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this in ReadOnlySpan<char> value) => value.IsEmpty;

        /// <summary>
        /// Determines whether the <see cref="string"/> is <see langword="null"/>, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> is <see langword="null"/>, empty, or consists only of white-space characters; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        /// <summary>
        /// Determines whether the span of characters is empty or consists only of white-space characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span is empty or consists only of white-space characters; otherwise, <see langword="false"/>.
        /// </returns>
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

        /// <summary>
        /// Determines whether the character is a number.
        /// </summary>
        /// <param name="value">
        /// The character to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character is a number; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumber(this in char value) => char.IsDigit(value);

        /// <summary>
        /// Determines whether the span is a number.
        /// </summary>
        /// <param name="value">
        /// The span to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span is a number; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsNumber(this in ReadOnlySpan<char> value)
        {
            var lastPosition = value.Length - 1;

            if (lastPosition is -1)
            {
                return false;
            }

            var last = value[lastPosition];

            if (last.IsNumber())
            {
                switch (lastPosition)
                {
                    case 0: return true;
                    case 1:
                        // check first character to be a number or a +/- sign
                        switch (value[0])
                        {
                            case '-':
                            case '+':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                            case '0':
                                return true;
                        }

                        return false;

                    default:
                        return value.ToString().IsNumber();
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the <see cref="string"/> is a number.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> is a number; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsNumber(this string value)
        {
            switch (value.Length)
            {
                case 0: return false;
                case 1: return value[0].IsNumber();
                default: return OnlyNumberRegex.IsMatch(value);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="string"/> is in <c>PascalCasing</c> format.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> is in <c>PascalCasing</c> format; otherwise, <see langword="false"/>.
        /// </returns>
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

        /// <summary>
        /// Determines whether the character is a sentence ending punctuation mark (period, question mark, or exclamation mark).
        /// </summary>
        /// <param name="value">
        /// The character to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character is a sentence ending punctuation mark; otherwise, <see langword="false"/>.
        /// </returns>
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

        /// <summary>
        /// Determines whether the <see cref="string"/> consists of a single word (no whitespace).
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> consists of a single word; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSingleWord(this string value) => value != null && IsSingleWord(value.AsSpan());

        /// <summary>
        /// Determines whether the span of characters consists of a single word (no whitespace).
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span consists of a single word; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSingleWord(this in ReadOnlySpan<char> value) => value.HasWhitespaces() is false;

        /// <summary>
        /// Determines whether all characters in the <see cref="string"/> are uppercase.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if all characters in the <see cref="string"/> are uppercase; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAllUpperCase(this string value) => value.AsSpan().IsAllUpperCase();

        /// <summary>
        /// Determines whether all characters in the span are uppercase.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if all characters in the span are uppercase; otherwise, <see langword="false"/>.
        /// </returns>
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

        /// <summary>
        /// Determines whether the character is uppercase.
        /// </summary>
        /// <param name="value">
        /// The character to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character is uppercase; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUpperCase(this in char value)
        {
            if ((uint)(value - 'a') <= 'z' - 'a') // see 'IsAsciiLetterLower', inlined for performance reasons
            {
                return false;
            }

            if ((uint)(value - 'A') <= 'Z' - 'A') // see 'IsAsciiLetterUpper', inlined for performance reasons
            {
                return true;
            }

            if ((uint)(value - '@') <= '@')
            {
                return false;
            }

            return IsUpperCaseWithSwitch(value);
        }

        /// <summary>
        /// Determines whether the character is an uppercase letter.
        /// </summary>
        /// <param name="value">
        /// The character to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character is an uppercase letter; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUpperCaseLetter(this in char value) => value.IsUpperCase() && value.IsLetter();

        /// <summary>
        /// Determines whether the character is a white-space character.
        /// </summary>
        /// <param name="value">
        /// The character to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character is a white-space character; otherwise, <see langword="false"/>.
        /// </returns>
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

        /// <summary>
        /// Determines whether the <see cref="string"/> starts with the specified character.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <param name="character">
        /// The character to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> starts with the character; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this string value, in char character) => value.HasCharacters() && value[0] == character;

        /// <summary>
        /// Determines whether the <see cref="string"/> starts with the specified characters.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <param name="characters">
        /// The characters to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> starts with the characters; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this string value, in ReadOnlySpan<char> characters) => value.HasCharacters() && value.AsSpan().StartsWith(characters);

        /// <summary>
        /// Determines whether the span of characters starts with the specified character.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <param name="character">
        /// The character to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span starts with the character; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this in ReadOnlySpan<char> value, in char character) => value.Length > 0 && value[0] == character;

        /// <summary>
        /// Determines whether the span of characters starts with the specified <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <param name="characters">
        /// The <see cref="string"/> to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span starts with the <see cref="string"/>; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this in ReadOnlySpan<char> value, string characters) => characters.HasCharacters() && value.StartsWith(characters.AsSpan());

        /// <summary>
        /// Determines whether the <see cref="string"/> starts with the specified characters using the specified comparison.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <param name="characters">
        /// The characters to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> starts with the characters; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this string value, in ReadOnlySpan<char> characters, in StringComparison comparison)
        {
            // for the moment this does not need a different handling for 'StringComparison' as it is used too few times to have any benefit
            return value.HasCharacters() && value.AsSpan().StartsWith(characters, comparison);
        }

        /// <summary>
        /// Determines whether the span of characters starts with the specified <see cref="string"/> using the specified comparison.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <param name="characters">
        /// The <see cref="string"/> to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span starts with the <see cref="string"/>; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this in ReadOnlySpan<char> value, string characters, in StringComparison comparison)
        {
            var others = characters.AsSpan();

            // perform quick check
            return QuickStartSubstringProbe(value, others, comparison) && value.StartsWith(others, comparison);
        }

        /// <summary>
        /// Determines whether the <see cref="string"/> starts with any of the specified characters.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <param name="characters">
        /// The characters to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> starts with any of the characters; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWithAny(this string value, IEnumerable<char> characters) => value.HasCharacters() && characters.Contains(value[0]);

        /// <summary>
        /// Determines whether the span of characters starts with any of the specified characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <param name="characters">
        /// The characters to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span starts with any of the characters; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWithAny(this in ReadOnlySpan<char> value, IEnumerable<char> characters) => value.Length > 0 && characters.Contains(value[0]);

        /// <summary>
        /// Determines whether the <see cref="string"/> starts with any of the specified prefixes.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <param name="prefixes">
        /// The prefixes to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> starts with any of the prefixes; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool StartsWithAny(this string value, in ReadOnlySpan<string> prefixes, in StringComparison comparison = StringComparison.Ordinal)
        {
            if (value.HasCharacters())
            {
                var valueSpan = value.AsSpan();

                for (int index = 0, length = prefixes.Length; index < length; index++)
                {
                    var prefix = prefixes[index];

                    if (QuickStartSubstringProbe(valueSpan, prefix.AsSpan(), comparison))
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

        /// <summary>
        /// Determines whether the span of characters starts with any of the specified prefixes.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <param name="prefixes">
        /// The prefixes to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span starts with any of the prefixes; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool StartsWithAny(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> prefixes, in StringComparison comparison = StringComparison.Ordinal)
        {
            if (value.Length > 0)
            {
                for (int index = 0, length = prefixes.Length; index < length; index++)
                {
                    if (value.StartsWith(prefixes[index], comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the <see cref="string"/> starts with a number.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> starts with a number; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWithNumber(this string value) => value.HasCharacters() && value[0].IsNumber();

        /// <summary>
        /// Creates a new <see cref="string"/> surrounded by the specified character.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to surround.
        /// </param>
        /// <param name="surrounding">
        /// The character to place at the beginning and end.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the original value surrounded by the specified character.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWith(this string value, in char surrounding) => surrounding.ConcatenatedWith(value, surrounding);

        /// <summary>
        /// Creates a new <see cref="string"/> surrounded by apostrophes.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to surround.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the original value surrounded by apostrophes.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWithApostrophe(this string value) => value?.SurroundedWith('\'');

        /// <summary>
        /// Creates a new <see cref="string"/> surrounded by double quotes.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to surround.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the original value surrounded by double quotes.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWithDoubleQuote(this string value) => value?.SurroundedWith('\"');

#pragma warning disable CA1308
        /// <summary>
        /// Converts the <see cref="string"/> to lowercase using the invariant culture.
        /// </summary>
        /// <param name="source">
        /// The <see cref="string"/> to convert.
        /// </param>
        /// <returns>
        /// A new <see cref="string"/> converted to lowercase.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToLowerCase(this string source) => source?.ToLowerInvariant();
#pragma warning restore CA1308

        /// <summary>
        /// Converts the character to lowercase.
        /// </summary>
        /// <param name="source">
        /// The character to convert.
        /// </param>
        /// <returns>
        /// The lowercase equivalent of the character.
        /// </returns>
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

        /// <summary>
        /// Converts the character to uppercase.
        /// </summary>
        /// <param name="source">
        /// The character to convert.
        /// </param>
        /// <returns>
        /// The uppercase equivalent of the character.
        /// </returns>
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

        /// <summary>
        /// Converts the <see cref="string"/> to uppercase using the invariant culture.
        /// </summary>
        /// <param name="source">
        /// The <see cref="string"/> to convert.
        /// </param>
        /// <returns>
        /// A new <see cref="string"/> converted to uppercase.
        /// </returns>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        /// <summary>
        /// Creates a new <see cref="string"/> with the specified character removed.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to process.
        /// </param>
        /// <param name="character">
        /// The character to remove.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the original value with the specified character removed.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Without(this string value, in char character) => value.Without(character.ToString());

        /// <summary>
        /// Creates a new <see cref="string"/> with the specified string removed.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to process.
        /// </param>
        /// <param name="phrase">
        /// The substring to remove.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the original value with the specified substring removed.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Without(this string value, string phrase) => value.Replace(phrase, string.Empty);

        /// <summary>
        /// Creates a new <see cref="string"/> with all specified phrases removed.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to process.
        /// </param>
        /// <param name="phrases">
        /// The collection of phrases to remove from the <see cref="string"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the original value with all specified phrases removed.
        /// </returns>
        public static string Without(this string value, in ReadOnlySpan<string> phrases) => value.AsCachedBuilder().Without(phrases).Trimmed().ToStringAndRelease();

        /// <summary>
        /// Creates a new <see cref="string"/> by removing the first word.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to process.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the original value with the first word removed.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string WithoutFirstWord(this string value) => WithoutFirstWord(value.AsSpan()).ToString();

        /// <summary>
        /// Removes the first word from the span of characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <returns>
        /// The span of characters after the first word.
        /// </returns>
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

        /// <summary>
        /// Removes the specified first words from the <see cref="string"/> based on the provided list of words.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to process.
        /// </param>
        /// <param name="words">
        /// The words to remove from the beginning of the <see cref="string"/>.
        /// </param>
        /// <returns>
        /// A span of characters with the specified first words removed.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> WithoutFirstWords(this string value, in ReadOnlySpan<string> words) => WithoutFirstWords(value.AsSpan(), words);

        /// <summary>
        /// Removes the first set of specified words from the span of characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <param name="words">
        /// The words to remove.
        /// </param>
        /// <returns>
        /// The span of characters after removing the first set of specified words.
        /// </returns>
        public static ReadOnlySpan<char> WithoutFirstWords(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> words)
        {
            var text = value.TrimStart();

            for (int index = 0, length = words.Length; index < length; index++)
            {
                var word = words[index];

                if (word.Contains(' '))
                {
                    foreach (var partialWord in word.Split(' '))
                    {
                        if (text.FirstWord().Equals(partialWord, StringComparison.OrdinalIgnoreCase))
                        {
                            text = text.WithoutFirstWord().TrimStart();
                        }
                    }
                }
                else
                {
                    if (text.FirstWord().Equals(word, StringComparison.OrdinalIgnoreCase))
                    {
                        text = text.WithoutFirstWord().TrimStart();
                    }
                }
            }

            return text.TrimStart();
        }

        /// <summary>
        /// Removes the suffix consisting of numbers from the <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to process.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the original value without the number suffix.
        /// </returns>
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

        /// <summary>
        /// Removes the suffix consisting of numbers from the span of characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <returns>
        /// The span of characters without the number suffix.
        /// </returns>
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

        /// <summary>
        /// Removes the surrounding quotes from the <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to process.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the original value without the surrounding quotes.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string WithoutQuotes(this string value) => value.Without(@"""");

        /// <summary>
        /// Removes the specified suffix from the <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to process.
        /// </param>
        /// <param name="suffix">
        /// The suffix to remove.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the original value without the specified suffix.
        /// </returns>
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

        /// <summary>
        /// Removes the specified suffix from the span of characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <param name="suffix">
        /// The character suffix to remove.
        /// </param>
        /// <returns>
        /// The span of characters without the specified suffix.
        /// </returns>
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

        /// <summary>
        /// Removes the specified suffix from the span of characters, using the specified comparison.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <param name="suffix">
        /// The <see cref="string"/> suffix to remove.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// The span of characters without the specified suffix.
        /// </returns>
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

        /// <summary>
        /// Removes consecutive suffixes from the span of characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <param name="suffixes">
        /// The suffixes to remove.
        /// </param>
        /// <returns>
        /// The span of characters without the specified suffixes.
        /// </returns>
        public static ReadOnlySpan<char> WithoutSuffixes(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> suffixes)
        {
            // do it twice to remove consecutive suffixes
            return value.RemoveSuffixes(suffixes)
                        .RemoveSuffixes(suffixes);
        }

        /// <summary>
        /// Converts the <see cref="string"/> to a <see cref="WordsReadOnlySpanEnumerator"/> for word enumeration.
        /// </summary>
        /// <param name="value">
        /// The span of characters to enumerate words from.
        /// </param>
        /// <param name="boundary">
        /// One of the enumeration members that specifies the word boundary method to use.
        /// The default is <see cref="WordBoundary.UpperCaseCharacters"/>.
        /// </param>
        /// <returns>
        /// A <see cref="WordsReadOnlySpanEnumerator"/> for the span of characters.
        /// </returns>
        public static WordsReadOnlySpanEnumerator WordsAsSpan(this string value, in WordBoundary boundary = WordBoundary.UpperCaseCharacters)
        {
            return value.AsSpan().WordsAsSpan(boundary);
        }

        /// <summary>
        /// Converts the <see cref="string"/> to a <see cref="WordsReadOnlySpanEnumerator"/> for word enumeration.
        /// </summary>
        /// <param name="value">
        /// The span of characters to enumerate words from.
        /// </param>
        /// <param name="boundary">
        /// One of the enumeration members that specifies the word boundary method to use.
        /// The default is <see cref="WordBoundary.UpperCaseCharacters"/>.
        /// </param>
        /// <returns>
        /// A <see cref="WordsReadOnlySpanEnumerator"/> for the span of characters.
        /// </returns>
        public static WordsReadOnlySpanEnumerator WordsAsSpan(this in ReadOnlySpan<char> value, in WordBoundary boundary = WordBoundary.UpperCaseCharacters) => new WordsReadOnlySpanEnumerator(value, boundary);

        /// <summary>
        /// Removes consecutive suffixes from the given span of characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to process.
        /// </param>
        /// <param name="suffixes">
        /// The collection of suffixes to remove.
        /// </param>
        /// <returns>
        /// A span of characters with the specified suffixes removed.
        /// </returns>
        private static ReadOnlySpan<char> RemoveSuffixes(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> suffixes)
        {
            if (value.Length <= 0)
            {
                return ReadOnlySpan<char>.Empty;
            }

            var slice = value;

            for (int index = 0, suffixesLength = suffixes.Length; index < suffixesLength; index++)
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

        /// <summary>
        /// Determines whether the span of characters contains any whitespace characters.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <param name="start">
        /// The index to start checking from.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the span contains any whitespace characters; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool HasWhitespaces(this in ReadOnlySpan<char> value, int start = 0)
        {
            for (var valueLength = value.Length; start < valueLength; start++)
            {
                if (value[start].IsWhiteSpace())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a new <see cref="string"/> with the character at the specified index converted to uppercase.
        /// </summary>
        /// <param name="source">
        /// The span of characters to process.
        /// </param>
        /// <param name="index">
        /// The zero-based index of the character to convert to uppercase.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the original value with the character at the specified index converted to uppercase.
        /// </returns>
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

        /// <summary>
        /// Creates a new <see cref="string"/> with the character at the specified index converted to lowercase.
        /// </summary>
        /// <param name="source">
        /// The span of characters to process.
        /// </param>
        /// <param name="index">
        /// The zero-based index of the character to convert to lowercase.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the original value with the character at the specified index converted to lowercase.
        /// </returns>
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

        /// <summary>
        /// Performs a quick check to determine if one span might contain the other span.
        /// </summary>
        /// <param name="value">
        /// The span of characters to search in.
        /// </param>
        /// <param name="other">
        /// The span of characters to search for.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a more detailed search should be performed; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool QuickContainsSubstringProbe(in ReadOnlySpan<char> value, in ReadOnlySpan<char> other, in StringComparison comparison)
        {
            if (value.Length > other.Length)
            {
                // continue to check
                return true;
            }

            return QuickStartSubstringProbe(value, other, comparison);
        }

        /// <summary>
        /// Performs a quick check to determine if one span might start with the other span.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <param name="other">
        /// The span of characters to check for at the start.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison method to use.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a more detailed check should be performed; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool QuickStartSubstringProbe(in ReadOnlySpan<char> value, in ReadOnlySpan<char> other, in StringComparison comparison)
        {
            if (value.Length < other.Length)
            {
                // cannot match
                return false;
            }

            // both are at least of similar length, so perform a quick compare first
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

        /// <summary>
        /// Performs a quick ordinal comparison of spans to determine if one starts with the other.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <param name="other">
        /// The span of characters to check for at the start.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the first and last characters match; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool QuickSubstringProbeOrdinal(in ReadOnlySpan<char> value, in ReadOnlySpan<char> other)
        {
            var length = Math.Min(value.Length, other.Length);

            if (length < QuickSubstringProbeLengthThreshold)
            {
                return true;
            }

            return value[0] == other[0] && value[length - 1] == other[length - 1];
        }

        /// <summary>
        /// Performs a quick case-insensitive comparison of spans to determine if one might start with the other.
        /// </summary>
        /// <param name="value">
        /// The span of characters to check.
        /// </param>
        /// <param name="other">
        /// The span of characters to check for at the start.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a more detailed check should be performed; otherwise, <see langword="false"/>.
        /// </returns>
        private static unsafe bool QuickSubstringProbeOrdinalIgnoreCase(in ReadOnlySpan<char> value, in ReadOnlySpan<char> other)
        {
            var length = Math.Min(value.Length, other.Length);

            if (length < QuickSubstringProbeLengthThreshold)
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

        /// <summary>
        /// Compares two characters at a specific index, ignoring case differences.
        /// </summary>
        /// <param name="a">
        /// The pointer to the first span of characters.
        /// </param>
        /// <param name="b">
        /// The pointer to the second span of characters.
        /// </param>
        /// <param name="index">
        /// The index at which to compare the characters.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the characters differ; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool QuickDiff(in char* a, in char* b, in int index)
        {
            int charA = *(a + index);

            if ((uint)(charA - 'a') <= 'z' - 'a')
            {
                charA -= DifferenceBetweenUpperAndLowerCaseAscii;
            }

            int charB = *(b + index);

            if ((uint)(charB - 'a') <= 'z' - 'a') // see 'IsAsciiLetterLower', inlined for performance reasons
            {
                charB -= DifferenceBetweenUpperAndLowerCaseAscii;
            }

            return charA != charB;
        }

        /// <summary>
        /// Determines whether a character is uppercase using a custom switch-based approach.
        /// </summary>
        /// <param name="value">
        /// The character to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character is uppercase; otherwise, <see langword="false"/>.
        /// </returns>
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

        /// <summary>
        /// Finds all indices of a substring in a span using ordinal comparison.
        /// </summary>
        /// <param name="span">
        /// The span of characters to search in.
        /// </param>
        /// <param name="other">
        /// The span of characters to search for.
        /// </param>
        /// <returns>
        /// An array of integer indices where the substring was found, or an empty array if not found.
        /// </returns>
        private static int[] AllIndicesOrdinal(in ReadOnlySpan<char> span, in ReadOnlySpan<char> other)
        {
            int[] indices = null;

            for (int index = 0, otherLength = other.Length; ; index += otherLength)
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

        /// <summary>
        /// Finds all indices of a string in a <see cref="string"/> using case-insensitive comparison.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> to search in.
        /// </param>
        /// <param name="finding">
        /// The <see cref="string"/> to search for.
        /// </param>
        /// <returns>
        /// An array of integer indices where the substring was found, or an empty array if not found.
        /// </returns>
        private static int[] AllIndicesNonOrdinal(string value, string finding)
        {
            int[] indices = null;

            for (int index = 0, findingLength = finding.Length; ; index += findingLength)
            {
                index = value.IndexOf(finding, index, StringComparison.OrdinalIgnoreCase);

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

        /// <summary>
        /// Determines whether a character is a lowercase ASCII letter.
        /// </summary>
        /// <param name="value">
        /// The character to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character is a lowercase ASCII letter; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAsciiLetterLower(this in char value) => IsAsciiLetterLower((int)value); // notice that we need just one compare per char

        /// <summary>
        /// Determines whether a character code represents a lowercase ASCII letter.
        /// </summary>
        /// <param name="value">
        /// The character code to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character code represents a lowercase ASCII letter; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAsciiLetterLower(this in int value) => (uint)(value - 'a') <= 'z' - 'a'; // notice that we need just one compare per char

        /// <summary>
        /// Determines whether a character is an uppercase ASCII letter.
        /// </summary>
        /// <param name="value">
        /// The character to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character is an uppercase ASCII letter; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAsciiLetterUpper(this in char value) => IsAsciiLetterUpper((int)value); // notice that we need just one compare per char

        /// <summary>
        /// Determines whether a character code represents an uppercase ASCII letter.
        /// </summary>
        /// <param name="value">
        /// The character code to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character code represents an uppercase ASCII letter; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAsciiLetterUpper(this in int value) => (uint)(value - 'A') <= 'Z' - 'A'; // notice that we need just one compare per char
    }
}