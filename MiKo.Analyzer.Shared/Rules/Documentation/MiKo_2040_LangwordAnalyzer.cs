﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2040_LangwordAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2040";

        internal static readonly string[] Phrases = { "true", "false", "null" };

        internal static readonly HashSet<string> WrongAttributes = new HashSet<string>
                                                                       {
                                                                           "langref",
                                                                           "langowrd", // find typos
                                                                           "langwrod", // find typos
                                                                           "langwowd", // find typos
                                                                       };

        private static readonly KeyValuePair<string, string>[] StartParts = CreateStartParts();

        private static readonly KeyValuePair<string, string>[] MiddleParts = CreateMiddleParts();

        private static readonly KeyValuePair<string, string>[] EndParts = CreateEndParts();

        public MiKo_2040_LangwordAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            return comment is null
                       ? Enumerable.Empty<Diagnostic>()
                       : AnalyzeComment(symbol.Name, comment);
        }

        private static KeyValuePair<string, string>[] CreateStartParts()
        {
            var results = new Dictionary<string, string>();

            foreach (var phrase in Phrases)
            {
                var proposal = Proposal(phrase);

                results.Add($"{phrase}.", $"{proposal}.");
                results.Add($"{phrase}?", $"{proposal}.");
                results.Add($"{phrase}!", $"{proposal}.");
                results.Add($"{phrase},", $"{proposal},");
                results.Add($"{phrase};", $"{proposal};");
                results.Add($"{phrase}:", $"{proposal}:");
            }

            return results.ToArray();
        }

        private static KeyValuePair<string, string>[] CreateMiddleParts()
        {
            var results = new Dictionary<string, string>();

            foreach (var phrase in Phrases)
            {
                var proposal = Proposal(phrase);

                results.Add($"({phrase} ", $"({proposal} ");
                results.Add($"({phrase})", $"({proposal})");
                results.Add($" {phrase})", $" {proposal})");
                results.Add($" {phrase} ", $" {proposal} ");
                results.Add($" {phrase}.", $" {proposal}.");
                results.Add($" {phrase}?", $" {proposal}.");
                results.Add($" {phrase}!", $" {proposal}.");
                results.Add($" {phrase},", $" {proposal},");
                results.Add($" {phrase};", $" {proposal};");
                results.Add($" {phrase}:", $" {proposal}:");
            }

            return results.ToArray();
        }

        private static KeyValuePair<string, string>[] CreateEndParts()
        {
            var results = new Dictionary<string, string>();

            foreach (var phrase in Phrases)
            {
                var proposal = Proposal(phrase);

                results.Add($" {phrase}", $" {proposal}");
            }

            return results.ToArray();
        }

        private static string Proposal(string phrase) => string.Intern($"<see {Constants.XmlTag.Attribute.Langword}=\"{phrase.Trim().ToLowerCase()}\"/>");

        private static string GetWrongText(SyntaxList<XmlAttributeSyntax> attributes)
        {
            var attribute = attributes.First(_ => WrongAttributes.Contains(_.GetName()));
            var token = attribute.ChildTokens().First(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken));

            return token.ValueText;
        }

        private IEnumerable<Diagnostic> AnalyzeComment(string symbolName, DocumentationCommentTriviaSyntax comment)
        {
            var descendantNodes = comment.DescendantNodes(descendant =>
                                                              {
                                                                  switch (descendant)
                                                                  {
                                                                      case DocumentationCommentTriviaSyntax _:
                                                                          return true;

                                                                      case XmlTextSyntax _:
                                                                          return false;

                                                                      case XmlElementSyntax e:
                                                                      {
                                                                          switch (e.GetName())
                                                                          {
                                                                              case "b":
                                                                              case Constants.XmlTag.C:
                                                                              case Constants.XmlTag.Code:
                                                                                  return false; // don't dig deeper

                                                                              default:
                                                                                  return true;
                                                                          }
                                                                      }

                                                                      default:
                                                                          return false;
                                                                  }
                                                              });

            foreach (var descendant in descendantNodes)
            {
                switch (descendant)
                {
                    case XmlElementSyntax e when e.IsWrongBooleanTag() || e.IsWrongNullTag():
                    {
                        var wrongText = e.Content.ToString();
                        var proposal = Proposal(wrongText);

                        yield return Issue(symbolName, e, wrongText, proposal);

                        break;
                    }

                    case XmlEmptyElementSyntax ee when ee.IsSee(WrongAttributes) || ee.IsSeeAlso(WrongAttributes):
                    {
                        var wrongText = GetWrongText(ee.Attributes);
                        var proposal = Proposal(wrongText);

                        yield return Issue(symbolName, ee, wrongText, proposal);

                        break;
                    }

                    case XmlElementSyntax e when e.IsSee(WrongAttributes) || e.IsSeeAlso(WrongAttributes):
                    {
                        var wrongText = GetWrongText(e.StartTag.Attributes);
                        var proposal = Proposal(wrongText);

                        yield return Issue(symbolName, e, wrongText, proposal);

                        break;
                    }

                    case XmlTextSyntax textNode:
                    {
                        foreach (var textToken in textNode.TextTokens)
                        {
                            var text = textToken.ValueText;

                            if (text.IsNullOrWhiteSpace())
                            {
                                continue;
                            }

                            // middle parts
                            foreach (var item in MiddleParts)
                            {
                                var wrongText = item.Key;
                                var proposal = item.Value;

                                const int StartOffset = 1; // we do not want to underline the first char
                                const int EndOffset = 1; // we do not want to underline the last char

                                foreach (var location in GetAllLocations(textToken, wrongText, StringComparison.OrdinalIgnoreCase, StartOffset, EndOffset))
                                {
                                    yield return Issue(symbolName, location, wrongText, proposal);
                                }
                            }

                            // start
                            foreach (var item in StartParts)
                            {
                                var wrongText = item.Key;
                                var proposal = item.Value;

                                const int StartOffset = 0;
                                const int EndOffset = 1; // we do not want to underline the last char

                                var location = GetFirstLocation(textToken, wrongText, StringComparison.OrdinalIgnoreCase, StartOffset, EndOffset);
                                if (location != null)
                                {
                                    yield return Issue(symbolName, location, wrongText, proposal);
                                }
                            }

                            // end
                            foreach (var item in EndParts)
                            {
                                var wrongText = item.Key;
                                var proposal = item.Value;

                                const int StartOffset = 1; // we do not want to underline the first char
                                const int EndOffset = 0;

                                var location = GetLastLocation(textToken, wrongText, StringComparison.OrdinalIgnoreCase, StartOffset, EndOffset);
                                if (location != null)
                                {
                                    yield return Issue(symbolName, location, wrongText, proposal);
                                }
                            }
                        }

                        break;
                    }
                }
            }
        }
    }
}