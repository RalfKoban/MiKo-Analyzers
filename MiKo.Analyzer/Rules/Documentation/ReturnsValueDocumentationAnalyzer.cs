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

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, string commentXml) => AnalyzeComment(symbol, symbol.GetDocumentationCommentXml());

        protected virtual bool ShallAnalyzeReturnType(ITypeSymbol returnType) => true;

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml)
        {
            var method = (IMethodSymbol)symbol;
            if (method.ReturnsVoid)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return AnalyzeReturnType(method, method.ReturnType, commentXml);
        }

        protected IEnumerable<Diagnostic> AnalyzeComment(IPropertySymbol symbol, string commentXml)
        {
            var returnType = symbol.GetReturnType();
            return returnType != null
                       ? AnalyzeReturnType(symbol, returnType, commentXml)
                       : Enumerable.Empty<Diagnostic>();
        }

        protected IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, string comment, string xmlTag, string[] phrase) => comment.StartsWithAny(phrase, StringComparison.Ordinal)
                                                                                                                                       ? Enumerable.Empty<Diagnostic>()
                                                                                                                                       : new[] { Issue(symbol, xmlTag, phrase[0]) };

        protected IEnumerable<Diagnostic> AnalyzePhrase(ISymbol symbol, string comment, string xmlTag, string[] phrase) => phrase.Any(_ => _.Equals(comment, StringComparison.Ordinal))
                                                                                                                               ? Enumerable.Empty<Diagnostic>()
                                                                                                                               : new[] { Issue(symbol, xmlTag, phrase[0]) };

        protected virtual IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string comment, string xmlTag) => Enumerable.Empty<Diagnostic>();

        private IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace())
            {
                return Enumerable.Empty<Diagnostic>();
            }

            if (!ShallAnalyzeReturnType(returnType))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return TryAnalyzeReturnType(owningSymbol, returnType, commentXml, Constants.XmlTag.Returns) ?? TryAnalyzeReturnType(owningSymbol, returnType, commentXml, Constants.XmlTag.Value) ?? Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> TryAnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string commentXml, string xmlTag)
        {
            List<Diagnostic> results = null;

            foreach (var comment in CommentExtensions.GetComments(commentXml, xmlTag).Where(_ => _ != null))
            {
                var findings = AnalyzeReturnType(owningSymbol, returnType, comment, xmlTag);
                if (findings.Any())
                {
                    if (results is null)
                    {
                        results = new List<Diagnostic>();
                    }

                    results.AddRange(findings);
                }
            }

            return results;
        }
    }
}