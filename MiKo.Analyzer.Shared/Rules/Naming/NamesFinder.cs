using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public static class NamesFinder
    {
        internal static IEnumerable<string> FindPropertyNames(IFieldSymbol symbol, string unwantedSuffix, string invocation)
        {
            // find properties
            var propertyNames = symbol.ContainingType.GetProperties().ToHashSet(_ => _.Name);

            // there might be none available; in such case do not report anything
            if (propertyNames.None())
            {
                return Enumerable.Empty<string>();
            }

            var symbolName = symbol.Name.WithoutSuffix(unwantedSuffix);

            // analyze correct name (must match string literal or nameof)
            var registeredName = GetRegisteredName(symbol, invocation);

            if (registeredName.IsNullOrWhiteSpace())
            {
                if (propertyNames.Contains(symbolName))
                {
                    return Enumerable.Empty<string>();
                }
            }
            else
            {
                if (registeredName == symbolName)
                {
                    return Enumerable.Empty<string>();
                }

                propertyNames.Clear();
                propertyNames.Add(registeredName);
            }

            return propertyNames;
        }

        internal static string FindBetterTestName(string symbolName)
        {
            if (symbolName.Length < 3)
            {
                return symbolName;
            }

            var correctedSymbolName = new StringBuilder(symbolName).ReplaceWithCheck("MustBe", "Is")
                                                                   .ReplaceWithCheck("MustNotBe", "IsNot")
                                                                   .ReplaceWithCheck("ShallBe", "Is")
                                                                   .ReplaceWithCheck("ShallNotBe", "IsNot")
                                                                   .ReplaceWithCheck("ShouldBe", "Is")
                                                                   .ReplaceWithCheck("ShouldNotBe", "IsNot")
                                                                   .ReplaceWithCheck("ShouldFail", "Fails")
                                                                   .ReplaceWithCheck("ShouldReturn", "Returns")
                                                                   .ReplaceWithCheck("ShouldThrow", "Throws")
                                                                   .ReplaceWithCheck("ReturnNull", "ReturnsNull")
                                                                   .ReplaceWithCheck("ReturnTrue", "ReturnsTrue")
                                                                   .ReplaceWithCheck("ReturnFalse", "ReturnsFalse")
                                                                   .ReplaceWithCheck("NullReturned", "ReturnsNull")
                                                                   .ReplaceWithCheck("TrueReturned", "ReturnsTrue")
                                                                   .ReplaceWithCheck("FalseReturned", "ReturnsFalse")
                                                                   .ReplaceWithCheck("IsExceptional", "ThrowsException")
                                                                   .ReplaceWithCheck(nameof(ArgumentNullException) + "Thrown", "Throws" + nameof(ArgumentNullException))
                                                                   .ReplaceWithCheck(nameof(ArgumentException) + "Thrown", "Throws" + nameof(ArgumentException))
                                                                   .ReplaceWithCheck(nameof(ArgumentOutOfRangeException) + "Thrown", "Throws" + nameof(ArgumentOutOfRangeException))
                                                                   .ReplaceWithCheck(nameof(InvalidOperationException) + "Thrown", "Throws" + nameof(InvalidOperationException))
                                                                   .ReplaceWithCheck(nameof(ObjectDisposedException) + "Thrown", "Throws" + nameof(ObjectDisposedException))
                                                                   .ReplaceWithCheck(nameof(NotSupportedException) + "Thrown", "Throws" + nameof(NotSupportedException))
                                                                   .ReplaceWithCheck(nameof(NotImplementedException) + "Thrown", "Throws" + nameof(NotImplementedException))
                                                                   .ReplaceWithCheck(nameof(TaskCanceledException) + "Thrown", "Throws" + nameof(TaskCanceledException))
                                                                   .ReplaceWithCheck(nameof(OperationCanceledException) + "Thrown", "Throws" + nameof(OperationCanceledException))
                                                                   .ReplaceWithCheck(nameof(Exception) + "Thrown", "Throws" + nameof(Exception))
                                                                   .ToString();

            var multipleUpperCases = false;

            const int CharacterToStartWith = 1;

            var characters = new List<char>(correctedSymbolName);

            for (var index = CharacterToStartWith; index < characters.Count; index++)
            {
                var c = characters[index];

                if (c == Constants.Underscore)
                {
                    // keep the existing underline
                    continue;
                }

                if (c.IsUpperCase())
                {
                    if (index == CharacterToStartWith)
                    {
                        // multiple upper cases in a line at beginning of the name, so do not flip
                        multipleUpperCases = true;
                    }

                    if (multipleUpperCases)
                    {
                        // let's see if we start with an IXyz interface
                        if (characters[index - 1] == 'I')
                        {
                            // seems we are in an IXyz interface
                            multipleUpperCases = false;
                        }

                        continue;
                    }

                    // let's consider an upper-case 'A' as a special situation as that is a single word
                    var isSpecialCharA = c == 'A';

                    multipleUpperCases = isSpecialCharA is false;

                    var nextC = c.ToLowerCase();

                    var nextIndex = index + 1;

                    if ((nextIndex >= characters.Count || (nextIndex < characters.Count && characters[nextIndex].IsUpperCase())) && isSpecialCharA is false)
                    {
                        // multiple upper cases in a line, so do not flip
                        nextC = c;
                    }

                    if (characters[index - 1] == Constants.Underscore)
                    {
                        characters[index] = nextC;
                    }
                    else
                    {
                        // only add an underline if we not already have one
                        characters[index] = Constants.Underscore;
                        index++;
                        characters.Insert(index, nextC);
                    }
                }
                else
                {
                    if (multipleUpperCases && characters[index - 1].IsUpperCase())
                    {
                        // we are behind multiple upper cases in a line, so add an underline
                        characters[index++] = Constants.Underscore;

                        characters.Insert(index, c);
                    }

                    multipleUpperCases = false;
                }
            }

            // fix some corrections, such as for known exceptions
            var result = new StringBuilder(characters.Count).Append(characters.ToArray())
                                                            .ReplaceWithCheck("argument_null_exception", nameof(ArgumentNullException))
                                                            .ReplaceWithCheck("argument_exception", nameof(ArgumentException))
                                                            .ReplaceWithCheck("argument_out_of_range_exception", nameof(ArgumentOutOfRangeException))
                                                            .ReplaceWithCheck("invalid_operation_exception", nameof(InvalidOperationException))
                                                            .ReplaceWithCheck("object_disposed_exception", nameof(ObjectDisposedException))
                                                            .ReplaceWithCheck("not_supported_exception", nameof(NotSupportedException))
                                                            .ReplaceWithCheck("not_implemented_exception", nameof(NotImplementedException))
                                                            .ReplaceWithCheck("task_canceled_exception", nameof(TaskCanceledException))
                                                            .ReplaceWithCheck("operation_canceled_exception", nameof(OperationCanceledException))
                                                            .ReplaceWithCheck("_guid_empty", "_empty_guid")
                                                            .ReplaceWithCheck("_string_empty", "_empty_string")
                                                            .ReplaceWithCheck("_in_return_", "<1>")
                                                            .ReplaceWithCheck("_to_return_", "<2>")
                                                            .ReplaceWithCheck("_not_throw_", "<3>")
                                                            .ReplaceWithCheck("_return_", "_returns_")
                                                            .ReplaceWithCheck("_throw_", "_throws_")
                                                            .ReplaceWithCheck("<1>", "_in_return_")
                                                            .ReplaceWithCheck("<2>", "_to_return_")
                                                            .ReplaceWithCheck("<3>", "_not_throw_")
                                                            .ReplaceWithCheck("D_oes", "_does")
                                                            .ReplaceWithCheck("O_bject", "_object")
                                                            .ReplaceWithCheck("R_eference", "_reference")
                                                            .ReplaceWithCheck("T_ype", "_type")
                                                            .ToString();

            return result;
        }

        internal static string FindDescribingWord(char c, string defaultValue = null)
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
                case '_': return "UNDERLINE";
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