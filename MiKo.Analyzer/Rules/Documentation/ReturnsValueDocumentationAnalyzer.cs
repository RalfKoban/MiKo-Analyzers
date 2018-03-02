using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ReturnsValueDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected ReturnsValueDocumentationAnalyzer(string diagnosticId) : base(diagnosticId, SymbolKind.Method) // TODO: what about properties ???
        {
        }
        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, string commentXml) => AnalyzeReturnValues(symbol, commentXml);

        protected virtual bool ShallAnalyzeReturnValue(ITypeSymbol returnValue) => true;

        protected virtual IEnumerable<Diagnostic> AnalyzeReturnValue(IMethodSymbol method, string comment, string xmlTag) => Enumerable.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeReturnValues(IMethodSymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();
            if (symbol.ReturnsVoid) return Enumerable.Empty<Diagnostic>();

            if (!ShallAnalyzeReturnValue(symbol.ReturnType)) return Enumerable.Empty<Diagnostic>();

            return TryAnalyzeReturnValues(commentXml, symbol, "returns") ?? TryAnalyzeReturnValues(commentXml, symbol, "value") ?? Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> TryAnalyzeReturnValues(string commentXml, IMethodSymbol method, string xmlTag)
        {
            List<Diagnostic> results = null;

            foreach (var comment in GetComments(commentXml, xmlTag).Where(_ => _ != null))
            {
                var findings = AnalyzeReturnValue(method, comment, xmlTag);
                if (findings.Any())
                {
                    if (results == null) results = new List<Diagnostic>();
                    results.AddRange(findings);
                }
            }

            return results;
        }

        protected IEnumerable<Diagnostic> AnalyzeStartingPhrase(IMethodSymbol method, string comment, string xmlTag, string[] phrase) => comment.StartsWithAny(StringComparison.Ordinal, phrase)
                                                                                                                                    ? Enumerable.Empty<Diagnostic>()
                                                                                                                                    : new[] { ReportIssue(method, method.Name, xmlTag, phrase.ConcatenatedWith(", ")) };
    }
}