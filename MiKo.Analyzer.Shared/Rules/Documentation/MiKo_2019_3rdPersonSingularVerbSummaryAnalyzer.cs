using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]

    public sealed class MiKo_2019_3rdPersonSingularVerbSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2019";

        public MiKo_2019_3rdPersonSingularVerbSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property);

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsNamespace is false && symbol.IsEnum() is false && symbol.IsException() is false && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var summaryXmls = comment.GetSummaryXmls();

            foreach (var summaryXml in summaryXmls)
            {
                yield return FindIssue(symbol, summaryXml);
            }
        }

        private Diagnostic FindIssue(ISymbol symbol, SyntaxNode summaryXml)
        {
            var descendantNodes = summaryXml.DescendantNodes();

            foreach (var node in descendantNodes)
            {
                switch (node)
                {
                    case XmlElementStartTagSyntax startTag:
                    {
                        var tagName = startTag.GetName();

                        switch (tagName)
                        {
                            case Constants.XmlTag.Summary:
                            case Constants.XmlTag.Para:
                                continue; // skip over the start tag and name syntax

                            default:
                                return Issue(node); // it's no text, so it must be something different
                        }
                    }

                    case XmlNameSyntax _:
                    case XmlElementSyntax e when e.GetName() == Constants.XmlTag.Para:
                    case XmlEmptyElementSyntax ee when ee.GetName() == Constants.XmlTag.Para:
                        continue; // skip over the start tag and name syntax

                    case XmlTextSyntax text:
                    {
                        // report the location of the first word via the corresponding text token
                        foreach (var textToken in text.TextTokens)
                        {
                            var summary = textToken.ValueText;

                            if (summary.IsNullOrWhiteSpace())
                            {
                                // we found the first but empty /// line, so ignore it
                                continue;
                            }

                            // we found some text
                            var firstWord = new StringBuilder(summary)
                                            .Without(Constants.Comments.AsynchrounouslyStartingPhrase) // skip over async starting phrase
                                            .Without(Constants.Comments.RecursivelyStartingPhrase) // skip over recursively starting phrase
                                            .Without(",") // skip over first comma
                                            .ToString()
                                            .FirstWord();

                            if (Verbalizer.IsThirdPersonSingularVerb(firstWord))
                            {
                                // it's a 3rd person singular verb, so we quit
                                return null;
                            }

                            // it's no 3rd person singular verb, so we have an issue
                            var position = summary.IndexOf(firstWord, StringComparison.Ordinal);
                            var start = textToken.SpanStart + position; // find start position of first word for underlining
                            var end = start + firstWord.Length; // find end position of first word
                            var location = CreateLocation(textToken, start, end);

                            return Issue(symbol.Name, location);
                        }

                        // we found a completely empty /// line, so ignore it
                        continue;
                    }

                    default:
                        return Issue(node); // it's no text, so it must be something different
                }
            }

            // nothing to report
            return null;
        }
    }
}