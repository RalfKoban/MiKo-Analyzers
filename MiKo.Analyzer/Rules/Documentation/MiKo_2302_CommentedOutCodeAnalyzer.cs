using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2302_CommentedOutCodeAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2302";

        private static readonly string[] CodeBlockMarkers =
            {
                "{",
                "}",
            };

        private static readonly string[] ArgumentBlockMarkers =
            {
                "(",
                ")",
                "[",
                "]",
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

        public MiKo_2302_CommentedOutCodeAnalyzer() : base(Id)
        {
        }

        protected override void PrepareAnalyzeMethod(Compilation compilation)
        {
            IEnumerable<IAssemblySymbol> assemblySymbols;
            try
            {
                assemblySymbols = compilation.GetUsedAssemblyReferences()
                                             .Select(compilation.GetAssemblyOrModuleSymbol)
                                             .OfType<IAssemblySymbol>()
                                             .ToList();
            }
            catch (InvalidOperationException)
            {
                // promise to not enqueue, may happen during multiple test runs
                assemblySymbols = Enumerable.Empty<IAssemblySymbol>();
            }

            // to speed up the lookup, add known assemblies and their types only once
            foreach (var assemblySymbol in assemblySymbols)
            {
                var assemblyName = string.Intern(assemblySymbol.FullyQualifiedName());

                if (KnownAssemblyNames.TryAdd(assemblyName, assemblyName))
                {
                    foreach (var typeName in assemblySymbol.TypeNames)
                    {
                        KnownTypeNames.TryAdd(typeName, typeName);
                    }
                }
            }
        }

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel)
        {
            if (comment == "else")
            {
                return true;
            }

            if (comment.ContainsAny(CodeBlockMarkers))
            {
                return true;
            }

            if (comment.ContainsAny(ReSharperMarkers))
            {
                return false; // ignore '// ReSharper' comments
            }

            if (comment.StartsWithAny(CodeStartMarkers, StringComparison.Ordinal))
            {
                return true;
            }

            if (comment.ContainsAny(CodeConditionMarkers, StringComparison.Ordinal))
            {
                return true;
            }

            if (comment.Contains("//"))
            {
                // comment in comment indicator
                if (comment.Contains("://"))
                {
                    return false; // allow indicators such as http:// or ftp://
                }

                if (comment.EndsWith("//", StringComparison.Ordinal))
                {
                    return false; // ignore all framed comments
                }

                return true;
            }

            if (comment.Contains("$\""))
            {
                return true; // found a string interpolation
            }

            if (comment.Contains(" = new"))
            {
                return true; // found a construction or initialization
            }

            if (comment.ContainsAny(FrameMarkers))
            {
                return false;
            }

            if (comment.EndsWith(";", StringComparison.Ordinal) || comment.Contains("="))
            {
                if (comment.Contains("."))
                {
                    return true;
                }

                if (comment.ContainsAny(ArgumentBlockMarkers, StringComparison.Ordinal))
                {
                    return true;
                }

                if (comment.ContainsAny(Operators))
                {
                    return true;
                }

                // attempt to find a type because it's likely commented out code if we find some
                var firstWord = comment.FirstWord();
                if (KnownTypeNames.ContainsKey(firstWord))
                {
                    return true;
                }
            }

            if (comment.Contains("case ") && comment.Contains(":"))
            {
                return true;
            }

            if (comment.ContainsAny(LockStatements))
            {
                return true;
            }

            return false;
        }
    }
}