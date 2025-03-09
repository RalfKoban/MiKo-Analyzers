using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2071_EnumMethodSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2071";

        private static readonly string[] ContinuationPhrases = { "whether ", "if " };

        private static readonly string[] BooleanPhrases = new[] { " indicating ", " indicates ", " indicate " }.SelectMany(_ => ContinuationPhrases, string.Concat).ToArray();

        private static readonly int MinimumPhraseLength = BooleanPhrases.Min(_ => _.Length);

        public MiKo_2071_EnumMethodSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            switch (symbol)
            {
                case IMethodSymbol method:
                    return method.ReturnType.IsEnum();

                case IPropertySymbol property:
                    return property.GetReturnType()?.IsEnum() is true;

                default:
                    return false;
            }
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                  DocumentationCommentTriviaSyntax comment,
                                                                  ISymbol symbol,
                                                                  IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                  Lazy<string> commentXml,
                                                                  Lazy<IReadOnlyCollection<string>> summaries)
        {
            var textTokens = comment.GetXmlTextTokens();
            var textTokensCount = textTokens.Count;

            if (textTokensCount == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var text = textTokens.GetTextTrimmedWithParaTags();

            if (text.ContainsAny(BooleanPhrases, StringComparison.Ordinal) is false)
            {
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> issues = null;

            for (var i = 0; i < textTokensCount; i++)
            {
                var token = textTokens[i];

                if (token.ValueText.Length < MinimumPhraseLength)
                {
                    continue;
                }

                const int Offset = 1; // we do not want to underline the first and last char

                var locations = GetAllLocations(token, BooleanPhrases, StringComparison.Ordinal, Offset, Offset);
                var locationsCount = locations.Count;

                if (locationsCount > 0)
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(locationsCount);
                    }

                    for (var index = 0; index < locationsCount; index++)
                    {
                        issues.Add(Issue(locations[index]));
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }
    }
}