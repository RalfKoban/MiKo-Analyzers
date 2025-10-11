using System;
using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2039_CodeFixProvider)), Shared]
    public sealed class MiKo_2039_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly string[] Parts = Constants.Comments.ExtensionMethodClassStartingPhraseTemplate.FormatWith("|").Split('|');

        private static readonly string[] ReplacementMapKeys = CreateReplacementMapKeys().OrderDescendingByLengthAndText();

        private static readonly Pair[] ReplacementMap = ReplacementMapKeys.ToArray(_ => new Pair(_));

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2039";

        internal static bool CanFix(in ReadOnlySpan<char> text) => text.StartsWithAny(ReplacementMapKeys, StringComparison.OrdinalIgnoreCase);

        internal static SyntaxNode GetUpdatedSyntax(XmlElementSyntax syntax)
        {
            var comment = Comment(syntax, ReplacementMapKeys, ReplacementMap);

            return CommentStartingWith(comment, Parts[0], SeeLangword("static"), Parts[1]);
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntax((XmlElementSyntax)syntax);

//// ncrunch: rdi off

        private static HashSet<string> CreateReplacementMapKeys()
        {
            var starts = new[]
                             {
                                 string.Empty,
                                 "Class containing",
                                 "Contains",
                                 "Offers",
                                 "Provides",
                                 "Static collection of",
                                 "The",
                             };

            var preMiddles = new[]
                                 {
                                     string.Empty,
                                     " different",
                                     " the",
                                 };

            var middles = new[]
                              {
                                  "extension",
                                  "extensions",
                                  "extension method",
                                  "extension methods",
                                  "extension-method",
                                  "extension-methods",
                                  "extension mehtod", // typo by intent because we want to fix that as well
                                  "extension-mehtod", // typo by intent because we want to fix that as well
                                  "extension mehtods", // typo by intent because we want to fix that as well
                                  "extension-mehtods", // typo by intent because we want to fix that as well
                              };

            var ends = new[]
                           {
                               "for",
                               "to",
                               "used in",
                           };

            var results = new HashSet<string>();

            foreach (var start in starts)
            {
                foreach (var preMiddle in preMiddles)
                {
                    foreach (var middle in middles)
                    {
                        foreach (var end in ends)
                        {
                            results.Add(start.IsNullOrWhiteSpace()
                                        ? string.Concat(middle.ToUpperCaseAt(0), " ", end)
                                        : string.Concat(start, preMiddle, " ", middle, " ", end));
                        }
                    }
                }
            }

            var alternativeStarts = new[] { "class", "a class", "the class" };

            foreach (var start in alternativeStarts)
            {
                foreach (var middle in middles)
                {
                    var phrase = start + " for " + middle;

                    var phrase1 = phrase + " that extend";
                    var phrase2 = phrase + " which extend";
                    var phrase3 = phrase + " extending";

                    results.Add(phrase1);
                    results.Add(phrase1.ToUpperCaseAt(0));
                    results.Add(phrase2);
                    results.Add(phrase2.ToUpperCaseAt(0));
                    results.Add(phrase3);
                    results.Add(phrase3.ToUpperCaseAt(0));

                    results.Add(phrase);
                    results.Add(phrase.ToUpperCaseAt(0));
                }
            }

            return results;
        }

//// ncrunch: rdi default
    }
}