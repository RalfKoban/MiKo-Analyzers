using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        protected virtual bool ShallAnalyze(INamedTypeSymbol symbol) => true;

        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => true;

        protected virtual bool ShallAnalyze(IEventSymbol symbol) => true;

        protected virtual bool ShallAnalyze(IFieldSymbol symbol) => true;

        protected virtual bool ShallAnalyze(IPropertySymbol symbol) => true;

        protected sealed override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            var comment = symbol.GetDocumentationCommentTriviaSyntax();

            return comment != null && ShallAnalyze(symbol)
                      ? AnalyzeType(symbol, compilation, comment)
                      : Enumerable.Empty<Diagnostic>();
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation)
        {
            var comment = symbol.GetDocumentationCommentTriviaSyntax();

            return comment != null && ShallAnalyze(symbol)
                  ? AnalyzeMethod(symbol, compilation, comment)
                  : Enumerable.Empty<Diagnostic>();
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation)
        {
            var comment = symbol.GetDocumentationCommentTriviaSyntax();

            return comment != null && ShallAnalyze(symbol)
                  ? AnalyzeEvent(symbol, compilation, comment)
                  : Enumerable.Empty<Diagnostic>();
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation)
        {
            var comment = symbol.GetDocumentationCommentTriviaSyntax();

            return comment != null && ShallAnalyze(symbol)
                  ? AnalyzeField(symbol, compilation, comment)
                  : Enumerable.Empty<Diagnostic>();
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation)
        {
            var comment = symbol.GetDocumentationCommentTriviaSyntax();

            return comment != null && ShallAnalyze(symbol)
                  ? AnalyzeProperty(symbol, compilation, comment)
                  : Enumerable.Empty<Diagnostic>();
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, comment);

        protected virtual IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment) => Enumerable.Empty<Diagnostic>();

        protected Diagnostic AnalyzeStart(ISymbol symbol, string tag, SyntaxNode startElement)
        {
            var elementsToSkip = 0;
            var analyzedFirstText = false;

            var descendantNodes = startElement.DescendantNodes();
            foreach (var node in descendantNodes.TakeWhile(_ => analyzedFirstText is false))
            {
                elementsToSkip++;

                switch (node)
                {
                    case XmlElementStartTagSyntax startTag:
                        {
                            var tagName = startTag.GetName();

                            if (tagName == tag || tagName == Constants.XmlTag.Para)
                            {
                                continue; // skip over the start tag and name syntax
                            }
                            else
                            {
                                return StartIssue(symbol, node); // it's no text, so it must be something different
                            }
                        }

                    case XmlNameSyntax _:
                    case XmlElementSyntax e when e.GetName() == Constants.XmlTag.Para:
                    case XmlEmptyElementSyntax ee when ee.GetName() == Constants.XmlTag.Para:
                        continue; // skip over the start tag and name syntax

                    case XmlTextSyntax text:
                        {
                            // report the location of the text issue via the corresponding text token
                            foreach (var textToken in text.TextTokens)
                            {
                                if (textToken.ValueText.IsNullOrWhiteSpace())
                                {
                                    // we found the first but empty /// line, so ignore it
                                    continue;
                                }

                                analyzedFirstText = true;

                                var issue = StartIssue(symbol, textToken);
                                if (issue != null)
                                {
                                    return issue;
                                }
                            }

                            // we found a completely empty /// line, so ignore it
                            continue;
                        }

                    default:
                        return StartIssue(symbol, node); // it's no text, so it must be something different
                }
            }

            return AnalyzeStartContinue(symbol, descendantNodes.Skip(elementsToSkip));
        }

        protected virtual Diagnostic AnalyzeStartContinue(ISymbol symbol, IEnumerable<SyntaxNode> remainingNodes) => null; // nothing to report

        protected virtual Diagnostic StartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node);

        protected virtual Diagnostic StartIssue(ISymbol symbol, SyntaxToken textToken) => Issue(symbol.Name, textToken);

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