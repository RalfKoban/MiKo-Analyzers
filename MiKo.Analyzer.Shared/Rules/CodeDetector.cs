using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules
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

        public static bool IsCSharpKeyword(string value) => SyntaxFactory.ParseToken(value).IsKeyword();

        public static bool IsCommentedOutCodeLine(string line) => IsCommentedOutCodeLine(line, null);

        public static bool IsCommentedOutCodeLine(string line, SemanticModel semanticModel)
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
                if (semanticModel != null)
                {
                    var firstWord = line.FirstWord();

                    /*
var type = semanticModel.GetTypeInfo(node).Type;
var convertedType = semanticModel.GetTypeInfo(node).ConvertedType;
                    */
                    var type = semanticModel.Compilation.GetTypeByMetadataName(firstWord);
                    if (type != null)
                    {
                        return true;
                    }
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