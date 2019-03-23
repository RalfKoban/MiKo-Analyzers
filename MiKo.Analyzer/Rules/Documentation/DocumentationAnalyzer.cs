using System;
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

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => ShallAnalyzeType(symbol)
                                                                                               ? AnalyzeType(symbol, symbol.GetDocumentationCommentXml())
                                                                                               : Enumerable.Empty<Diagnostic>();

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol) => ShallAnalyzeMethod(symbol)
                                                                                              ? AnalyzeMethod(symbol, symbol.GetDocumentationCommentXml())
                                                                                              : Enumerable.Empty<Diagnostic>();


        protected override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol) => ShallAnalyzeEvent(symbol)
                                                                                            ? AnalyzeEvent(symbol, symbol.GetDocumentationCommentXml())
                                                                                            : Enumerable.Empty<Diagnostic>();

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol) => ShallAnalyzeField(symbol)
                                                                                            ? AnalyzeField(symbol, symbol.GetDocumentationCommentXml())
                                                                                            : Enumerable.Empty<Diagnostic>();

        protected override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol) => ShallAnalyzeProperty(symbol)
                                                                                                  ? AnalyzeProperty(symbol, symbol.GetDocumentationCommentXml())
                                                                                                  : Enumerable.Empty<Diagnostic>();

        protected virtual bool ShallAnalyzeType(INamedTypeSymbol symbol) => true;

        protected virtual bool ShallAnalyzeMethod(IMethodSymbol symbol) => true;

        protected virtual bool ShallAnalyzeEvent(IEventSymbol symbol) => true;

        protected virtual bool ShallAnalyzeField(IFieldSymbol symbol) => true;

        protected virtual bool ShallAnalyzeProperty(IPropertySymbol symbol) => true;

        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, string commentXml) => AnalyzeComment(symbol, commentXml);

        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, string commentXml) => AnalyzeComment(symbol, commentXml);

        protected virtual IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, string commentXml) => AnalyzeComment(symbol, commentXml);

        protected virtual IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, string commentXml) => AnalyzeComment(symbol, commentXml);

        protected virtual IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, string commentXml) => AnalyzeComment(symbol, commentXml);

        protected virtual IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => Enumerable.Empty<Diagnostic>();

        protected static bool TryGetGenericArgumentType(ITypeSymbol symbol, out ITypeSymbol genericArgument, int index = 0)
        {
            genericArgument = null;

            if (symbol is INamedTypeSymbol namedType && namedType.TypeArguments.Length >= index + 1)
                genericArgument = namedType.TypeArguments[index];

            return genericArgument != null;
        }

        protected static bool TryGetGenericArgumentCount(ITypeSymbol symbol, out int index)
        {
            index = 0;
            if (symbol is INamedTypeSymbol namedType) index = namedType.TypeArguments.Length;
            return index > 0;
        }

        protected static string GetGenericArgumentsAsTs(ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol namedType)
            {
                var count = namedType.TypeArguments.Length;
                switch (count)
                {
                    case 0: return string.Empty;
                    case 1: return "T";
                    case 2: return "T1,T2";
                    case 3: return "T1,T2,T3";
                    case 4: return "T1,T2,T3,T4";
                    case 5: return "T1,T2,T3,T4,T5";
                    default: return Enumerable.Range(1, count).Select(_ => "T" + _).ConcatenatedWith(",");
                }
            }

            return string.Empty;
        }

        protected static IEnumerable<string> GetStartingPhrases(ITypeSymbol symbolReturnType, string[] startingPhrases)
        {
            var returnType = symbolReturnType.ToString();

            TryGetGenericArgumentCount(symbolReturnType, out var count);
            if (count <= 0) return startingPhrases.Select(_ => string.Format(_, returnType));

            var ts = GetGenericArgumentsAsTs(symbolReturnType);

            var length = returnType.IndexOf('<'); // just until the first one

            var returnTypeWithTs = returnType.Substring(0, length) + "{" + ts + "}";
            var returnTypeWithGenericCount = returnType.Substring(0, length) + '`' + count;

            return Enumerable.Empty<string>()
                             .Concat(startingPhrases.Select(_ => string.Format(_, returnTypeWithTs))) // for the phrases to show to the user
                             .Concat(startingPhrases.Select(_ => string.Format(_, returnTypeWithGenericCount))); // for the real check
        }
    }
}