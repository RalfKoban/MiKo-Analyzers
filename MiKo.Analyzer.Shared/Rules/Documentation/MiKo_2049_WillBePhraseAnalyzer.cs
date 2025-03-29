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
    public sealed class MiKo_2049_WillBePhraseAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2049";

        private const string AcceptedPhrase = "willing";

        private const string WillPhrase = "will";
        private const string NeverPhrase = "never";
        private const string WillNeverPhrase = WillPhrase + " " + NeverPhrase;

        private static readonly string WillPhraseStartUpperCase = WillPhrase.ToUpperCaseAt(0);
        private static readonly string WillNeverPhraseStartUpperCase = WillNeverPhrase.ToUpperCaseAt(0);
        private static readonly string NeverPhraseStartUpperCase = NeverPhrase.ToUpperCaseAt(0);

        private static readonly IDictionary<string, string> PhrasesMap = new Dictionary<string, string>
                                                                             {
                                                                                 { "will be", "is" },
                                                                                 { "will also be", "is" },
                                                                                 { "will as well be", "is" },
                                                                                 { "will not be", "is not" },
                                                                                 { "will not", "does not" },
                                                                                 { "will never be", "is never" },
                                                                                 { "will also never be", "is also never" },
                                                                                 { "will base", "is based" },
                                                                                 { "will all be", "all are" },
                                                                                 { "will all", "all" },
                                                                                 { "will always be", "is always" },
                                                                                 { "will both be", "are both" },
                                                                             };

        private static readonly string[] PhrasesMapKeys = PhrasesMap.Keys.ToArray();

        private static readonly string[] Phrases = PhrasesMapKeys.WithDelimiters();

        private static readonly int MinimumPhraseLength = Phrases.Min(_ => _.Length);

        public MiKo_2049_WillBePhraseAnalyzer() : base(Id)
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

            if (count < 2)
            {
                return issues;
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
            var issues = new List<Diagnostic>(0);

            var textTokens = comment.GetXmlTextTokens();
            var textTokensCount = textTokens.Count;

            if (textTokensCount == 0)
            {
                return issues;
            }

            var commentXml = textTokens.GetTextTrimmedWithParaTags();

            if (commentXml.Contains(WillPhrase, StringComparison.OrdinalIgnoreCase) is false)
            {
                return issues;
            }

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            for (var i = 0; i < textTokensCount; i++)
            {
                var token = textTokens[i];

                if (token.ValueText.Length < MinimumPhraseLength)
                {
                    continue;
                }

                const int Offset = 1; // we do not want to underline the first and last char

                foreach (var location in GetAllLocations(token, Phrases, StringComparison.OrdinalIgnoreCase, Offset, Offset))
                {
                    var text = location.GetText().ToLowerCase();

                    if (PhrasesMap.TryGetValue(text, out var replacement))
                    {
                        issues.Add(Issue(location, replacement));
                    }
                }

                // ReSharper disable once LoopCanBeConvertedToQuery
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var issue in AnalyzeForSpecialPhrase(token, WillPhraseStartUpperCase, _ => Verbalizer.MakeThirdPersonSingularVerb(_.ToUpperCaseAt(0))))
                {
                    var text = issue.Location.GetText().ToLowerCase();

                    if (text.Contains(AcceptedPhrase) is false)
                    {
                        issues.Add(issue);
                    }
                }

                // ReSharper disable once LoopCanBeConvertedToQuery
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var issue in AnalyzeForSpecialPhrase(token, WillPhrase, Verbalizer.MakeThirdPersonSingularVerb))
                {
                    var text = issue.Location.GetText().ToLowerCase();

                    if (text.Contains(AcceptedPhrase) is false && text.ContainsAny(PhrasesMapKeys) is false)
                    {
                        issues.Add(issue);
                    }
                }

                issues.AddRange(AnalyzeForSpecialPhrase(token, WillNeverPhraseStartUpperCase, _ => NeverPhraseStartUpperCase + " " + Verbalizer.MakeThirdPersonSingularVerb(_)));
                issues.AddRange(AnalyzeForSpecialPhrase(token, WillNeverPhrase, _ => NeverPhrase + " " + Verbalizer.MakeThirdPersonSingularVerb(_)));
            }

            return issues;
        }
    }
}