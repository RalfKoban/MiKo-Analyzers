using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2045_InvalidParameterReferenceInSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2045";

        private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

        public MiKo_2045_InvalidParameterReferenceInSummaryAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var method = (IMethodSymbol)symbol;

            if (method.Parameters.Length == 0)
                return Enumerable.Empty<Diagnostic>();

            List<Diagnostic> findings = null;
            foreach (var summary in summaries)
            {
                var comment = summary.RemoveAll(Constants.SymbolMarkers);

                foreach (var parameter in method.Parameters)
                {
                    InspectPhrases(parameter, comment, ref findings);
                }
            }

            return findings ?? Enumerable.Empty<Diagnostic>();
        }

        private static IEnumerable<string> CreatePhrases(string parameterName) => new[]
                                                                                      {
                                                                                          string.Intern("<param name=\"" + parameterName + "\""),
                                                                                          string.Intern("<paramref name=\"" + parameterName + "\""),
                                                                                          string.Intern("'" + parameterName + "'"),
                                                                                          string.Intern("\"" + parameterName + "\""),
                                                                                      };

        private void InspectPhrases(IParameterSymbol parameter, string commentXml, ref List<Diagnostic> findings)
        {
            var phrases = CreatePhrases(parameter.Name);

            foreach (var phrase in phrases
                                   .Where(_ => commentXml.Contains(_, Comparison))
                                   .Select(_ => _.StartsWith("<", StringComparison.OrdinalIgnoreCase) ? _ + Constants.Comments.XmlElementEndingTag : _))
            {
                if (findings == null) findings = new List<Diagnostic>();
                findings.Add(ReportIssue(parameter, phrase));
            }
        }
    }
}