using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public static class NamesFinder
    {
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
                                                        .Remove(0, betterNamePrefix.Length)
                                                        .Insert(0, methodName)
                                                        .ToStringAndRelease();

                        return fixedBetterName;
                    }
                }
            }

            return betterName;
        }

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

        internal static IReadOnlyCollection<string> FindPropertyNames(IFieldSymbol symbol, string unwantedSuffix, string invocation)
        {
            // find properties
            var propertyNames = symbol.ContainingType.GetProperties().ToHashSet(_ => _.Name);

            // there might be none available; in such case do not report anything
            if (propertyNames.None())
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

        private static string FindBetterTestName(string symbolName)
        {
            if (symbolName.Length < 3)
            {
                return symbolName;
            }

            var result = symbolName.AsCachedBuilder()
                                   .ReplaceWithProbe("MustBe", "Is")
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
                                   .ReplaceWithProbe(nameof(ArgumentException) + "Thrown", "Throws" + nameof(ArgumentException))
                                   .ReplaceWithProbe(nameof(ArgumentNullException) + "Thrown", "Throws" + nameof(ArgumentNullException))
                                   .ReplaceWithProbe(nameof(ArgumentOutOfRangeException) + "Thrown", "Throws" + nameof(ArgumentOutOfRangeException))
                                   .ReplaceWithProbe(nameof(InvalidOperationException) + "Thrown", "Throws" + nameof(InvalidOperationException))
                                   .ReplaceWithProbe("JsonExceptionThrown", "ThrowsJsonException")
                                   .ReplaceWithProbe(nameof(KeyNotFoundException) + "Thrown", "Throws" + nameof(KeyNotFoundException))
                                   .ReplaceWithProbe(nameof(NotImplementedException) + "Thrown", "Throws" + nameof(NotImplementedException))
                                   .ReplaceWithProbe(nameof(NotSupportedException) + "Thrown", "Throws" + nameof(NotSupportedException))
                                   .ReplaceWithProbe(nameof(NullReferenceException) + "Thrown", "Throws" + nameof(NullReferenceException))
                                   .ReplaceWithProbe(nameof(ObjectDisposedException) + "Thrown", "Throws" + nameof(ObjectDisposedException))
                                   .ReplaceWithProbe(nameof(OperationCanceledException) + "Thrown", "Throws" + nameof(OperationCanceledException))
                                   .ReplaceWithProbe(nameof(TaskCanceledException) + "Thrown", "Throws" + nameof(TaskCanceledException))
                                   .ReplaceWithProbe("ValidationExceptionThrown", "ThrowsValidationException")
                                   .ReplaceWithProbe(nameof(UnauthorizedAccessException) + "Thrown", "Throws" + nameof(UnauthorizedAccessException))
                                   .ReplaceWithProbe(nameof(Exception) + "Thrown", "Throws" + nameof(Exception))
                                   .SeparateWords(Constants.Underscore)
                                   .ReplaceWithProbe("argument_exception", nameof(ArgumentException))
                                   .ReplaceWithProbe("argument_null_exception", nameof(ArgumentNullException)) // fix some corrections, such as for known exceptions
                                   .ReplaceWithProbe("argument_out_of_range_exception", nameof(ArgumentOutOfRangeException))
                                   .ReplaceWithProbe("invalid_operation_exception", nameof(InvalidOperationException))
                                   .ReplaceWithProbe("json_exception", "JsonException")
                                   .ReplaceWithProbe("key_not_found_exception", nameof(KeyNotFoundException))
                                   .ReplaceWithProbe("not_implemented_exception", nameof(NotImplementedException))
                                   .ReplaceWithProbe("not_supported_exception", nameof(NotSupportedException))
                                   .ReplaceWithProbe("null_reference_exception", nameof(NullReferenceException))
                                   .ReplaceWithProbe("object_disposed_exception", nameof(ObjectDisposedException))
                                   .ReplaceWithProbe("operation_canceled_exception", nameof(OperationCanceledException))
                                   .ReplaceWithProbe("task_canceled_exception", nameof(TaskCanceledException))
                                   .ReplaceWithProbe("unauthorized_access_exception", nameof(UnauthorizedAccessException))
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
                                   .ReplaceWithProbe("O_bject", "_object")
                                   .ReplaceWithProbe("R_eference", "_reference")
                                   .ReplaceWithProbe("T_ype", "_type")
                                   .ReplaceWithProbe("_is_is_", "_is_")
                                   .ReplaceWithProbe("_does_alter_", "_alters_")
                                   .ReplaceWithProbe("_remove_", "_removes_")
                                   .ReplaceWithProbe("_not_removes_", "_not_remove_")
                                   .ReplaceWithProbe("_reject_", "_rejects_")
                                   .ReplaceWithProbe("_not_rejects_", "_not_reject_")
                                   .ToStringAndRelease();

            return result;
        }

        private static string GetRegisteredName(IFieldSymbol symbol, string invocation)
        {
            var arguments = symbol.GetInvocationArgumentsFrom(invocation);

            if (arguments.Count > 0)
            {
                return arguments[0].Expression.GetName();
            }

            return null;
        }
    }
}