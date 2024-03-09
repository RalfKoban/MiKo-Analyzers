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

            var properties = new Dictionary<string, string>
                                 {
                                     { Constants.AnalyzerCodeFixSharedData.TextKey, text },
                                     { Constants.AnalyzerCodeFixSharedData.TextReplacementKey, replacement },
                                 };

            return Issue(location, replacement, properties);
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var alreadyReportedLocations = new List<Location>();

            var issues = AnalyzeCommentXml(comment).OrderByDescending(_ => _.Location.SourceSpan.Length); // find largest parts first

            foreach (var issue in issues)
            {
                var location = issue.Location;

                if (alreadyReportedLocations.Exists(_ => location.IntersectsWith(_)))
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
                const int Offset = 1; // we do not want to underline the first and last char

                foreach (var location in GetAllLocations(token, Phrases, StringComparison.OrdinalIgnoreCase, Offset, Offset))
                {
                    var text = location.GetText().ToLowerInvariant();

                    if (PhrasesMap.TryGetValue(text, out var replacement))
                    {
                        yield return Issue(location, replacement);
                    }
                }

                foreach (var issue in AnalyzeForSpecialPhrase(token, WillPhraseStartUpperCase, _ => Verbalizer.MakeThirdPersonSingularVerb(_).ToUpperCaseAt(0)))
                {
                    var text = issue.Location.GetText().ToLowerInvariant();

                    if (text.Contains(AcceptedPhrase) is false)
                    {
                        yield return issue;
                    }
                }

                foreach (var issue in AnalyzeForSpecialPhrase(token, WillPhrase, Verbalizer.MakeThirdPersonSingularVerb))
                {
                    var text = issue.Location.GetText().ToLowerInvariant();

                    if (text.Contains(AcceptedPhrase) is false && text.ContainsAny(PhrasesMapKeys) is false)
                    {
                        yield return issue;
                    }
                }

                foreach (var issue in AnalyzeForSpecialPhrase(token, WillNeverPhraseStartUpperCase, _ => NeverPhraseStartUpperCase + " " + Verbalizer.MakeThirdPersonSingularVerb(_)))
                {
                    yield return issue;
                }

                foreach (var issue in AnalyzeForSpecialPhrase(token, WillNeverPhrase, _ => NeverPhrase + " " + Verbalizer.MakeThirdPersonSingularVerb(_)))
                {
                    yield return issue;
                }
            }
        }
    }
}