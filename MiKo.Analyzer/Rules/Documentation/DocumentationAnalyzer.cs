using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class DocumentationAnalyzer : Analyzer
    {
        protected DocumentationAnalyzer(string diagnosticId, SymbolKind symbolKind) : base(nameof(Documentation), diagnosticId, symbolKind)
        {
        }

        protected static IEnumerable<string> GetStartingPhrases(ITypeSymbol symbolReturnType, string[] startingPhrases)
        {
            var returnType = symbolReturnType.ToString();

            var returnTypeFullyQualified = symbolReturnType.FullyQualifiedName();
            if (returnTypeFullyQualified.Contains('.') is false)
            {
                returnTypeFullyQualified = symbolReturnType.FullyQualifiedName(false);
            }

            symbolReturnType.TryGetGenericArgumentCount(out var count);
            if (count <= 0)
            {
                return Enumerable.Empty<string>()
                                 .Concat(startingPhrases.Select(_ => string.Format(_, returnType)))
                                 .Concat(startingPhrases.Select(_ => string.Format(_, returnTypeFullyQualified)));
            }

            var ts = symbolReturnType.GetGenericArgumentsAsTs();

            var length = returnType.IndexOf('<'); // just until the first one

            var returnTypeWithTs = returnType.Substring(0, length) + "{" + ts + "}";
            var returnTypeWithGenericCount = returnType.Substring(0, length) + '`' + count;

            return Enumerable.Empty<string>()
                             .Concat(startingPhrases.Select(_ => string.Format(_, returnTypeWithTs))) // for the phrases to show to the user
                             .Concat(startingPhrases.Select(_ => string.Format(_, returnTypeWithGenericCount))); // for the real check
        }

        protected virtual bool ShallAnalyze(INamedTypeSymbol symbol) => true;

        protected sealed override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => ShallAnalyze(symbol)
                                                                                                      ? AnalyzeType(symbol, symbol.GetDocumentationCommentXml())
                                                                                                      : Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, string commentXml) => AnalyzeComment(symbol, commentXml);

        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => true;

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol) => ShallAnalyze(symbol)
                                                                                                     ? AnalyzeMethod(symbol, symbol.GetDocumentationCommentXml())
                                                                                                     : Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, string commentXml) => AnalyzeComment(symbol, commentXml);

        protected virtual bool ShallAnalyze(IEventSymbol symbol) => true;

        protected sealed override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol) => ShallAnalyze(symbol)
                                                                                                   ? AnalyzeEvent(symbol, symbol.GetDocumentationCommentXml())
                                                                                                   : Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, string commentXml) => AnalyzeComment(symbol, commentXml);

        protected virtual bool ShallAnalyze(IFieldSymbol symbol) => true;

        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol) => ShallAnalyze(symbol)
                                                                                                   ? AnalyzeField(symbol, symbol.GetDocumentationCommentXml())
                                                                                                   : Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, string commentXml) => AnalyzeComment(symbol, commentXml);

        protected virtual bool ShallAnalyze(IPropertySymbol symbol) => true;

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol) => ShallAnalyze(symbol)
                                                                                                         ? AnalyzeProperty(symbol, symbol.GetDocumentationCommentXml())
                                                                                                         : Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, string commentXml) => AnalyzeComment(symbol, commentXml);

        protected virtual IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => Enumerable.Empty<Diagnostic>();
    }
}