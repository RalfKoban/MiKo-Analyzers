using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2039_CodeFixProvider)), Shared]
    public sealed class MiKo_2039_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly string[] Parts = Constants.Comments.ExtensionMethodClassStartingPhraseTemplate.FormatWith('|').Split('|');

        private static readonly string[] ReplacementMapKeys = CreateReplacementMapKeys().ToArray();

        private static readonly KeyValuePair<string, string>[] ReplacementMap = ReplacementMapKeys.Select(_ => new KeyValuePair<string, string>(_, string.Empty)).ToArray();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2039";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = PrepareComment((XmlElementSyntax)syntax);

            return CommentStartingWith(comment, Parts[0], SeeLangword("static"), Parts[1]);
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment) => Comment(comment, ReplacementMapKeys, ReplacementMap);

//// ncrunch: rdi off

        private static IEnumerable<string> CreateReplacementMapKeys()
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
                                  "extension mehtod", // typo by intent
                                  "extension-mehtod", // typo by intent
                                  "extension mehtods", // typo by intent
                                  "extension-mehtods", // typo by intent
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

            return results;
        }

//// ncrunch: rdi default
    }
}