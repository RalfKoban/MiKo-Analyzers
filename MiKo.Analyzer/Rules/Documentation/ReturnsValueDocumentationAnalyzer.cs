using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ReturnsValueDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected ReturnsValueDocumentationAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, string commentXml) => AnalyzeReturnType(symbol, commentXml);

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, string commentXml) => AnalyzeReturnType(symbol, symbol.GetDocumentationCommentXml());

        protected virtual bool ShallAnalyzeReturnType(ITypeSymbol returnType) => true;

        protected virtual IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string comment, string xmlTag) => Enumerable.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeReturnType(IMethodSymbol symbol, string commentXml)
        {
            if (symbol.ReturnsVoid) return Enumerable.Empty<Diagnostic>();

            return AnalyzeReturnType(symbol, symbol.ReturnType, commentXml);
        }

        private IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();

            if (!ShallAnalyzeReturnType(returnType)) return Enumerable.Empty<Diagnostic>();

            return TryAnalyzeReturnType(owningSymbol, returnType, commentXml, Constants.XmlTag.Returns) ?? TryAnalyzeReturnType(owningSymbol, returnType, commentXml, Constants.XmlTag.Value) ?? Enumerable.Empty<Diagnostic>();
        }

        protected IEnumerable<Diagnostic> AnalyzeReturnType(IPropertySymbol symbol, string commentXml)
        {
            if (symbol.GetMethod != null) return AnalyzeReturnType(symbol, symbol.GetMethod.ReturnType, commentXml);
            if (symbol.SetMethod != null) return AnalyzeReturnType(symbol, symbol.SetMethod.Parameters[0].Type, commentXml);

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> TryAnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string commentXml, string xmlTag)
        {
            List<Diagnostic> results = null;

            foreach (var comment in GetComments(commentXml, xmlTag).Where(_ => _ != null))
            {
                var findings = AnalyzeReturnType(owningSymbol, returnType, comment, xmlTag);
                if (findings.Any())
                {
                    if (results == null) results = new List<Diagnostic>();
                    results.AddRange(findings);
                }
            }

            return results;
        }

        protected IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, string comment, string xmlTag, string[] phrase) => comment.StartsWithAny(StringComparison.Ordinal, phrase)
                                                                                                                                       ? Enumerable.Empty<Diagnostic>()
                                                                                                                                       : new[] { ReportIssue(symbol, symbol.Name, xmlTag, phrase[0]) };

        protected IEnumerable<Diagnostic> AnalyzePhrase(ISymbol symbol, string comment, string xmlTag, string[] phrase) => phrase.Any(_ => _.Equals(comment, StringComparison.Ordinal))
                                                                                                                               ? Enumerable.Empty<Diagnostic>()
                                                                                                                               : new[] { ReportIssue(symbol, symbol.Name, xmlTag, phrase[0]) };
    }
}