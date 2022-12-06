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

            var correctedSymbolName = new StringBuilder(symbolName).Replace("MustBe", "Is")
                                                                   .Replace("MustNotBe", "IsNot")
                                                                   .Replace("ShallBe", "Is")
                                                                   .Replace("ShallNotBe", "IsNot")
                                                                   .Replace("ShouldBe", "Is")
                                                                   .Replace("ShouldNotBe", "IsNot")
                                                                   .Replace("ShouldFail", "Fails")
                                                                   .Replace("ShouldReturn", "Returns")
                                                                   .Replace("ShouldThrow", "Throws")
                                                                   .Replace("IsExceptional", "ThrowsException")
                                                                   .ToString();

            var multipleUpperCases = false;

            const int CharacterToStartWith = 1;

            var characters = new List<char>(correctedSymbolName);
            for (var index = CharacterToStartWith; index < characters.Count; index++)
            {
                var c = characters[index];

                if (c == '_')
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
                    if (nextIndex >= characters.Count || (nextIndex < characters.Count && characters[nextIndex].IsUpperCase()) && isSpecialCharA is false)
                    {
                        // multiple upper cases in a line, so do not flip
                        nextC = c;
                    }

                    if (characters[index - 1] == '_')
                    {
                        characters[index] = nextC;
                    }
                    else
                    {
                        // only add an underline if we not already have one
                        characters[index] = '_';
                        index++;
                        characters.Insert(index, nextC);
                    }
                }
                else
                {
                    if (multipleUpperCases && characters[index - 1].IsUpperCase())
                    {
                        // we are behind multiple upper cases in a line, so add an underline
                        characters[index++] = '_';
                        characters.Insert(index, c);
                    }

                    multipleUpperCases = false;
                }
            }

            // fix some corrections, such as for known exceptions
            var result = new StringBuilder().Append(characters.ToArray())
                                            .Replace("argument_null_exception", nameof(ArgumentNullException))
                                            .Replace("argument_exception", nameof(ArgumentException))
                                            .Replace("argument_out_of_range_exception", nameof(ArgumentOutOfRangeException))
                                            .Replace("invalid_operation_exception", nameof(InvalidOperationException))
                                            .Replace("object_disposed_exception", nameof(ObjectDisposedException))
                                            .Replace("not_supported_exception", nameof(NotSupportedException))
                                            .Replace("not_implemented_exception", nameof(NotImplementedException))
                                            .Replace("task_canceled_exception", nameof(TaskCanceledException))
                                            .Replace("operation_canceled_exception", nameof(OperationCanceledException))
                                            .Replace("_return_", "_returns_")
                                            .ToString();

            return string.Intern(result);
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