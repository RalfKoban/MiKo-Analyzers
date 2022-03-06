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

        private const string StartingPhrase = "Used to";

        private static readonly string[] Phrases =
            {
                "that is used to",
                "that are used to",
                "that shall be used to",
                "which is used to",
                "which are used to",
                "which shall be used to",
                "used to",
            };

        public MiKo_2218_DocumentationShouldNotContainUsedToAnalyzer() : base(Id)
        {
        }

        internal static string GetBetterText(string text) => Phrases.Aggregate(text, (current, phrase) => current.Replace(phrase, Replacement));

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            foreach (var token in symbol.GetDocumentationCommentTriviaSyntax().DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens))
            {
                foreach (var location in GetAllLocations(token, Phrases))
                {
                    yield return Issue(symbol.Name, location, location.GetText(), Replacement);
                }

                foreach (var location in GetAllLocations(token, StartingPhrase))
                {
                    var subText = token.ValueText.Substring(StartingPhrase.Length + 1);
                    var firstWord = subText.FirstWord();

                    yield return Issue(symbol.Name, location, location.GetText(), Verbalizer.MakeThirdPersonSingularVerb(firstWord));
                }
            }
        }
    }
}