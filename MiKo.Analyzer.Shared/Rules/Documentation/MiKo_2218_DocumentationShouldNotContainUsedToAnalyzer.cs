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
        private const string CanPluralReplacement = "allow to";
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

        private const string UsedWhenPhrase = "to be used when";
        private const string UsedWhenReplacement = "suitable when";

        private const string UsedWithinReplacement = "applicable to";

        private const string UsedToDetermineInSingularReplacement = "defines";
        private const string UsedToDetermineInPluralReplacement = "define";

        private static readonly string[] CanPhrases =
                                                      {
                                                          "can be used in order to",
                                                          "can be used to",
                                                          "could be used in order to",
                                                          "could be used to",
                                                          "may be used to",
                                                          "might be used to",
                                                          "is expected to be used to",
                                                          "is intended to be used to",
                                                          "is meant to be used to",
                                                          "is primarily expected to be used to",
                                                          "is primarily intended to be used to",
                                                          "is primarily meant to be used to",
                                                      };

        private static readonly string[] CanPluralPhrases =
                                                            {
                                                                "are expected to be used to",
                                                                "are intended to be used to",
                                                                "are meant to be used to",
                                                                "are primarily expected to be used to",
                                                                "are primarily intended to be used to",
                                                                "are primarily meant to be used to",
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
                                                             "to be used to",
                                                         };

        private static readonly string[] UsedInCombinationPluralPhrases =
                                                                          {
                                                                              "are expected to be used in combination with",
                                                                              "are expected to be used in conjunction with",
                                                                              "are intended to be used in combination with",
                                                                              "are intended to be used in conjunction with",
                                                                              "are meant to be used in combination with",
                                                                              "are meant to be used in conjunction with",
                                                                              "are primarily expected to be used in combination with",
                                                                              "are primarily expected to be used in conjunction with",
                                                                              "are primarily intended to be used in combination with",
                                                                              "are primarily intended to be used in conjunction with",
                                                                              "are primarily meant to be used in combination with",
                                                                              "are primarily meant to be used in conjunction with",
                                                                              "are to be used in combination with",
                                                                              "are to be used in conjunction with",
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
                                                                                "is expected to be used in combination with",
                                                                                "is expected to be used in conjunction with",
                                                                                "is intended to be used in combination with",
                                                                                "is intended to be used in conjunction with",
                                                                                "is meant to be used in combination with",
                                                                                "is meant to be used in conjunction with",
                                                                                "is primarily expected to be used in combination with",
                                                                                "is primarily expected to be used in conjunction with",
                                                                                "is primarily intended to be used in combination with",
                                                                                "is primarily intended to be used in conjunction with",
                                                                                "is primarily meant to be used in combination with",
                                                                                "is primarily meant to be used in conjunction with",
                                                                                "may be used in combination with",
                                                                                "may be used in conjunction with",
                                                                                "might be used in combination with",
                                                                                "might be used in conjunction with",
                                                                            };

        private static readonly string[] UsedInCombinationUnclearPhrases =
                                                                           {
                                                                               "expected to be used in combination with",
                                                                               "expected to be used in conjunction with",
                                                                               "intended to be used in combination with",
                                                                               "intended to be used in conjunction with",
                                                                               "meant to be used in combination with",
                                                                               "meant to be used in conjunction with",
                                                                           };

        private static readonly string[] UsedInPluralPhrases =
                                                               {
                                                                   "are primarily expected to be used by",
                                                                   "are primarily expected to be used for",
                                                                   "are primarily expected to be used in",
                                                                   "are primarily intended to be used by",
                                                                   "are primarily intended to be used for",
                                                                   "are primarily intended to be used in",
                                                                   "are primarily meant to be used by",
                                                                   "are primarily meant to be used for",
                                                                   "are primarily meant to be used in",
                                                                   "are expected to be used by",
                                                                   "are expected to be used for",
                                                                   "are expected to be used in",
                                                                   "are intended to be used by",
                                                                   "are intended to be used for",
                                                                   "are intended to be used in",
                                                                   "are meant to be used by",
                                                                   "are meant to be used for",
                                                                   "are meant to be used in",
                                                                   "are to be used by",
                                                                   "are to be used in",
                                                                   "have to be used in",
                                                               };

        private static readonly string[] UsedInSingularPhrases =
                                                                 {
                                                                     "can be used in",
                                                                     "could be used in",
                                                                     "has to be used in",
                                                                     "is primarily expected to be used by",
                                                                     "is primarily expected to be used in",
                                                                     "is primarily intended to be used by",
                                                                     "is primarily intended to be used in",
                                                                     "is primarily meant to be used by",
                                                                     "is primarily meant to be used in",
                                                                     "is expected to be used by",
                                                                     "is expected to be used in",
                                                                     "is intended to be used by",
                                                                     "is intended to be used in",
                                                                     "is meant to be used by",
                                                                     "is meant to be used in",
                                                                     "may be used by",
                                                                     "may be used for",
                                                                     "may be used in",
                                                                     "might be used by",
                                                                     "might be used for",
                                                                     "might be used in",
                                                                 };

        private static readonly string[] UsedInUnclearPhrases =
                                                                {
                                                                    "primarily expected to be used by",
                                                                    "primarily expected to be used for",
                                                                    "primarily expected to be used in",
                                                                    "primarily intended to be used by",
                                                                    "primarily intended to be used for",
                                                                    "primarily intended to be used in",
                                                                    "primarily meant to be used by",
                                                                    "primarily meant to be used for",
                                                                    "primarily meant to be used in",
                                                                    "expected to be used by",
                                                                    "expected to be used for",
                                                                    "expected to be used in",
                                                                    "intended to be used by",
                                                                    "intended to be used for",
                                                                    "intended to be used in",
                                                                    "meant to be used by",
                                                                    "meant to be used for",
                                                                    "meant to be used in",
                                                                    "to be used by",
                                                                    "to be used during",
                                                                    "to be used for",
                                                                    "to be used with",
                                                                };

        private static readonly string[] UsedWithinPhrases =
                                                             {
                                                                 "primarily expected to be used within",
                                                                 "expected to be used within",
                                                                 "to be used within",
                                                             };

        private static readonly string[] UsedToDetermineInSingular =
                                                                     {
                                                                         "is used to determine",
                                                                         "is used to find out",
                                                                         "is used to check",
                                                                     };

        private static readonly string[] UsedToDetermineInPlural =
                                                                   {
                                                                       "are used to determine",
                                                                       "are used to find out",
                                                                       "are used to check",
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

        protected override Diagnostic Issue(Location location, string replacement)
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

                foreach (var location in GetAllLocations(token, CanPhrases, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(location, CanReplacement);
                }

                foreach (var location in GetAllLocations(token, CanPluralPhrases, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(location, CanPluralReplacement);
                }

                foreach (var location in GetAllLocations(token, UsedToDetermineInSingular, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(location, UsedToDetermineInSingularReplacement);
                }

                foreach (var location in GetAllLocations(token, UsedToDetermineInPlural, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(location, UsedToDetermineInPluralReplacement);
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

                foreach (var location in GetAllLocations(token, UsedWhenPhrase, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(location, UsedWhenReplacement);
                }

                foreach (var location in GetAllLocations(token, UsedWithinPhrases, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(location, UsedWithinReplacement);
                }
            }
        }
    }
}