using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2079_PropertiesDocumentationShouldNotStateObviousAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2079";

        private static readonly string[] ObviousStartingPhrases =
                                                                  {
                                                                      "Gets ",
                                                                      "Sets ",
                                                                      "Gets/Sets ",
                                                                      "Gets or Sets ",
                                                                      "Gets Or Sets ",
                                                                      "Gets OR Sets ",
                                                                      "Gets and Sets ",
                                                                      "Gets And Sets ",
                                                                      "Gets AND Sets ",
                                                                      "Get ",
                                                                      "Set ",
                                                                      "Get/Set ",
                                                                      "Get or Set ",
                                                                      "Get Or Set ",
                                                                      "Get OR Set ",
                                                                      "Get and Set ",
                                                                      "Get And Set ",
                                                                      "Get AND Set ",
                                                                  };

        private static readonly string[] ContinuationPhrases = { "flag", "Flag", "field", "Field" };

        public MiKo_2079_PropertiesDocumentationShouldNotStateObviousAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol.Kind is SymbolKind.Property;

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                  DocumentationCommentTriviaSyntax comment,
                                                                  ISymbol symbol,
                                                                  IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                  Lazy<string> commentXml,
                                                                  Lazy<string[]> summaries)
        {
            var symbolName = symbol.Name;
            var obviousComments = GetObviousComments(symbolName);

            List<Diagnostic> issues = null;

            for (int index = 0, count = summaryXmls.Count; index < count; index++)
            {
                var summary = summaryXmls[index];
                var trimmed = summary.GetTextTrimmed();

                if (obviousComments.Contains(trimmed))
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    // we have an issue
                    issues.Add(Issue(symbolName, comment));
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }

        private static HashSet<string> GetObviousComments(string symbolName)
        {
            var articles = new[] { string.Empty, "the " };

            var result = new HashSet<string>();

            foreach (var startingPhrase in ObviousStartingPhrases)
            {
                foreach (var article in articles)
                {
                    var obviousPhrase = startingPhrase + article + symbolName;

                    result.Add(obviousPhrase);
                    result.Add(obviousPhrase + ".");

                    foreach (var continuation in ContinuationPhrases)
                    {
                        var phrase = obviousPhrase + " " + continuation;

                        result.Add(phrase);
                        result.Add(phrase + ".");
                    }
                }
            }

            return result;
        }
    }
}