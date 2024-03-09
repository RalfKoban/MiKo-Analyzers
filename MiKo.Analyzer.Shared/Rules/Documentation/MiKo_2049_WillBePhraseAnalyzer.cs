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
    public sealed class MiKo_2049_WillBePhraseAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2049";

        private const string TextKey = "TextKey";
        private const string TextReplacementKey = "TextReplacementKey";

        private const string AcceptedPhrase = "willing";

        private const string IsPhrase = "is";
        private const string ArePhrase = "are";
        private const string DoPhrase = "do";
        private const string DoesPhrase = "does";

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

        internal static XmlTextSyntax GetBetterText(XmlTextSyntax node, Diagnostic issue)
        {
            var properties = issue.Properties;
            var textToReplace = properties[TextKey];
            var textToReplaceWith = properties[TextReplacementKey];

            var tokensToReplace = new Dictionary<SyntaxToken, SyntaxToken>();

            var textTokens = node.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            for (var index = 0; index < textTokensCount; index++)
            {
                var token = textTokens[index];

                if (token.IsKind(SyntaxKind.XmlTextLiteralToken))
                {
                    var text = token.Text;

                    if (text.Length <= Constants.EnvironmentNewLine.Length && text.IsNullOrWhiteSpace())
                    {
                        // do not bother with only empty text
                        continue;
                    }

                    var start = text.IndexOf(textToReplace, StringComparison.Ordinal);

                    if (start < 0)
                    {
                        // does not seem to fit
                        continue;
                    }

                    var startingPart = text.AsSpan(0, start);
                    var lastWord = startingPart.LastWord().ToString();

                    // let's see if we have to deal with 'does' or 'is' but need to have plural
                    if (Verbalizer.IsPlural(lastWord.AsSpan()))
                    {
                        if (textToReplaceWith.StartsWith(IsPhrase, StringComparison.Ordinal))
                        {
                            textToReplaceWith = ArePhrase + textToReplaceWith.Substring(2);
                        }
                        else if (textToReplaceWith.StartsWith(DoesPhrase, StringComparison.Ordinal))
                        {
                            textToReplaceWith = DoPhrase + textToReplaceWith.Substring(4);
                        }
                    }

                    tokensToReplace[token] = token.WithText(text.Replace(textToReplace, textToReplaceWith));
                }
            }

            if (tokensToReplace.Count > 0)
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