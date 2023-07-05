using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2218_DocumentationShouldNotContainUsedToAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2218";

        private const string TextKey = "TextKey";
        private const string TextReplacementKey = "TextReplacementKey";

        private const string CanReplacement = "allows to";
        private const string UsedToReplacement = "to";

        private const string UsedInCombinationPluralReplacement = "are made to work with";
        private const string UsedInCombinationSingularReplacement = "is made to work with";
        private const string UsedInCombinationUnclearReplacement = "made to work with";

        private const string UsedInPluralReplacement = "are suitable for";
        private const string UsedInSingularReplacement = "is suitable for";
        private const string UsedInUnclearReplacement = "suitable for";

        private const string UsedToPhrase = "used to";
        private const string IsUsedToPhrase = "is used to";
        private const string AreUsedToPhrase = "are used to";

        private const string UsedByPhrase = "to be used by";
        private const string UsedByReplacement = "for";

        private static readonly string[] CanPhrases =
                                                      {
                                                          "can be used in order to",
                                                          "can be used to",
                                                          "could be used in order to",
                                                          "could be used to",
                                                      };

        private static readonly string[] UsedToPhrases =
                                                         {
                                                             "that is used in order to",
                                                             "that is used to",
                                                             "that it is used in order to",
                                                             "that it is used to",
                                                             "that are used in order to",
                                                             "that are used to",
                                                             "that shall be used in order to",
                                                             "that shall be used to",
                                                             "that should be used in order to",
                                                             "that should be used to",
                                                             "that should currently be used in order to",
                                                             "that should currently be used to",
                                                             "that will be used in order to",
                                                             "that will be used to",
                                                             "that would be used in order to",
                                                             "that would be used to",
                                                             "which is used in order to",
                                                             "which is used to",
                                                             "which it is used in order to",
                                                             "which it is used to",
                                                             "which are used in order to",
                                                             "which are used to",
                                                             "which shall be used in order to",
                                                             "which shall be used to",
                                                             "which should be used in order to",
                                                             "which should be used to",
                                                             "which should currently be used in order to",
                                                             "which should currently be used to",
                                                             "which will be used in order to",
                                                             "which will be used to",
                                                             "which would be used in order to",
                                                             "which would be used to",
                                                         };

        private static readonly string[] UsedInCombinationPluralPhrases =
                                                                          {
                                                                              "are to be used in combination with",
                                                                              "are to be used in conjunction with",
                                                                              "are intended to be used in combination with",
                                                                              "are intended to be used in conjunction with",
                                                                              "are meant to be used in combination with",
                                                                              "are meant to be used in conjunction with",
                                                                              "are primarily meant to be used in combination with",
                                                                              "are primarily meant to be used in conjunction with",
                                                                              "are primarily intended to be used in combination with",
                                                                              "are primarily intended to be used in conjunction with",
                                                                              "have to be used in combination with",
                                                                              "have to be used in conjunction with",
                                                                          };

        private static readonly string[] UsedInCombinationSingularPhrases =
                                                                            {
                                                                                "can be used in combination with",
                                                                                "can be used in conjunction with",
                                                                                "could be used in combination with",
                                                                                "could be used in conjunction with",
                                                                                "has to be used in combination with",
                                                                                "has to be used in conjunction with",
                                                                                "is intended to be used in combination with",
                                                                                "is intended to be used in conjunction with",
                                                                                "is meant to be used in combination with",
                                                                                "is meant to be used in conjunction with",
                                                                                "is primarily meant to be used in combination with",
                                                                                "is primarily meant to be used in conjunction with",
                                                                                "is primarily intended to be used in combination with",
                                                                                "is primarily intended to be used in conjunction with",
                                                                                "might be used in combination with",
                                                                                "might be used in conjunction with",
                                                                            };

        private static readonly string[] UsedInCombinationUnclearPhrases =
                                                                           {
                                                                               "meant to be used in combination with",
                                                                               "meant to be used in conjunction with",
                                                                           };

        private static readonly string[] UsedInPluralPhrases =
                                                               {
                                                                   "are to be used in",
                                                                   "are intended to be used in",
                                                                   "are meant to be used in",
                                                                   "are primarily meant to be used in",
                                                                   "are primarily intended to be used in",
                                                                   "have to be used in",
                                                               };

        private static readonly string[] UsedInSingularPhrases =
                                                                 {
                                                                     "can be used in",
                                                                     "could be used in",
                                                                     "has to be used in",
                                                                     "is intended to be used in",
                                                                     "is meant to be used in",
                                                                     "is primarily meant to be used in",
                                                                     "is primarily intended to be used in",
                                                                     "might be used in",
                                                                 };

        private static readonly string[] UsedInUnclearPhrases =
                                                                {
                                                                    "meant to be used in",
                                                                };

        public MiKo_2218_DocumentationShouldNotContainUsedToAnalyzer() : base(Id)
        {
        }

        internal static XmlTextSyntax GetBetterText(XmlTextSyntax node, Diagnostic issue)
        {
            var tokens = node.TextTokens.Where(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken));

            var properties = issue.Properties;
            var textToReplace = properties[TextKey];
            var textToReplaceWith = properties[TextReplacementKey];

            var tokensToReplace = new Dictionary<SyntaxToken, SyntaxToken>();

            foreach (var token in tokens)
            {
                var text = token.Text;

                if (text.Length <= Constants.EnvironmentNewLine.Length && text.IsNullOrWhiteSpace())
                {
                    // do not bother with only empty text
                    continue;
                }

                tokensToReplace[token] = token.WithText(text.Replace(textToReplace, textToReplaceWith));
            }

            if (tokensToReplace.Any())
            {
                return node.ReplaceTokens(tokensToReplace.Keys, (original, rewritten) => tokensToReplace[original]);
            }

            return node;
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var alreadyReportedLocations = new List<Location>();

            var issues = AnalyzeCommentXml(comment).OrderByDescending(_ => _.Location.SourceSpan.Length).ToList(); // find largest parts first

            foreach (var issue in issues)
            {
                var location = issue.Location;

                if (alreadyReportedLocations.Any(_ => location.IntersectsWith(_)))
                {
                    // already reported, so ignore it
                    continue;
                }

                alreadyReportedLocations.Add(location);

                yield return issue;
            }
        }

        private IEnumerable<Diagnostic> AnalyzeCommentXml(DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.GetXmlTextTokens())
            {
                foreach (var location in GetAllLocations(token, UsedToPhrases))
                {
                    yield return Issue(location, UsedToReplacement);
                }

                foreach (var canPhrase in CanPhrases)
                {
                    foreach (var location in GetAllLocations(token, canPhrase, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return Issue(location, CanReplacement);
                    }
                }

                foreach (var issue in AnalyzeForSpecialPhrase(token, IsUsedToPhrase.ToUpperCaseAt(0), _ => Verbalizer.MakeThirdPersonSingularVerb(_).ToUpperCaseAt(0)))
                {
                    yield return issue;
                }

                foreach (var issue in AnalyzeForSpecialPhrase(token, AreUsedToPhrase.ToUpperCaseAt(0), _ => Verbalizer.MakeThirdPersonSingularVerb(_).ToUpperCaseAt(0)))
                {
                    yield return issue;
                }

                foreach (var issue in AnalyzeForSpecialPhrase(token, UsedToPhrase.ToUpperCaseAt(0), _ => Verbalizer.MakeThirdPersonSingularVerb(_).ToUpperCaseAt(0)))
                {
                    yield return issue;
                }

                foreach (var issue in AnalyzeForSpecialPhrase(token, IsUsedToPhrase, Verbalizer.MakeThirdPersonSingularVerb))
                {
                    yield return issue;
                }

                foreach (var issue in AnalyzeForSpecialPhrase(token, AreUsedToPhrase, Verbalizer.MakeInfiniteVerb))
                {
                    yield return issue;
                }

                foreach (var location in GetAllLocations(token, UsedToPhrase)) // do not use case insensitive here
                {
                    yield return Issue(location, UsedToReplacement);
                }

                foreach (var location in GetAllLocations(token, UsedInCombinationPluralPhrases, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(location, UsedInCombinationPluralReplacement);
                }

                foreach (var location in GetAllLocations(token, UsedInCombinationSingularPhrases, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(location, UsedInCombinationSingularReplacement);
                }

                foreach (var location in GetAllLocations(token, UsedInCombinationUnclearPhrases, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(location, UsedInCombinationUnclearReplacement);
                }

                foreach (var location in GetAllLocations(token, UsedInPluralPhrases, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(location, UsedInPluralReplacement);
                }

                foreach (var location in GetAllLocations(token, UsedInSingularPhrases, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(location, UsedInSingularReplacement);
                }

                foreach (var location in GetAllLocations(token, UsedInUnclearPhrases, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(location, UsedInUnclearReplacement);
                }

                foreach (var location in GetAllLocations(token, UsedByPhrase, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(location, UsedByReplacement);
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeForSpecialPhrase(SyntaxToken token, string startingPhrase, Func<string, string> replacementCallback)
        {
            foreach (var location in GetAllLocations(token, startingPhrase))
            {
                var start = location.SourceSpan.Start;
                var index = start - token.SpanStart + startingPhrase.Length;

                var textAfterStartingPhrase = token.ValueText.AsSpan(index);
                var nextWord = textAfterStartingPhrase.FirstWord();

                // let's find the end of the next word in the source code (but keep in mind the offset of the starting phrase)
                var offset = start + startingPhrase.Length;
                var end = textAfterStartingPhrase.IndexOf(nextWord, StringComparison.Ordinal) + nextWord.Length + offset;

                var finalLocation = CreateLocation(token, start, end);
                var replacement = replacementCallback(nextWord.ToString());

                yield return Issue(finalLocation, replacement);
            }
        }

        private Diagnostic Issue(Location location, string replacement)
        {
            var text = location.GetText();

            if (text[0].IsUpperCaseLetter())
            {
                text = text.ToUpperCaseAt(0);
                replacement = replacement.ToUpperCaseAt(0);
            }

            var properties = new Dictionary<string, string>
                                 {
                                     { TextKey, text },
                                     { TextReplacementKey, replacement },
                                 };

            return Issue(location, replacement, properties);
        }
    }
}