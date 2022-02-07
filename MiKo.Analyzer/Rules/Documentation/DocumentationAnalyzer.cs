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

        protected static Location GetLocation(SyntaxToken textToken, string value, StringComparison comparison = StringComparison.Ordinal)
        {
            var syntaxTree = textToken.SyntaxTree;
            var position = textToken.ValueText.IndexOf(value, comparison);

            if (position == -1)
            {
                return null;
            }

            var start = textToken.SpanStart + position; // find start position for underlining
            var end = start + value.Length; // find end position

            return Location.Create(syntaxTree, TextSpan.FromBounds(start, end));
        }

        protected static IEnumerable<Location> GetLocations(SyntaxToken textToken, string value, StringComparison comparison = StringComparison.Ordinal)
        {
            var syntaxTree = textToken.SyntaxTree;
            var positions = textToken.ValueText.AllIndexesOf(value, comparison);

            foreach (var position in positions)
            {
                var start = textToken.SpanStart + position; // find start position for underlining
                var end = start + value.Length; // find end position

                yield return Location.Create(syntaxTree, TextSpan.FromBounds(start, end));
            }
        }

        protected static IEnumerable<Location> GetLocations(SyntaxToken textToken, IEnumerable<string> values, StringComparison comparison = StringComparison.Ordinal)
        {
            var syntaxTree = textToken.SyntaxTree;

            foreach (var value in values)
            {
                var positions = textToken.ValueText.AllIndexesOf(value, comparison);

                foreach (var position in positions)
                {
                    var start = textToken.SpanStart + position; // find start position for underlining
                    var end = start + value.Length; // find end position

                    yield return Location.Create(syntaxTree, TextSpan.FromBounds(start, end));
                }
            }
        }

        protected static Location GetLocation(SyntaxTrivia trivia, string value, StringComparison comparison = StringComparison.Ordinal)
        {
            var syntaxTree = trivia.SyntaxTree;
            var text = trivia.ToFullString();

            var position = text.IndexOf(value, comparison);

            if (position == -1)
            {
                return null;
            }

            var start = trivia.SpanStart + position; // find start position for underlining
            var end = start + value.Length; // find end position

            return Location.Create(syntaxTree, TextSpan.FromBounds(start, end));
        }

        protected static IEnumerable<Location> GetLocations(SyntaxTrivia trivia, string value, StringComparison comparison = StringComparison.Ordinal)
        {
            var syntaxTree = trivia.SyntaxTree;
            var text = trivia.ToFullString();

            var positions = text.AllIndexesOf(value, comparison);

            foreach (var position in positions)
            {
                var start = trivia.SpanStart + position; // find start position for underlining
                var end = start + value.Length; // find end position

                yield return Location.Create(syntaxTree, TextSpan.FromBounds(start, end));
            }
        }

        protected static IEnumerable<Location> GetLocations(SyntaxTrivia trivia, IEnumerable<string> values, StringComparison comparison = StringComparison.Ordinal)
        {
            var syntaxTree = trivia.SyntaxTree;
            var text = trivia.ToFullString();

            foreach (var value in values)
            {
                var positions = text.AllIndexesOf(value, comparison);

                foreach (var position in positions)
                {
                    var start = trivia.SpanStart + position; // find start position for underlining
                    var end = start + value.Length; // find end position

                    yield return Location.Create(syntaxTree, TextSpan.FromBounds(start, end));
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
    }
}