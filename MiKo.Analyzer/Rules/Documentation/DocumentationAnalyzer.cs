using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class DocumentationAnalyzer : Analyzer
    {
        protected DocumentationAnalyzer(string diagnosticId, SymbolKind symbolKind) : base(nameof(Documentation), diagnosticId, symbolKind)
        {
        }

        protected static Location GetFirstLocation(SyntaxToken textToken, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return CreateLocation(value, textToken.SyntaxTree, textToken.SpanStart, textToken.ValueText.IndexOf(value, comparison), startOffset, endOffset);
        }

        protected static Location GetLastLocation(SyntaxToken textToken, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return CreateLocation(value, textToken.SyntaxTree, textToken.SpanStart, textToken.ValueText.LastIndexOf(value, comparison), startOffset, endOffset);
        }

        protected static IEnumerable<Location> GetAllLocations(SyntaxToken textToken, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            var syntaxTree = textToken.SyntaxTree;
            var spanStart = textToken.SpanStart;
            var text = textToken.ValueText;

            if (text.Length <= 2 && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                yield break;
            }

            foreach (var position in text.AllIndexesOf(value, comparison))
            {
                var location = CreateLocation(value, syntaxTree, spanStart, position, startOffset, endOffset);
                if (location != null)
                {
                    yield return location;
                }
            }
        }

        protected static IEnumerable<Location> GetAllLocations(SyntaxToken textToken, IEnumerable<string> values, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            var syntaxTree = textToken.SyntaxTree;
            var spanStart = textToken.SpanStart;
            var text = textToken.ValueText;

            if (text.Length <= 2 && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                yield break;
            }

            foreach (var value in values)
            {
                foreach (var position in text.AllIndexesOf(value, comparison))
                {
                    var location = CreateLocation(value, syntaxTree, spanStart, position, startOffset, endOffset);
                    if (location != null)
                    {
                        yield return location;
                    }
                }
            }
        }

        protected static Location GetFirstLocation(SyntaxTrivia trivia, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return CreateLocation(value, trivia.SyntaxTree, trivia.SpanStart, trivia.ToFullString().IndexOf(value, comparison), startOffset, endOffset);
        }

        protected static Location GetLastLocation(SyntaxTrivia trivia, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return CreateLocation(value, trivia.SyntaxTree, trivia.SpanStart, trivia.ToFullString().LastIndexOf(value, comparison), startOffset, endOffset);
        }

        protected static IEnumerable<Location> GetAllLocations(SyntaxTrivia trivia, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            var syntaxTree = trivia.SyntaxTree;
            var spanStart = trivia.SpanStart;
            var text = trivia.ToFullString();

            if (text.Length <= 2 && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                yield break;
            }

            foreach (var position in text.AllIndexesOf(value, comparison))
            {
                var location = CreateLocation(value, syntaxTree, spanStart, position, startOffset, endOffset);
                if (location != null)
                {
                    yield return location;
                }
            }
        }

        protected static IEnumerable<Location> GetAllLocations(SyntaxTrivia trivia, IEnumerable<string> values, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            var syntaxTree = trivia.SyntaxTree;
            var spanStart = trivia.SpanStart;
            var text = trivia.ToFullString();

            if (text.Length <= 2 && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                yield break;
            }

            foreach (var value in values)
            {
                foreach (var position in text.AllIndexesOf(value, comparison))
                {
                    var location = CreateLocation(value, syntaxTree, spanStart, position, startOffset, endOffset);
                    if (location != null)
                    {
                        yield return location;
                    }
                }
            }
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

        protected virtual bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.GetDocumentationCommentTriviaSyntax() != null;

        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => symbol.GetDocumentationCommentTriviaSyntax() != null;

        protected virtual bool ShallAnalyze(IEventSymbol symbol) => symbol.GetDocumentationCommentTriviaSyntax() != null;

        protected virtual bool ShallAnalyze(IFieldSymbol symbol) => symbol.GetDocumentationCommentTriviaSyntax() != null;

        protected virtual bool ShallAnalyze(IPropertySymbol symbol) => symbol.GetDocumentationCommentTriviaSyntax() != null;

        protected sealed override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                               ? AnalyzeType(symbol, compilation, symbol.GetDocumentationCommentXml())
                                                                                                                               : Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation, string commentXml) => AnalyzeComment(symbol, compilation, commentXml);

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                              ? AnalyzeMethod(symbol, compilation, symbol.GetDocumentationCommentXml())
                                                                                                                              : Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, string commentXml) => AnalyzeComment(symbol, compilation, commentXml);

        protected sealed override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                            ? AnalyzeEvent(symbol, compilation, symbol.GetDocumentationCommentXml())
                                                                                                                            : Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation, string commentXml) => AnalyzeComment(symbol, compilation, commentXml);

        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                            ? AnalyzeField(symbol, compilation, symbol.GetDocumentationCommentXml())
                                                                                                                            : Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation, string commentXml) => AnalyzeComment(symbol, compilation, commentXml);

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                                  ? AnalyzeProperty(symbol, compilation, symbol.GetDocumentationCommentXml())
                                                                                                                                  : Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation, string commentXml) => AnalyzeComment(symbol, compilation, commentXml);

        protected virtual IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml) => Enumerable.Empty<Diagnostic>();

        private static Location CreateLocation(string value, SyntaxTree syntaxTree, int spanStart, int position, int startOffset, int endOffset)
        {
            if (position == -1)
            {
                return null;
            }

            var start = spanStart + position + startOffset; // find start position for underlining
            var end = start + value.Length - startOffset - endOffset; // find end position

            return Location.Create(syntaxTree, TextSpan.FromBounds(start, end));
        }
    }
}