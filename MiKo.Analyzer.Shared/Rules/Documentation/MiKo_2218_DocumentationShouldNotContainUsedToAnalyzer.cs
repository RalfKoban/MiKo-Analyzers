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

        internal static string GetBetterText(string text)
        {
            if (text.Length <= Environment.NewLine.Length && text.IsNullOrWhiteSpace())
            {
                return text;
            }

            var result = Phrases.Aggregate(text, (current, phrase) => current.Replace(phrase, Replacement));

            result = ReplaceSpecialPhrase(IsUsedToPhrase, result);
            result = ReplaceSpecialPhrase(UsedToPhrase.ToUpperCaseAt(0), result);

            result = result.Replace(CanPhrase, CanReplacement);
            result = result.Replace(CanPhrase.ToUpperCaseAt(0), CanReplacement.ToUpperCaseAt(0));
            result = result.Replace(UsedToPhrase, Replacement);

            return result;
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            foreach (var token in symbol.GetDocumentationCommentTriviaSyntax().DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens))
            {
                var locations = new List<Location>();

                foreach (var location in GetAllLocations(token, Phrases))
                {
                    locations.Add(location);

                    yield return Issue(symbol.Name, location, location.GetText(), Replacement);
                }

                foreach (var location in GetAllLocations(token, CanPhrase))
                {
                    locations.Add(location);

                    yield return Issue(symbol.Name, location, location.GetText(), CanReplacement);
                }

                foreach (var location in GetAllLocations(token, CanPhrase.ToUpperCaseAt(0)))
                {
                    locations.Add(location);

                    yield return Issue(symbol.Name, location, location.GetText(), CanReplacement.ToUpperCaseAt(0));
                }

                foreach (var issue in AnalyzeForSpecialStartupPhrase(symbol, token, IsUsedToPhrase).Where(_ => locations.None(__ => __.Contains(_.Location))))
                {
                    locations.Add(issue.Location);

                    yield return issue;
                }

                foreach (var issue in AnalyzeForSpecialStartupPhrase(symbol, token, UsedToPhrase, Replacement).Where(_ => locations.None(__ => __.Contains(_.Location))))
                {
                    locations.Add(issue.Location);

                    yield return issue;
                }

                foreach (var issue in AnalyzeForSpecialStartupPhrase(symbol, token, UsedToPhrase.ToUpperCaseAt(0)))
                {
                    yield return issue;
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeForSpecialStartupPhrase(ISymbol symbol, SyntaxToken token, string startingPhrase, string preferredReplacement = null)
        {
            var makeUpper = startingPhrase[0].IsUpperCase();

            foreach (var location in GetAllLocations(token, startingPhrase))
            {
                var index = location.SourceSpan.Start - token.SpanStart + startingPhrase.Length;
                var nextWord = token.ValueText.Substring(index).FirstWord();

                var replacement = preferredReplacement ?? GetReplacement(nextWord);
                if (makeUpper)
                {
                    replacement = replacement.ToUpperCaseAt(0);
                }

                yield return Issue(symbol.Name, location, location.GetText(), replacement);
            }
        }

        private static string GetReplacement(string firstWord) => Verbalizer.MakeThirdPersonSingularVerb(firstWord);

        private static string ReplaceSpecialPhrase(string phrase, string result)
        {
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

                var replacement = GetReplacement(nextWord);

                if (phrase[0].IsUpperCase())
                {
                    replacement = replacement.ToUpperCaseAt(0);
                }

                result = result.Replace(replaceText, replacement);
            }

            return result;
        }
    }
}