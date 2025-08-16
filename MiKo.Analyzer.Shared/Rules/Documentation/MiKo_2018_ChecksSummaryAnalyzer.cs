using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2018_ChecksSummaryAnalyzer : SummaryStartDocumentationAnalyzer
    {
        public const string Id = "MiKo_2018";

        private const string StartingPhrase = Constants.Comments.DeterminesWhetherPhrase;

        private static readonly string[] WrongPhrases = { "Check ", "Checks ", "Test ", "Tests ", "Determines if " };

        public MiKo_2018_ChecksSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ConsiderEmptyTextAsIssue(ISymbol symbol) => symbol is IMethodSymbol method && method.ReturnType.IsBoolean();

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            switch (symbol)
            {
                case INamedTypeSymbol type:
                    return type.IsNamespace is false && type.IsEnum() is false;

                case IMethodSymbol _:
                case IPropertySymbol _:
                    return true;

                default:
                    return false;
            }
        }

        protected override Diagnostic NonTextStartIssue(ISymbol symbol, SyntaxNode node) => null; // this is no issue as we do not start with any word

        protected override Diagnostic TextStartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, StartingPhrase);

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                  DocumentationCommentTriviaSyntax comment,
                                                                  ISymbol symbol,
                                                                  IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                  Lazy<string> commentXml,
                                                                  Lazy<string[]> summaries)
        {
            List<Diagnostic> issues = null;

            for (int index = 0, count = summaryXmls.Count; index < count; index++)
            {
                var summaryXml = summaryXmls[index];

                if (symbol is ITypeSymbol && summaryXml.GetTextTrimmed().IsNullOrEmpty())
                {
                    // do not report for empty types
                    continue;
                }

                var issue = AnalyzeTextStart(symbol, summaryXml);

                if (issue is null)
                {
                    continue;
                }

                if (issues is null)
                {
                    issues = new List<Diagnostic>(1);
                }

                issues.Add(issue);
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            comparison = StringComparison.OrdinalIgnoreCase;

            var trimmedSummary = valueText.AsCachedBuilder()
                                          .Without(Constants.Comments.AsynchronouslyStartingPhrase) // skip over async starting phrase
                                          .Without(Constants.Comments.RecursivelyStartingPhrase) // skip over recursively starting phrase
                                          .Without(',') // skip over first comma
                                          .TrimmedStart()
                                          .ToStringAndRelease();

            foreach (var wrongPhrase in WrongPhrases)
            {
                if (trimmedSummary.StartsWith(wrongPhrase, comparison))
                {
                    problematicText = wrongPhrase.TrimEnd();

                    return true;
                }
            }

            problematicText = null;

            return false;
        }
    }
}