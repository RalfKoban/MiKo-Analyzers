﻿using System;

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

        public static bool IsCommentedOutCodeLine(in ReadOnlySpan<char> line) => IsCommentedOutCodeLine(line, null);

        public static bool IsCommentedOutCodeLine(string line, SemanticModel semanticModel) => line != null && IsCommentedOutCodeLine(line.AsSpan(), semanticModel);

        public static bool IsCommentedOutCodeLine(in ReadOnlySpan<char> line, SemanticModel semanticModel)
        {
            if (line.IsNullOrWhiteSpace())
            {
                return false;
            }

            if (line.Equals("do", StringComparison.Ordinal))
            {
                return true;
            }

            if (line.Equals("else", StringComparison.Ordinal))
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

            var lineString = line.ToString();

            if (lineString.ContainsAny(CodeConditionMarkers, StringComparison.Ordinal))
            {
                return true;
            }

            if (line.Contains(Constants.Comments.CommentExterior))
            {
                // comment in comment indicator
                if (line.Contains("://"))
                {
                    return false; // allow indicators such as http:// or ftp://
                }

                if (line.EndsWith(Constants.Comments.CommentExterior, StringComparison.Ordinal))
                {
                    return false; // ignore all framed comments
                }

                if (lineString.ContainsAny(Constants.Markers.ReSharper))
                {
                    return false; // ignore '// ReSharper' comments
                }

                if (line.Contains(Constants.Comments.XmlCommentExterior))
                {
                    return false; // allow triplets such as '///'
                }

                if (line.Contains("ncrunch:"))
                {
                    return false; // ignore '//ncrunch: ' markers
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

            if (lineString.ContainsAny(FrameMarkers))
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

                if (lineString.ContainsAny(Operators))
                {
                    return true;
                }

                // attempt to find a type because it's likely commented out code if we find some
                if (semanticModel != null)
                {
                    var firstWord = line.FirstWord().ToString();

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

            if (lineString.ContainsAny(LockStatements))
            {
                return true;
            }

            return false;
        }
    }
}