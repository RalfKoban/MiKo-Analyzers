using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class DocumentationAnalyzer : Analyzer
    {
        protected static readonly string[] CodeTags = { Constants.XmlTag.Code, Constants.XmlTag.C };

        protected DocumentationAnalyzer(string diagnosticId, SymbolKind symbolKind) : base(nameof(Documentation), diagnosticId, symbolKind)
        {
        }

        /// <summary>
        /// Encapsulates the given terms with a space or parenthesis before and a delimiter character behind.
        /// </summary>
        protected static string[] GetWithDelimiters(params string[] values)
        {
            var result = new List<string>();

            foreach (var delimiter in Constants.Comments.Delimiters)
            {
                foreach (var phrase in values)
                {
                    result.Add(' ' + phrase + delimiter);
                    result.Add('(' + phrase + delimiter);
                }
            }

            return result.ToArray();
        }

        protected static Location GetFirstLocation(SyntaxToken textToken, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return CreateLocation(value, textToken.SyntaxTree, textToken.SpanStart, textToken.ValueText.IndexOf(value, comparison), startOffset, endOffset);
        }

        protected static Location GetFirstLocation(SyntaxTrivia trivia, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return CreateLocation(value, trivia.SyntaxTree, trivia.SpanStart, trivia.ToFullString().IndexOf(value, comparison), startOffset, endOffset);
        }

        protected static Location GetLastLocation(SyntaxToken textToken, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return CreateLocation(value, textToken.SyntaxTree, textToken.SpanStart, textToken.ValueText.LastIndexOf(value, comparison), startOffset, endOffset);
        }

        protected static Location GetLastLocation(SyntaxTrivia trivia, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return CreateLocation(value, trivia.SyntaxTree, trivia.SpanStart, trivia.ToFullString().LastIndexOf(value, comparison), startOffset, endOffset);
        }

        protected static IEnumerable<Location> GetAllLocations(SyntaxToken textToken, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            var text = textToken.ValueText;

            if (text.Length <= 2 && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                yield break;
            }

            if (text.Length < value.Length)
            {
                // nothing to inspect as the text is too short
                yield break;
            }

            var syntaxTree = textToken.SyntaxTree;
            var spanStart = textToken.SpanStart;

            foreach (var position in text.AllIndicesOf(value, comparison))
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
            var text = textToken.ValueText;

            if (text.Length <= 2 && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                yield break;
            }

            var syntaxTree = textToken.SyntaxTree;
            var spanStart = textToken.SpanStart;

            foreach (var value in values)
            {
                if (text.Length < value.Length)
                {
                    // nothing to inspect as the text is too short
                    continue;
                }

                foreach (var position in text.AllIndicesOf(value, comparison))
                {
                    var location = CreateLocation(value, syntaxTree, spanStart, position, startOffset, endOffset);

                    if (location != null)
                    {
                        yield return location;
                    }
                }
            }
        }

        protected static IEnumerable<Location> GetAllLocations(SyntaxToken textToken, string value, Func<char, bool> nextCharValidationCallback, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            var text = textToken.ValueText;

            if (text.Length <= 2 && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                yield break;
            }

            if (text.Length < value.Length)
            {
                // nothing to inspect as the text is too short
                yield break;
            }

            var syntaxTree = textToken.SyntaxTree;
            var spanStart = textToken.SpanStart;

            var lastPosition = text.Length - 1;

            foreach (var position in text.AllIndicesOf(value, comparison))
            {
                var afterPosition = position + value.Length;
                if (afterPosition <= lastPosition)
                {
                    if (nextCharValidationCallback(text[afterPosition]) is false)
                    {
                        continue;
                    }
                }

                var location = CreateLocation(value, syntaxTree, spanStart, position, startOffset, endOffset);

                if (location != null)
                {
                    yield return location;
                }
            }
        }

        protected static IEnumerable<Location> GetAllLocations(SyntaxTrivia trivia, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            var text = trivia.ToFullString();

            if (text.Length <= 2 && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                yield break;
            }

            if (text.Length < value.Length)
            {
                // nothing to inspect as the text is too short
                yield break;
            }

            var syntaxTree = trivia.SyntaxTree;
            var spanStart = trivia.SpanStart;

            foreach (var position in text.AllIndicesOf(value, comparison))
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
            var text = trivia.ToFullString();

            if (text.Length <= 2 && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                yield break;
            }

            var syntaxTree = trivia.SyntaxTree;
            var spanStart = trivia.SpanStart;

            foreach (var value in values)
            {
                if (text.Length < value.Length)
                {
                    // nothing to inspect as the text is too short
                    continue;
                }

                foreach (var position in text.AllIndicesOf(value, comparison))
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
                                 .Concat(startingPhrases.Select(_ => _.FormatWith(returnType)))
                                 .Concat(startingPhrases.Select(_ => _.FormatWith(returnTypeFullyQualified)));
            }

            var ts = symbolReturnType.GetGenericArgumentsAsTs();

            var length = returnType.IndexOf('<'); // just until the first one

            var firstPart = returnType.Substring(0, length);

            var returnTypeWithTs = string.Concat(firstPart, "{", ts, "}");
            var returnTypeWithGenericCount = string.Concat(firstPart, '`', count);

            return Enumerable.Empty<string>()
                             .Concat(startingPhrases.Select(_ => _.FormatWith(returnTypeWithTs))) // for the phrases to show to the user
                             .Concat(startingPhrases.Select(_ => _.FormatWith(returnTypeWithGenericCount))); // for the real check
        }

        protected virtual bool ShallAnalyze(INamedTypeSymbol symbol) => true;

        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => true;

        protected virtual bool ShallAnalyze(IEventSymbol symbol) => true;

        protected virtual bool ShallAnalyze(IFieldSymbol symbol) => true;

        protected virtual bool ShallAnalyze(IPropertySymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol))
            {
                var comment = symbol.GetDocumentationCommentTriviaSyntax();

                if (comment != null)
                {
                    return AnalyzeType(symbol, compilation, symbol.GetDocumentationCommentXml(), comment);
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, commentXml, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol))
            {
                var comment = symbol.GetDocumentationCommentTriviaSyntax();

                if (comment != null)
                {
                    return AnalyzeMethod(symbol, compilation, symbol.GetDocumentationCommentXml(), comment);
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, commentXml, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol))
            {
                var comment = symbol.GetDocumentationCommentTriviaSyntax();

                if (comment != null)
                {
                    return AnalyzeEvent(symbol, compilation, symbol.GetDocumentationCommentXml(), comment);
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, commentXml, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol))
            {
                var comment = symbol.GetDocumentationCommentTriviaSyntax();

                if (comment != null)
                {
                    return AnalyzeField(symbol, compilation, symbol.GetDocumentationCommentXml(), comment);
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, commentXml, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol))
            {
                var comment = symbol.GetDocumentationCommentTriviaSyntax();

                if (comment != null)
                {
                    return AnalyzeProperty(symbol, compilation, comment, symbol.GetDocumentationCommentXml());
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment, string commentXml) => AnalyzeComment(symbol, compilation, commentXml, comment);

        protected virtual IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => Enumerable.Empty<Diagnostic>();

        private static Location CreateLocation(string value, SyntaxTree syntaxTree, int spanStart, int position, int startOffset, int endOffset)
        {
            if (position == -1)
            {
                return null;
            }

            var start = spanStart + position + startOffset; // find start position for underlining
            var end = start + value.Length - startOffset - endOffset; // find end position

            return CreateLocation(syntaxTree, start, end);
        }
    }
}