using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ReturnsValueDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected ReturnsValueDocumentationAnalyzer(string diagnosticId) : base(diagnosticId, SymbolKind.Method)
        {
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);
            Initialize(context, SymbolKind.Property);
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, string commentXml) => AnalyzeReturnTypes(symbol, commentXml);

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, string commentXml) => AnalyzeReturnTypes(symbol.GetMethod, symbol.GetDocumentationCommentXml());

        protected virtual bool ShallAnalyzeReturnType(ITypeSymbol returnType) => true;

        protected virtual IEnumerable<Diagnostic> AnalyzeReturnType(IMethodSymbol method, string comment, string xmlTag) => Enumerable.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeReturnTypes(IMethodSymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();
            if (symbol.ReturnsVoid) return Enumerable.Empty<Diagnostic>();

            if (!ShallAnalyzeReturnType(symbol.ReturnType)) return Enumerable.Empty<Diagnostic>();

            return TryAnalyzeReturnTypes(commentXml, symbol, "returns") ?? TryAnalyzeReturnTypes(commentXml, symbol, "value") ?? Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> TryAnalyzeReturnTypes(string commentXml, IMethodSymbol method, string xmlTag)
        {
            List<Diagnostic> results = null;

            foreach (var comment in GetComments(commentXml, xmlTag).Where(_ => _ != null))
            {
                var findings = AnalyzeReturnType(method, comment, xmlTag);
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
                                                                                                                                    : new[] { ReportIssue(method, method.Name, xmlTag, phrase[0]) };

        protected IEnumerable<Diagnostic> AnalyzePhrase(IMethodSymbol method, string comment, string xmlTag, string[] phrase) => phrase.Any(_ => _.Equals(comment, StringComparison.Ordinal))
                                                                                                                                    ? Enumerable.Empty<Diagnostic>()
                                                                                                                                    : new[] { ReportIssue(method, method.Name, xmlTag, phrase[0]) };
    }
}