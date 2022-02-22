using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public static class CodeDetector
    {
        private static readonly char[] CodeBlockMarkers =
            {
                '{',
                '}',
            };

        private static readonly char[] ArgumentBlockMarkers =
            {
                '(',
                ')',
                '[',
                ']',
            };

        private static readonly string[] CodeConditionMarkers =
            {
                "??",
                "?.",
                "if(",
                "if (",
                "switch(",
                "switch (",
                "else if(",
                "else if (",
            };

        private static readonly string[] Operators =
            {
                "==",
                "!=",
                ">=",
                "<=",
                ">",
                "<",
                "++",
                "--",
                "+=",
                "-=",
                "*=",
                "/=",
                "=>", // lambda
                "&&",
                "||",
            };

        private static readonly string[] CodeStartMarkers =
            {
                "var ",
                "int ",
                "bool ",
                "public ",
                "private ",
                "internal ",
                "protected ",
            };

        private static readonly string[] ReSharperMarkers =
            {
                "ReSharper disable ",
                "ReSharper restore ",
            };

        private static readonly string[] FrameMarkers =
            {
                "===",
                "---",
                "***",
            };

        private static readonly string[] LockStatements =
            {
                "lock (",
                "lock(",
            };

        private static readonly ConcurrentDictionary<string, string> KnownTypeNames = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> KnownAssemblyNames = new ConcurrentDictionary<string, string>();

        public static void RefreshKnownTypes(Compilation compilation)
        {
            var assemblySymbols = new List<IAssemblySymbol>
                                      {
                                          compilation.Assembly,
                                      };
            try
            {
                var assemblies = compilation.GetUsedAssemblyReferences()
                                            .Select(compilation.GetAssemblyOrModuleSymbol)
                                            .OfType<IAssemblySymbol>();
                assemblySymbols.AddRange(assemblies);
            }
            catch (InvalidOperationException)
            {
                // promise to not enqueue, may happen during multiple test runs
            }

            // to speed up the lookup, add known assemblies and their types only once
            foreach (var assemblySymbol in assemblySymbols)
            {
                var assemblyName = string.Intern(assemblySymbol.FullyQualifiedName());

                if (KnownAssemblyNames.TryAdd(assemblyName, assemblyName))
                {
                    foreach (var typeName in assemblySymbol.TypeNames)
                    {
                        if (typeName[0] == '<')
                        {
                            continue;
                        }

                        KnownTypeNames.TryAdd(typeName, typeName);
                    }
                }
            }
        }

        public static bool IsCommentedOutCodeLine(string line)
        {
            if (line.IsNullOrWhiteSpace())
            {
                return false;
            }

            if (line == "else")
            {
                return true;
            }

            if (line.ContainsAny(CodeBlockMarkers))
            {
                return true;
            }

            if (line.StartsWithAny(CodeStartMarkers, StringComparison.Ordinal))
            {
                return true;
            }

            if (line.ContainsAny(CodeConditionMarkers, StringComparison.Ordinal))
            {
                return true;
            }

            if (line.Contains("//"))
            {
                // comment in comment indicator
                if (line.Contains("://"))
                {
                    return false; // allow indicators such as http:// or ftp://
                }

                if (line.EndsWith("//", StringComparison.Ordinal))
                {
                    return false; // ignore all framed comments
                }

                if (line.ContainsAny(ReSharperMarkers))
                {
                    return false; // ignore '// ReSharper' comments
                }

                return true;
            }

            if (line.Contains("$\""))
            {
                return true; // found a string interpolation
            }

            if (line.Contains(" = new"))
            {
                return true; // found a construction or initialization
            }

            if (line.ContainsAny(FrameMarkers))
            {
                return false;
            }

            if (line.EndsWith(';') || line.Contains('='))
            {
                if (line.Contains('.'))
                {
                    return true;
                }

                if (line.ContainsAny(ArgumentBlockMarkers))
                {
                    return true;
                }

                if (line.ContainsAny(Operators))
                {
                    return true;
                }

                // attempt to find a type because it's likely commented out code if we find some
                var firstWord = line.FirstWord();
                if (KnownTypeNames.ContainsKey(firstWord))
                {
                    return true;
                }
            }

            if (line.Contains(':') && line.Contains("case "))
            {
                return true;
            }

            if (line.ContainsAny(LockStatements))
            {
                return true;
            }

            return false;
        }
    }
}