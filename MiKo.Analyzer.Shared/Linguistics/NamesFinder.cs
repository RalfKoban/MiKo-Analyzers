using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Linguistics
{
    /// <summary>
    /// Provides methods to find and improve names, particularly for test methods and properties.
    /// </summary>
    public static class NamesFinder
    {
        private const string If = "If";
        private const string IfIt = "If_It";
        private const string Is = "Is";
        private const string When = "When";
        private const string And = "And";
        private const string Returned = "Returned";
        private const string Returns = "Returns";
        private const string Throws = "Throws";
        private const string Throw = "Throw";
        private const string Threw = "Threw";
        private const string NoLongerThrows = "NoLongerThrows";
        private const string NoLongerThrow = "NoLongerThrow";
        private const string NotThrows = "NotThrows";
        private const string NotThrow = "NotThrow";
        private const string Given = "Given";
        private const string IsGiven = Is + Given;
        private const string Consumed = "Consumed";
        private const string Rejected = "Rejected";
        private const string Accepted = "Accepted";

        private static readonly string[] SpecialThrowPhrases =
                                                               {
                                                                   Throws,
                                                                   Throw,
                                                                   NotThrows,
                                                                   NotThrow,
                                                                   NoLongerThrows,
                                                                   NoLongerThrow,
                                                                   Threw,
                                                               };

        private static readonly string[] SpecialNotPhrases =
                                                             {
                                                                 "DoesNot",
                                                                 "WillNot",
                                                                 "Wont",
                                                             };

        private static readonly ConcurrentDictionary<string, string> TestNamesCache = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        /// <summary>
        /// Determines an improved test name based on the specified name and symbol.
        /// </summary>
        /// <param name="name">
        /// The original name to improve.
        /// </param>
        /// <param name="symbol">
        /// The symbol to analyze for context.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the improved test name that follows naming conventions.
        /// </returns>
        internal static string FindBetterTestName(string name, ISymbol symbol)
        {
            var betterName = FindBetterTestName(name);

            // let's see if the test name starts with any method/property/event/field name that shall be kept
            var symbolName = symbol.Name;

            var index = symbolName.IndexOf(Constants.Underscore);

            if (index > 0)
            {
                var methodName = symbolName.Substring(0, index);

                if (symbol.GetSyntax().DescendantNodes<IdentifierNameSyntax>().Any(_ => _.GetName() == methodName))
                {
                    var betterNamePrefix = FindBetterTestName(methodName);

                    if (betterName.StartsWith(betterNamePrefix, StringComparison.Ordinal))
                    {
                        var fixedBetterName = betterName.AsCachedBuilder()
                                                        .TrimStartBy(betterNamePrefix.Length)
                                                        .Insert(0, methodName)
                                                        .ToStringAndRelease();

                        return fixedBetterName;
                    }
                }
            }

            return betterName;
        }

        /// <summary>
        /// Determines an improved test name with reordered parts based on the specified symbol name and symbol.
        /// </summary>
        /// <param name="symbolName">
        /// The original symbol name to improve.
        /// </param>
        /// <param name="symbol">
        /// The symbol to analyze for context.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the improved test name with parts reordered according to naming conventions.
        /// </returns>
        internal static string FindBetterTestNameWithReorder(string symbolName, ISymbol symbol)
        {
            var name = symbolName.Replace("_Expect_", "_");

            var nameToImprove = TryGetInOrder(name, out var nameInOrder)
                                ? nameInOrder
                                : name;

            return FindBetterTestName(nameToImprove, symbol);
        }

        /// <summary>
        /// Determines a descriptive word representation for the specified character.
        /// </summary>
        /// <param name="c">
        /// The character to describe.
        /// </param>
        /// <param name="defaultValue">
        /// The value to return if no descriptive word is available for the character.
        /// The default is <see langword="null"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the descriptive word for the character, or the specified default value if the character has no known description.
        /// </returns>
        internal static string FindDescribingWord(in char c, string defaultValue = null)
        {
            switch (c)
            {
                case '[': return "OPENING_BRACKET";
                case ']': return "CLOSING_BRACKET";
                case '(': return "OPENING_PARENTHESIS";
                case ')': return "CLOSING_PARENTHESIS";
                case '}': return "OPENING_BRACE";
                case '{': return "CLOSING_BRACE";
                case '<': return "OPENING_CHEVRON";
                case '>': return "CLOSING_CHEVRON";
                case ' ': return "SPACE";
                case '.': return "DOT";
                case '?': return "QUESTION_MARK";
                case '!': return "EXCLAMATION_MARK";
                case ',': return "COMMA";
                case ':': return "COLON";
                case ';': return "SEMICOLON";
                case '/': return "SLASH";
                case '\\': return "BACKSLASH";
                case Constants.Underscore: return "UNDERLINE";
                case '+': return "PLUS";
                case '-': return "MINUS";
                case '*': return "ASTERIX";
                case '=': return "EQUALS";
                case '&': return "AMPERSAND";
                case '%': return "PERCENT";
                case '$': return "DOLLAR";
                case '€': return "EURO";
                case '§': return "PARAGRAPH";
                case '~': return "TILDE";
                case '#': return "HASH";
                case '@': return "AT";
                case '"': return "QUOTATION_MARK";
                case '\'': return "APOSTROPHE";

                default:
                    return defaultValue;
            }
        }

        /// <summary>
        /// Determines the property names that match the specified field symbol.
        /// </summary>
        /// <param name="symbol">
        /// The field symbol to analyze.
        /// </param>
        /// <param name="unwantedSuffix">
        /// The suffix to remove from the symbol name before comparison.
        /// </param>
        /// <param name="invocation">
        /// The invocation to extract the registered name from.
        /// </param>
        /// <returns>
        /// A collection of property names that match the field symbol, or an empty collection if no matching properties are found or if the field name already matches a property.
        /// </returns>
        internal static IReadOnlyCollection<string> FindPropertyNames(IFieldSymbol symbol, string unwantedSuffix, string invocation)
        {
            // find properties
            var propertyNames = symbol.ContainingType.GetProperties().ToHashSet(_ => _.Name);

            // there might be none available; in such case do not report anything
            if (propertyNames.Count is 0)
            {
                return Array.Empty<string>();
            }

            var symbolName = symbol.Name.WithoutSuffix(unwantedSuffix);

            // analyze correct name (must match string literal or nameof)
            var registeredName = GetRegisteredName(symbol, invocation);

            if (registeredName.IsNullOrWhiteSpace())
            {
                if (propertyNames.Contains(symbolName))
                {
                    return Array.Empty<string>();
                }
            }
            else
            {
                if (registeredName == symbolName)
                {
                    return Array.Empty<string>();
                }

                propertyNames.Clear();
                propertyNames.Add(registeredName);
            }

            return propertyNames;
        }

        /// <summary>
        /// Determines an improved test name based on the specified symbol name.
        /// </summary>
        /// <param name="symbolName">
        /// The original symbol name to improve.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the improved test name that follows naming conventions.
        /// </returns>
        private static string FindBetterTestName(string symbolName)
        {
            if (symbolName.Length < 3)
            {
                return symbolName;
            }

            return TestNamesCache.GetOrAdd(symbolName, FindBetterTestNameLocal);

            string FindBetterTestNameLocal(string name)
            {
                var sb = name.AsCachedBuilder();

                if (name.StartsWith("Test", StringComparison.Ordinal))
                {
                    sb.Without("TestIf")
                      .Without("TestThat")
                      .Without("TestWhether");
                }

                sb.ReplaceWithProbe("MustBe", "Is")
                  .ReplaceWithProbe("MustNotBe", "IsNot")
                  .ReplaceWithProbe("ShallBe", "Is")
                  .ReplaceWithProbe("ShallNotBe", "IsNot")
                  .ReplaceWithProbe("ShouldBe", "Is")
                  .ReplaceWithProbe("ShouldNotBe", "IsNot")
                  .ReplaceWithProbe("ShouldFail", "Fails")
                  .ReplaceWithProbe("ShouldReturn", "Returns")
                  .ReplaceWithProbe("ShouldThrow", "Throws")
                  .ReplaceWithProbe("ReturnNull", "ReturnsNull")
                  .ReplaceWithProbe("ReturnTrue", "ReturnsTrue")
                  .ReplaceWithProbe("ReturnFalse", "ReturnsFalse")
                  .ReplaceWithProbe("NullReturned", "ReturnsNull")
                  .ReplaceWithProbe("TrueReturned", "ReturnsTrue")
                  .ReplaceWithProbe("FalseReturned", "ReturnsFalse")
                  .ReplaceWithProbe("IsExceptional", "ThrowsException")
                  .ReplaceWithProbe("ingFine", "ingIsFine")
                  .ReplaceWithProbe(Threw, Throw)
                  .ReplaceWithProbe("NoLongerThrows", "ThrowsNo")
                  .ReplaceWithProbe("NoLongerThrow", "ThrowsNo")
                  .ReplaceWithProbe("DoesNotThrow", "_does_not_throw_")
                  .ReplaceWithProbe("NotThrows", "ThrowsNo")
                  .ReplaceWithProbe("NotThrow", "ThrowsNo")
                  .ReplaceWithProbe("NoThrows", "ThrowsNo")
                  .ReplaceWithProbe("NoThrow", "ThrowsNo")
                  .ReplaceWithProbe("NoError", "HasNoError")
                  .ReplaceWithProbe("Already", "IsAlready")
                  .ReplaceWithProbe("Keep", "Keeps")
                  .ReplaceWithProbe("Keepss", "Keeps") // fix typo
                  .ReplaceWithProbe("Wont", "Does_Not_")
                  .ReplaceWithProbe("HTTPClient", "HTTP_client")
                  .ReplaceWithProbe("HTTPResponse", "HTTP_response")
                  .ReplaceWithProbe("HTTPRequest", "HTTP_request")
                  .ReplaceWithProbe("ArgumentExceptionThrown", "ThrowsArgumentException")
                  .ReplaceWithProbe("ArgumentsExceptionThrown", "ThrowsArgumentException")
                  .ReplaceWithProbe("ArgumentNullExceptionThrown", "ThrowsArgumentNullException")
                  .ReplaceWithProbe("ArgumentsNullExceptionThrown", "ThrowsArgumentNullException")
                  .ReplaceWithProbe("ArgumentOutOfRangeExceptionThrown", "ThrowsArgumentOutOfRangeException")
                  .ReplaceWithProbe("ArgumentsOutOfRangeExceptionThrown", "ThrowsArgumentOutOfRangeException")
                  .ReplaceWithProbe("InvalidOperationExceptionThrown", "ThrowsInvalidOperationException")
                  .ReplaceWithProbe("JsonExceptionThrown", "ThrowsJsonException")
                  .ReplaceWithProbe("KeyNotFoundExceptionThrown", "ThrowsKeyNotFoundException")
                  .ReplaceWithProbe("NotImplementedExceptionThrown", "ThrowsNotImplementedException")
                  .ReplaceWithProbe("NotSupportedExceptionThrown", "ThrowsNotSupportedException")
                  .ReplaceWithProbe("NullReferenceExceptionThrown", "ThrowsNullReferenceException")
                  .ReplaceWithProbe("ObjectDisposedExceptionThrown", "ThrowsObjectDisposedException")
                  .ReplaceWithProbe("OperationCanceledExceptionThrown", "ThrowsOperationCanceledException")
                  .ReplaceWithProbe("TaskCanceledExceptionThrown", "ThrowsTaskCanceledException")
                  .ReplaceWithProbe("ValidationExceptionThrown", "ThrowsValidationException")
                  .ReplaceWithProbe("UnauthorizedAccessExceptionThrown", "ThrowsUnauthorizedAccessException")
                  .ReplaceWithProbe("ExceptionThrown", "ThrowsException")
                  .ReplaceWithProbe("ActiveMQ", "<0>")
                  .ReplaceWithProbe("URL", "<1>")
                  .ReplaceWithProbe("URI", "<2>")
                  .ReplaceWithProbe("IRI", "<3>");

                sb.SeparateWords(Constants.Underscore);

                sb.ReplaceWithProbe("<0>", "ActiveMQ")
                  .ReplaceWithProbe("<1>", "URL")
                  .ReplaceWithProbe("<2>", "URI")
                  .ReplaceWithProbe("<3>", "IRI")
                  .ReplaceWithProbe("<_0>", "_ActiveMQ")
                  .ReplaceWithProbe("<_1>", "_URL")
                  .ReplaceWithProbe("<_2>", "_URI")
                  .ReplaceWithProbe("<_3>", "_IRI")
                  .ReplaceWithProbe("argument_exception", "ArgumentException")
                  .ReplaceWithProbe("argument_null_exception", "ArgumentNullException") // fix some corrections, such as for known exceptions
                  .ReplaceWithProbe("argument_out_of_range_exception", "ArgumentOutOfRangeException")
                  .ReplaceWithProbe("invalid_operation_exception", "InvalidOperationException")
                  .ReplaceWithProbe("json_exception", "JsonException")
                  .ReplaceWithProbe("key_not_found_exception", "KeyNotFoundException")
                  .ReplaceWithProbe("not_implemented_exception", "NotImplementedException")
                  .ReplaceWithProbe("not_supported_exception", "NotSupportedException")
                  .ReplaceWithProbe("null_reference_exception", "NullReferenceException")
                  .ReplaceWithProbe("object_disposed_exception", "ObjectDisposedException")
                  .ReplaceWithProbe("operation_canceled_exception", "OperationCanceledException")
                  .ReplaceWithProbe("task_canceled_exception", "TaskCanceledException")
                  .ReplaceWithProbe("unauthorized_access_exception", "UnauthorizedAccessException")
                  .ReplaceWithProbe("validation_exception", "ValidationException")
                  .ReplaceWithProbe("_guid_empty", "_empty_guid")
                  .ReplaceWithProbe("_string_empty", "_empty_string")
                  .ReplaceWithProbe("_in_return_", "<1>")
                  .ReplaceWithProbe("_to_return_", "<2>")
                  .ReplaceWithProbe("_not_throw_", "<3>")
                  .ReplaceWithProbe("_return_", "_returns_")
                  .ReplaceWithProbe("_throw_", "_throws_")
                  .ReplaceWithProbe("<1>", "_in_return_")
                  .ReplaceWithProbe("<2>", "_to_return_")
                  .ReplaceWithProbe("<3>", "_not_throw_")
                  .ReplaceWithProbe("_not_throws_", "_throws_no_")
                  .ReplaceWithProbe("_no_throws_", "_throws_no_")
                  .ReplaceWithProbe("_has_has_", "_has_")
                  .ReplaceWithProbe("D_oes", "_does")
                  .ReplaceWithProbe("F_actory", "_factory")
                  .ReplaceWithProbe("M_akes", "_makes")
                  .ReplaceWithProbe("O_bject", "_object")
                  .ReplaceWithProbe("R_eference", "_reference")
                  .ReplaceWithProbe("T_ype", "_type")
                  .ReplaceWithProbe("_is_is_", "_is_")
                  .ReplaceWithProbe("_returned_from_", "_of_")
                  .ReplaceWithProbe("_returns_gets_if_", "_returns_")
                  .ReplaceWithProbe("_returns_got_if_", "_returns_")
                  .ReplaceWithProbe("_returns_had_if_", "_returns_")
                  .ReplaceWithProbe("_returns_has_if_", "_returns_")
                  .ReplaceWithProbe("_returns_is_if_", "_returns_")
                  .ReplaceWithProbe("_returns_was_if_", "_returns_")
                  .ReplaceWithProbe("_will_not_be", "_does_not_")
                  .ReplaceWithProbe("_will_not_", "_does_not_")
                  .ReplaceWithProbe("_wont_", "_does_not_")
                  .ReplaceWithProbe("_will_returns_if_", "_returns_")
                  .ReplaceWithProbe("_will_returns_", "_returns_")
                  .ReplaceWithProbe("_was_", "_is_")
                  .ReplaceWithProbe("_will_be_", "_is_")
                  .ReplaceWithProbe("_will_", "_does_")
                  .Replace("__", "_")
                  .AdjustWordAfter("_does_not_", FirstWordAdjustment.MakeInfinite)
                  .ReplaceWithProbe("_does_not_", "<4>")
                  .AdjustWordAfter("_does_", FirstWordAdjustment.MakeThirdPersonSingular)
                  .Replace("_does_", "_")
                  .ReplaceWithProbe("<4>", "_does_not_")
                  .AdjustWordAfter("_to_", FirstWordAdjustment.MakeInfinite);

                return sb.ToStringAndRelease();
            }
        }

        /// <summary>
        /// Determines the registered name from the invocation arguments of the specified field symbol.
        /// </summary>
        /// <param name="symbol">
        /// The field symbol to analyze.
        /// </param>
        /// <param name="invocation">
        /// The invocation to extract the registered name from.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name extracted from the first invocation argument, or <see langword="null"/> if no arguments are available.
        /// </returns>
        private static string GetRegisteredName(IFieldSymbol symbol, string invocation)
        {
            var arguments = symbol.GetInvocationArgumentsFrom(invocation);

            if (arguments.Count > 0)
            {
                return arguments[0].Expression.GetName();
            }

            return null;
        }

        /// <summary>
        /// Attempts to reorder the parts of the specified name according to naming conventions.
        /// </summary>
        /// <param name="name">
        /// The name to reorder.
        /// </param>
        /// <param name="result">
        /// On successful return, contains the reordered name; otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the name parts were reordered; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool TryGetInOrder(string name, out string result)
        {
            var parts = name.Split(Constants.Underscores, StringSplitOptions.RemoveEmptyEntries);

            switch (parts.Length)
            {
                case 2: return TryGetInOrder(parts[0], parts[1], out result);
                case 3: return TryGetInOrder(parts[0], parts[1], parts[2], out result);
                case 4: return TryGetInOrder(parts[0], parts[1], parts[2], parts[3], out result);

                default:
                {
                    result = null;

                    return false;
                }
            }
        }

        /// <summary>
        /// Attempts to reorder two parts of a test name according to naming conventions.
        /// </summary>
        /// <param name="part0">
        /// The first part of the name.
        /// </param>
        /// <param name="part1">
        /// The second part of the name.
        /// </param>
        /// <param name="result">
        /// On successful return, contains the reordered name; otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the name parts were reordered; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool TryGetInOrder(string part0, string part1, out string result)
        {
            result = null;

            if (part1.StartsWithAny(SpecialNotPhrases))
            {
                // it seems like this is in normal order, so do not change the order
                return false;
            }

            if (part1.StartsWithAny(SpecialThrowPhrases, StringComparison.OrdinalIgnoreCase) && part1.Contains(If, StringComparison.Ordinal))
            {
                // it seems like this is in normal order, so do not change the order
                return false;
            }

            var addIf = IsIfRequired(part0, part1);
            var ifToAdd = Verbalizer.IsThirdPersonSingularVerb(part0.AsSpan().FirstWord()) ? IfIt : If;

            var capacity = part0.Length + part1.Length;

            if (addIf)
            {
                capacity += ifToAdd.Length;
            }

            var builder = StringBuilderCache.Acquire(capacity);
            FixPart(builder, part1);

            if (addIf)
            {
                builder.Append(ifToAdd);
            }

            FixPart(builder, part0);

            builder.ReplaceWithProbe(When, If).ReplaceWithProbe(If + If, If);

            result = StringBuilderCache.GetStringAndRelease(builder);

            return true;
        }

        /// <summary>
        /// Attempts to reorder three parts of a test name according to naming conventions.
        /// </summary>
        /// <param name="part0">
        /// The first part of the name.
        /// </param>
        /// <param name="part1">
        /// The second part of the name.
        /// </param>
        /// <param name="part2">
        /// The third part of the name.
        /// </param>
        /// <param name="result">
        /// On successful return, contains the reordered name; otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the name parts were reordered; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool TryGetInOrder(string part0, string part1, string part2, out string result)
        {
            if (part1.StartsWithAny(SpecialThrowPhrases, StringComparison.OrdinalIgnoreCase))
            {
                // it seems like this is in normal order, so do not change the order
                result = null;

                return false;
            }

            var adjustedPart2 = part2.AdjustFirstWord(FirstWordAdjustment.MakeThirdPersonSingular);

            var addIf = IsIfRequired(part1, adjustedPart2);
            var ifToAdd = Verbalizer.IsThirdPersonSingularVerb(part1.AsSpan().FirstWord()) ? IfIt : If;

            var capacity = part0.Length + part1.Length + adjustedPart2.Length;

            if (addIf)
            {
                capacity += ifToAdd.Length;
            }

            var builder = StringBuilderCache.Acquire(capacity);
            FixPart(builder, part0);
            FixPart(builder, adjustedPart2);

            if (addIf)
            {
                builder.Append(ifToAdd);
            }

            if (part1.StartsWith(Given, StringComparison.OrdinalIgnoreCase))
            {
                part1 = part1.AsSpan(Given.Length).ConcatenatedWith(IsGiven);
            }

            FixPart(builder, part1);

            builder.ReplaceWithProbe(When, If).ReplaceWithProbe(If + If, If);

            result = StringBuilderCache.GetStringAndRelease(builder);

            return true;
        }

        /// <summary>
        /// Attempts to reorder four parts of a test name according to naming conventions.
        /// </summary>
        /// <param name="part0">
        /// The first part of the name.
        /// </param>
        /// <param name="part1">
        /// The second part of the name.
        /// </param>
        /// <param name="part2">
        /// The third part of the name.
        /// </param>
        /// <param name="part3">
        /// The fourth part of the name.
        /// </param>
        /// <param name="result">
        /// On successful return, contains the reordered name; otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the name parts were reordered; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool TryGetInOrder(string part0, string part1, string part2, string part3, out string result)
        {
            if (part2.StartsWith(And, StringComparison.Ordinal))
            {
                // it seems like this is in normal order and a combination of 2 scenarios, so do not change the order
                result = null;

                return false;
            }

            if (part1.StartsWithAny(SpecialThrowPhrases, StringComparison.OrdinalIgnoreCase))
            {
                // it seems like this is in normal order, so do not change the order
                result = null;

                return false;
            }

            var useWhen = part1.StartsWith(When, StringComparison.Ordinal);

            var addIf = useWhen is false && IsIfRequired(part2, part3);
            var ifToAdd = Verbalizer.IsThirdPersonSingularVerb((useWhen ? part1 : part2).AsSpan().FirstWord()) ? IfIt : If;

            var capacity = part0.Length + part1.Length + part2.Length + part3.Length + And.Length;

            if (addIf)
            {
                capacity += ifToAdd.Length;
            }

            var builder = StringBuilderCache.Acquire(capacity);
            FixPart(builder, part0);
            FixPart(builder, part3);

            if (addIf)
            {
                builder.Append(ifToAdd);
            }

            if (useWhen)
            {
                FixPart(builder, part1);
                FixPart(builder, part2);
            }
            else
            {
                FixPart(builder, part2);
                builder.Append(And);
                FixPart(builder, part1);
            }

            builder.ReplaceWithProbe(When, If).ReplaceWithProbe(If + If, If);

            result = StringBuilderCache.GetStringAndRelease(builder);

            return true;
        }

        /// <summary>
        /// Determines whether an "If" connector is required between two name parts.
        /// </summary>
        /// <param name="part1">
        /// The first part of the name to evaluate.
        /// </param>
        /// <param name="part2">
        /// The second part of the name to evaluate.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if an "If" connector is required; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool IsIfRequired(string part1, string part2)
        {
            if (part1.StartsWith(If, StringComparison.OrdinalIgnoreCase)
             || part1.StartsWith(Consumed, StringComparison.OrdinalIgnoreCase)
             || part2.Equals(Returned, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Appends a modified version of a name part to the <see cref="StringBuilder"/>  according to naming conventions.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="StringBuilder"/>  to append to.
        /// </param>
        /// <param name="original">
        /// The original name part to process and append.
        /// </param>
        private static void FixPart(StringBuilder builder, string original)
        {
            if (original.EndsWith(Returned, StringComparison.OrdinalIgnoreCase))
            {
                builder.Append(Returns).Append(original, 0, original.Length - Returned.Length);
            }
            else if (original.StartsWith(Rejected, StringComparison.OrdinalIgnoreCase))
            {
                builder.Append(original, Rejected.Length, original.Length - Rejected.Length).Append(Is).Append(Rejected);
            }
            else if (original.StartsWith(Accepted, StringComparison.OrdinalIgnoreCase))
            {
                builder.Append(original, Accepted.Length, original.Length - Accepted.Length).Append(Is).Append(Accepted);
            }
            else
            {
                builder.Append(original);
            }
        }
    }
}