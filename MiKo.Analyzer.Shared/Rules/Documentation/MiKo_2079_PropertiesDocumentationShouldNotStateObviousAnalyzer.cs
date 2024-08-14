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

        public MiKo_2079_PropertiesDocumentationShouldNotStateObviousAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Property);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var summaries = comment.GetSummaryXmls();
            var symbolName = symbol.Name;

            var obviousComments = GetObviousComments(symbolName);

            foreach (var summary in summaries)
            {
                var trimmed = summary.GetTextTrimmed().ToString();

                if (obviousComments.Contains(trimmed))
                {
                    yield return Issue(symbolName, comment);
                }
            }
        }

        private static HashSet<string> GetObviousComments(string symbolName)
        {
            var result = new HashSet<string>();

            foreach (var startingPhrase in ObviousStartingPhrases)
            {
                var obviousPhrase = startingPhrase + symbolName;

                result.Add(obviousPhrase);
                result.Add(obviousPhrase + ".");
            }

            return result;
        }
    }
}