using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2044_InvalidSeeParameterInXmlAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2044";

        private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

        public MiKo_2044_InvalidSeeParameterInXmlAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml)
        {
            var method = (IMethodSymbol)symbol;

            if (method.Parameters.Length == 0)
                return Enumerable.Empty<Diagnostic>();

            var comment = commentXml.RemoveAll(Constants.Markers.Symbols);

            List<Diagnostic> findings = null;
            foreach (var parameter in method.Parameters)
            {
                InspectPhrase("<see cref", parameter, comment, ref findings);
                InspectPhrase("<seealso cref", parameter, comment, ref findings);
            }

            return findings ?? Enumerable.Empty<Diagnostic>();
        }

        private void InspectPhrase(string xmlTag, IParameterSymbol parameter, string commentXml, ref List<Diagnostic> findings)
        {
            var phrase = xmlTag + "=\"" + parameter.Name + "\"";
            if (commentXml.Contains(phrase, Comparison))
            {
                if (findings == null) findings = new List<Diagnostic>();
                findings.Add(ReportIssue(parameter, phrase + Constants.Comments.XmlElementEndingTag));
            }
        }
    }
}