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

        private const string Replacement = "to";

        private const string CanPhrase = "can be used to";
        private const string CanReplacement = "allows to";
        private const string UsedToPhrase = "used to";
        private const string IsUsedToPhrase = "is used to";
        private const string AreUsedToPhrase = "are used to";

        private static readonly string[] Phrases =
            {
                "that is used to",
                "that it is used to",
                "that are used to",
                "that shall be used to",
                "which is used to",
                "which are used to",
                "which shall be used to",
            };

        public MiKo_2218_DocumentationShouldNotContainUsedToAnalyzer() : base(Id)
        {
        }

        internal static XmlTextSyntax GetBetterText(XmlTextSyntax node)
        {
            var tokens = node.TextTokens;

            var textSoFar = string.Empty;

            var tokensToReplace = new Dictionary<SyntaxToken, SyntaxToken>();

            foreach (var token in tokens)
            {
                var text = token.Text;

                if (text.Length <= Environment.NewLine.Length && text.IsNullOrWhiteSpace())
                {
                    // do not bother with only empty text
                    continue;
                }

                var result = Phrases.Aggregate(text, (current, phrase) => current.Replace(phrase, Replacement));

                result = ReplaceSpecialPhrase(IsUsedToPhrase, result, Verbalizer.MakeThirdPersonSingularVerb);
                result = ReplaceSpecialPhrase(AreUsedToPhrase, result, _ => _);
                result = ReplaceSpecialPhrase(UsedToPhrase.ToUpperCaseAt(0), result, _ => Verbalizer.MakeThirdPersonSingularVerb(_).ToUpperCaseAt(0));

                result = result.Replace(CanPhrase.ToUpperCaseAt(0), CanReplacement.ToUpperCaseAt(0));

                // special situation for <param> texts
                var belowParam = node.Ancestors<XmlElementSyntax>().Any(_ => _.GetName() == Constants.XmlTag.Param);
                if (belowParam)
                {
                    // let's find out if we have the first sentence
                    var canIndex = result.IndexOf(CanPhrase, StringComparison.Ordinal);
                    if (canIndex != -1)
                    {
                        var firstSentence = textSoFar.LastIndexOf('.') == -1 && canIndex < result.IndexOf('.');
                        if (firstSentence)
                        {
                            // we seem to be in the first sentence
                            result = result.Replace(CanPhrase, Replacement);
                        }
                    }
                }

                result = result.Replace(CanPhrase, CanReplacement);
                result = result.Replace(UsedToPhrase, Replacement);

                textSoFar += result;

                if (result.Length != text.Length)
                {
                    tokensToReplace[token] = token.WithText(result);
                }
            }

            if (tokensToReplace.Any())
            {
                return node.ReplaceTokens(tokensToReplace.Keys, (original, rewritten) => tokensToReplace[original]);
            }

            return node;
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            var locations = new List<Location>();

            var issues = AnalyzeCommentXml(symbol);

            foreach (var issue in issues)
            {
                // filter issues within other issues as those are somehow duplicates due to the phrases to search for
                if (locations.None(__ => __.Contains(issue.Location)))
                {
                    locations.Add(issue.Location);

                    yield return issue;
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeCommentXml(ISymbol symbol)
        {
            foreach (var token in symbol.GetDocumentationCommentTriviaSyntax().DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens))
            {
                foreach (var location in GetAllLocations(token, Phrases))
                {
                    yield return Issue(symbol.Name, location, location.GetText(), Replacement);
                }

                foreach (var issue in AnalyzeForSpecialPhrase(symbol, token, IsUsedToPhrase, Verbalizer.MakeThirdPersonSingularVerb))
                {
                    yield return issue;
                }

                foreach (var issue in AnalyzeForSpecialPhrase(symbol, token, AreUsedToPhrase, Verbalizer.MakeThirdPersonSingularVerb))
                {
                    yield return issue;
                }

                foreach (var issue in AnalyzeForSpecialPhrase(symbol, token, UsedToPhrase.ToUpperCaseAt(0), _ => Verbalizer.MakeThirdPersonSingularVerb(_).ToUpperCaseAt(0)))
                {
                    yield return issue;
                }

                foreach (var location in GetAllLocations(token, CanPhrase.ToUpperCaseAt(0)))
                {
                    yield return Issue(symbol.Name, location, location.GetText(), CanReplacement.ToUpperCaseAt(0));
                }

                foreach (var location in GetAllLocations(token, CanPhrase))
                {
                    yield return Issue(symbol.Name, location, location.GetText(), CanReplacement);
                }

                foreach (var issue in AnalyzeForSpecialPhrase(symbol, token, IsUsedToPhrase, Verbalizer.MakeThirdPersonSingularVerb))
                {
                    yield return issue;
                }

                foreach (var location in GetAllLocations(token, UsedToPhrase))
                {
                    yield return Issue(symbol.Name, location, location.GetText(), Replacement);
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeForSpecialPhrase(ISymbol symbol, SyntaxToken token, string startingPhrase, Func<string, string> replacementCallback)
        {
            var makeUpper = startingPhrase[0].IsUpperCase();

            foreach (var location in GetAllLocations(token, startingPhrase))
            {
                var start = location.SourceSpan.Start;
                var index = start - token.SpanStart + startingPhrase.Length;

                var textAfterStartingPhrase = token.ValueText.Substring(index);
                var nextWord = textAfterStartingPhrase.FirstWord();

                // let's find the end of the next word in the source code (but keep in mind the offset of the starting phrase)
                var offset = start + startingPhrase.Length;
                var end = textAfterStartingPhrase.IndexOf(nextWord, StringComparison.Ordinal) + nextWord.Length + offset;

                var finalLocation = CreateLocation(token, start, end);

                var replacement = replacementCallback(nextWord);
                if (makeUpper)
                {
                    replacement = replacement.ToUpperCaseAt(0);
                }

                yield return Issue(symbol.Name, finalLocation, finalLocation.GetText(), replacement);
            }
        }

        private static string ReplaceSpecialPhrase(string phrase, string text, Func<string, string> replacementCallback)
        {
            var result = text;

            while (true)
            {
                if (result.Length == 0)
                {
                    // no text left to search within
                    break;
                }

                var index = result.IndexOf(phrase, StringComparison.Ordinal);

                if (index < 0)
                {
                    // nothing found anymore
                    break;
                }

                var nextWord = result.Substring(index + phrase.Length).FirstWord();
                var nextWordEnd = result.IndexOf(nextWord, StringComparison.Ordinal) + nextWord.Length;

                var replaceText = result.Substring(index, nextWordEnd - index);
                var replacement = replacementCallback(nextWord);

                result = result.Replace(replaceText, replacement);
            }

            return result;
        }
    }
}