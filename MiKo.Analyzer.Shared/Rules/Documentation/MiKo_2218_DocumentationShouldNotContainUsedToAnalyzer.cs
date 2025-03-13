using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2218_DocumentationShouldNotContainUsedToAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2218";

        private const string CanReplacement = "allows to";
        private const string CanPluralReplacement = "allow to";
        private const string UsedToReplacement = "to";

        private const string UsedInCombinationPluralReplacement = "are made to work with";
        private const string UsedInCombinationSingularReplacement = "is made to work with";
        private const string UsedInCombinationUnclearReplacement = "made to work with";

        private const string UsedInPluralReplacement = "are suitable for";
        private const string UsedInSingularReplacement = "is suitable for";
        private const string UsedInUnclearReplacement = "suitable for";

        private const string UsedInternallyPluralReplacement = "are suitable for internal use";
        private const string UsedInternallySingularReplacement = "is suitable for internal use";
        private const string UsedInternallyUnclearReplacement = "suitable for internal use";

        private const string UsedToPhrase = "used to";
        private const string IsUsedToPhrase = "is used to";
        private const string AreUsedToPhrase = "are used to";

        private const string UsedByPhrase = "to be used by";
        private const string UsedByReplacement = "for";

        private const string UsedWhenReplacement = "suitable when";

        private const string UsedWithinReplacement = "applicable to";

        private const string UsedToDetermineInSingularReplacement = "defines";
        private const string UsedToDetermineInPluralReplacement = "define";

        private static readonly string IsUsedToPhraseStartUpperCase = IsUsedToPhrase.ToUpperCaseAt(0);
        private static readonly string AreUsedToPhraseStartUpperCase = AreUsedToPhrase.ToUpperCaseAt(0);
        private static readonly string UsedToPhraseStartUpperCase = UsedToPhrase.ToUpperCaseAt(0);

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

        private static readonly string[] UsedInternallyPluralPhrases =
                                                                       {
                                                                           "are primarily expected to be used internally",
                                                                           "are primarily intended to be used internally",
                                                                           "are primarily meant to be used internally",
                                                                           "are expected to be used internally",
                                                                           "are intended to be used internally",
                                                                           "are meant to be used internally",
                                                                           "are to be used internally",
                                                                           "have to be used internally",
                                                                       };

        private static readonly string[] UsedInternallySingularPhrases =
                                                                         {
                                                                             "can be used internally",
                                                                             "could be used internally",
                                                                             "has to be used internally",
                                                                             "is primarily expected to be used internally",
                                                                             "is primarily intended to be used internally",
                                                                             "is primarily meant to be used internally",
                                                                             "is expected to be used internally",
                                                                             "is intended to be used internally",
                                                                             "is meant to be used internally",
                                                                             "may be used internally",
                                                                             "might be used internally",
                                                                         };

        private static readonly string[] UsedInternallyUnclearPhrases =
                                                                        {
                                                                            "primarily expected to be used internally",
                                                                            "primarily intended to be used internally",
                                                                            "primarily meant to be used internally",
                                                                            "expected to be used internally",
                                                                            "intended to be used internally",
                                                                            "meant to be used internally",
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

        private static readonly string[] UsedWhenPhrases =
                                                           {
                                                               "intended to be used when",
                                                               "to be used when",
                                                           };

        public MiKo_2218_DocumentationShouldNotContainUsedToAnalyzer() : base(Id)
        {
        }

        protected override Diagnostic Issue(Location location, string replacement)
        {
            var text = location.GetText();

            if (text[0].IsUpperCaseLetter())
            {
                text = text.ToUpperCaseAt(0);
                replacement = replacement.ToUpperCaseAt(0);
            }

            var properties = new[]
                                 {
                                     new Pair(Constants.AnalyzerCodeFixSharedData.TextKey, text),
                                     new Pair(Constants.AnalyzerCodeFixSharedData.TextReplacementKey, replacement),
                                 };

            return Issue(location, replacement, properties);
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var issues = AnalyzeCommentXml(comment);
            var count = issues.Count;

            switch (count)
            {
                case 0: return Array.Empty<Diagnostic>();
                case 1: return new[] { issues[0] };
            }

            var alreadyReportedLocations = new List<Location>(count);
            var finalIssues = new List<Diagnostic>(count);

            foreach (var issue in issues.OrderByDescending(_ => _.Location.SourceSpan.Length)) // find largest parts first
            {
                var location = issue.Location;

                if (alreadyReportedLocations.Exists(_ => location.IntersectsWith(_)))
                {
                    // already reported, so ignore it
                    continue;
                }

                alreadyReportedLocations.Add(location);
                finalIssues.Add(issue);
            }

            return finalIssues;
        }

        private List<Diagnostic> AnalyzeCommentXml(DocumentationCommentTriviaSyntax comment)
        {
            var issues = new List<Diagnostic>();

            var textTokens = comment.GetXmlTextTokens();
            var textTokensCount = textTokens.Count;

            if (textTokensCount > 0)
            {
                for (var i = 0; i < textTokensCount; i++)
                {
                    var token = textTokens[i];

                    AnalyzeForPhrases(issues, token, UsedToPhrases, UsedToReplacement);
                    AnalyzeForPhrases(issues, token, CanPhrases, CanReplacement, StringComparison.OrdinalIgnoreCase);
                    AnalyzeForPhrases(issues, token, CanPluralPhrases, CanPluralReplacement, StringComparison.OrdinalIgnoreCase);
                    AnalyzeForPhrases(issues, token, UsedToDetermineInSingular, UsedToDetermineInSingularReplacement, StringComparison.OrdinalIgnoreCase);
                    AnalyzeForPhrases(issues, token, UsedToDetermineInPlural, UsedToDetermineInPluralReplacement, StringComparison.OrdinalIgnoreCase);

                    issues.AddRange(AnalyzeForSpecialPhrase(token, IsUsedToPhraseStartUpperCase, _ => Verbalizer.MakeThirdPersonSingularVerb(_).ToUpperCaseAt(0)));
                    issues.AddRange(AnalyzeForSpecialPhrase(token, AreUsedToPhraseStartUpperCase, _ => Verbalizer.MakeThirdPersonSingularVerb(_).ToUpperCaseAt(0)));
                    issues.AddRange(AnalyzeForSpecialPhrase(token, UsedToPhraseStartUpperCase, _ => Verbalizer.MakeThirdPersonSingularVerb(_).ToUpperCaseAt(0)));
                    issues.AddRange(AnalyzeForSpecialPhrase(token, IsUsedToPhrase, Verbalizer.MakeThirdPersonSingularVerb));
                    issues.AddRange(AnalyzeForSpecialPhrase(token, AreUsedToPhrase, Verbalizer.MakeInfiniteVerb));

                    AnalyzeForPhrases(issues, token, UsedToPhrase, UsedToReplacement); // do not use case-insensitive here
                    AnalyzeForPhrases(issues, token, UsedInCombinationPluralPhrases, UsedInCombinationPluralReplacement, StringComparison.OrdinalIgnoreCase);
                    AnalyzeForPhrases(issues, token, UsedInCombinationSingularPhrases, UsedInCombinationSingularReplacement, StringComparison.OrdinalIgnoreCase);
                    AnalyzeForPhrases(issues, token, UsedInCombinationUnclearPhrases, UsedInCombinationUnclearReplacement, StringComparison.OrdinalIgnoreCase);
                    AnalyzeForPhrases(issues, token, UsedInternallyPluralPhrases, UsedInternallyPluralReplacement, StringComparison.OrdinalIgnoreCase);
                    AnalyzeForPhrases(issues, token, UsedInternallySingularPhrases, UsedInternallySingularReplacement, StringComparison.OrdinalIgnoreCase);
                    AnalyzeForPhrases(issues, token, UsedInternallyUnclearPhrases, UsedInternallyUnclearReplacement, StringComparison.OrdinalIgnoreCase);
                    AnalyzeForPhrases(issues, token, UsedInPluralPhrases, UsedInPluralReplacement, StringComparison.OrdinalIgnoreCase);
                    AnalyzeForPhrases(issues, token, UsedInSingularPhrases, UsedInSingularReplacement, StringComparison.OrdinalIgnoreCase);
                    AnalyzeForPhrases(issues, token, UsedInUnclearPhrases, UsedInUnclearReplacement, StringComparison.OrdinalIgnoreCase);
                    AnalyzeForPhrases(issues, token, UsedByPhrase, UsedByReplacement, StringComparison.OrdinalIgnoreCase);
                    AnalyzeForPhrases(issues, token, UsedWhenPhrases, UsedWhenReplacement, StringComparison.OrdinalIgnoreCase);
                    AnalyzeForPhrases(issues, token, UsedWithinPhrases, UsedWithinReplacement, StringComparison.OrdinalIgnoreCase);
                }
            }

            return issues;
        }

        private void AnalyzeForPhrases(List<Diagnostic> issues, SyntaxToken token, string[] phrases, string replacement, StringComparison comparison = StringComparison.Ordinal)
        {
            var locations = GetAllLocations(token, phrases, comparison);

            if (locations.Count > 0)
            {
                AddIssues(issues, replacement, locations);
            }
        }

        private void AnalyzeForPhrases(List<Diagnostic> issues, SyntaxToken token, string phrase, string replacement, StringComparison comparison = StringComparison.Ordinal)
        {
            var locations = GetAllLocations(token, phrase, comparison);

            if (locations.Count > 0)
            {
                AddIssues(issues, replacement, locations);
            }
        }

        private void AddIssues(List<Diagnostic> issues, string replacement, IReadOnlyList<Location> locations)
        {
            var count = locations.Count;

            for (var index = 0; index < count; index++)
            {
                issues.Add(Issue(locations[index], replacement));
            }
        }
    }
}