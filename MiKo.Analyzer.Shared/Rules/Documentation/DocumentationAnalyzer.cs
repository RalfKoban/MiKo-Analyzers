using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class DocumentationAnalyzer : Analyzer
    {
        protected static readonly string[] CodeTags = { Constants.XmlTag.Code, Constants.XmlTag.C };

        protected DocumentationAnalyzer(string diagnosticId, SymbolKind symbolKind) : base(nameof(Documentation), diagnosticId, symbolKind)
        {
        }

        protected static Pair[] CreateStartingPhraseProposal(string phrase) => new[] { new Pair(Constants.AnalyzerCodeFixSharedData.StartingPhrase, phrase) };

        protected static Pair[] CreateStartingEndingPhraseProposal(string startPhrase, string endingPhrase) => new[]
                                                                                                                   {
                                                                                                                       new Pair(Constants.AnalyzerCodeFixSharedData.StartingPhrase, startPhrase),
                                                                                                                       new Pair(Constants.AnalyzerCodeFixSharedData.EndingPhrase, endingPhrase),
                                                                                                                   };

        protected static Pair[] CreateEndingPhraseProposal(string phrase) => new[] { new Pair(Constants.AnalyzerCodeFixSharedData.EndingPhrase, phrase) };

        protected static Pair[] CreatePhraseProposal(string phrase) => new[] { new Pair(Constants.AnalyzerCodeFixSharedData.Phrase, phrase) };

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

        protected static IReadOnlyList<Location> GetAllLocations(SyntaxToken textToken, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return GetAllLocations(textToken.ValueText, textToken.SyntaxTree, textToken.SpanStart, value, comparison, startOffset, endOffset);
        }

        protected static IReadOnlyList<Location> GetAllLocations(SyntaxToken textToken, string[] values, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return GetAllLocations(textToken.ValueText, textToken.SyntaxTree, textToken.SpanStart, values, comparison, startOffset, endOffset);
        }

        protected static IReadOnlyList<Location> GetAllLocations(SyntaxTrivia trivia, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return GetAllLocations(trivia.ToFullString(), trivia.SyntaxTree, trivia.SpanStart, value, comparison, startOffset, endOffset);
        }

        protected static IReadOnlyList<Location> GetAllLocations(SyntaxTrivia trivia, string[] values, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return GetAllLocations(trivia.ToFullString(), trivia.SyntaxTree, trivia.SpanStart, values, comparison, startOffset, endOffset);
        }

        protected static IReadOnlyList<Location> GetAllLocations(SyntaxToken textToken, string value, Func<char, bool> nextCharValidationCallback, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return GetAllLocations(textToken.ValueText, textToken.SyntaxTree, textToken.SpanStart, value, nextCharValidationCallback, comparison, startOffset, endOffset);
        }

        protected static Location GetFirstTextIssueLocation(SyntaxList<XmlNodeSyntax> content)
        {
            var item = content[0];

            if (item is XmlTextSyntax text)
            {
                var textTokens = text.TextTokens;

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var textTokensCount = textTokens.Count;

                for (var index = 0; index < textTokensCount; index++)
                {
                    var token = textTokens[index];

                    if (token.IsKind(SyntaxKind.XmlTextLiteralToken))
                    {
                        return token.GetLocation();
                    }
                }
            }

            return item.GetLocation();
        }

        protected static IEnumerable<string> GetStartingPhrases(ITypeSymbol symbolReturnType, string[] startingPhrases)
        {
            var returnType = symbolReturnType.ToString();

            var returnTypeFullyQualified = symbolReturnType.FullyQualifiedName();

            if (returnTypeFullyQualified.Contains('.') is false)
            {
                returnTypeFullyQualified = symbolReturnType.FullyQualifiedName(false);
            }

            if (symbolReturnType.TryGetGenericArgumentCount(out var count) && count > 0)
            {
                var ts = symbolReturnType.GetGenericArgumentsAsTs();

                var length = returnType.IndexOf('<'); // just until the first one

                var firstPart = returnType.AsSpan(0, length);

                var returnTypeWithTs = firstPart.ConcatenatedWith('{', ts, '}');
                var returnTypeWithGenericCount = firstPart.ConcatenatedWith("`", count.ToString());

//// ncrunch: rdi off
                return Array.Empty<string>()
                            .Concat(startingPhrases.Select(_ => _.FormatWith(returnTypeWithTs))) // for the phrases to show to the user
                            .Concat(startingPhrases.Select(_ => _.FormatWith(returnTypeWithGenericCount))); // for the real check
            }

            return Array.Empty<string>()
                        .Concat(startingPhrases.Select(_ => _.FormatWith(returnType)))
                        .Concat(startingPhrases.Select(_ => _.FormatWith(returnTypeFullyQualified)));
//// ncrunch: rdi default
        }

        protected virtual bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.HasDocumentationCommentTriviaSyntax();

        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => symbol.HasDocumentationCommentTriviaSyntax();

        protected virtual bool ShallAnalyze(IEventSymbol symbol) => symbol.HasDocumentationCommentTriviaSyntax();

        protected virtual bool ShallAnalyze(IPropertySymbol symbol) => symbol.HasDocumentationCommentTriviaSyntax();

        protected virtual bool ShallAnalyze(IFieldSymbol symbol) => symbol.HasDocumentationCommentTriviaSyntax();

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol) is false)
            {
                return Array.Empty<Diagnostic>();
            }

            var comments = symbol.GetDocumentationCommentTriviaSyntax();
            var commentsLength = comments.Length;

            if (commentsLength <= 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var commentXml = symbol.GetDocumentationCommentXml();

            if (commentsLength == 1)
            {
                return AnalyzeType(symbol, compilation, commentXml, comments[0]);
            }

            return AnalyzeTypeWithLoop(symbol, compilation, commentXml, comments);
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, commentXml, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol) is false)
            {
                return Array.Empty<Diagnostic>();
            }

            var comments = symbol.GetDocumentationCommentTriviaSyntax();
            var commentsLength = comments.Length;

            if (commentsLength <= 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var commentXml = symbol.GetDocumentationCommentXml();

            if (commentsLength == 1)
            {
                return AnalyzeMethod(symbol, compilation, commentXml, comments[0]);
            }

            return AnalyzeMethodWithLoop(symbol, compilation, commentXml, comments);
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, commentXml, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol) is false)
            {
                return Array.Empty<Diagnostic>();
            }

            var comments = symbol.GetDocumentationCommentTriviaSyntax();
            var commentsLength = comments.Length;

            if (commentsLength <= 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var commentXml = symbol.GetDocumentationCommentXml();

            if (commentsLength == 1)
            {
                return AnalyzeEvent(symbol, compilation, commentXml, comments[0]);
            }

            return AnalyzeEventWithLoop(symbol, compilation, commentXml, comments);
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, commentXml, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol) is false)
            {
                return Array.Empty<Diagnostic>();
            }

            var comments = symbol.GetDocumentationCommentTriviaSyntax();
            var commentsLength = comments.Length;

            if (commentsLength <= 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var commentXml = symbol.GetDocumentationCommentXml();

            if (commentsLength == 1)
            {
                return AnalyzeProperty(symbol, compilation, commentXml, comments[0]);
            }

            return AnalyzePropertyWithLoop(symbol, compilation, commentXml, comments);
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, commentXml, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol) is false)
            {
                return Array.Empty<Diagnostic>();
            }

            var comments = symbol.GetDocumentationCommentTriviaSyntax();
            var commentsLength = comments.Length;

            if (commentsLength <= 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var commentXml = symbol.GetDocumentationCommentXml();

            if (commentsLength == 1)
            {
                return AnalyzeField(symbol, compilation, commentXml, comments[0]);
            }

            return AnalyzeFieldWithLoop(symbol, compilation, commentXml, comments);
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, commentXml, comment);

        protected virtual IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => Array.Empty<Diagnostic>();

        protected virtual Diagnostic StartIssue(ISymbol symbol, SyntaxNode node) => StartIssue(symbol, node.GetLocation());

        protected virtual Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location);

        protected Diagnostic AnalyzeTextStart(ISymbol symbol, XmlElementSyntax xml)
        {
            var tag = xml.StartTag.GetName();

            var descendantNodes = xml.DescendantNodes();

            foreach (var node in descendantNodes)
            {
                switch (node)
                {
                    case XmlElementStartTagSyntax startTag:
                    {
                        var tagName = startTag.GetName();

                        if (tagName == tag || tagName == Constants.XmlTag.Para)
                        {
                            continue; // skip over the start tag and name syntax
                        }

                        return StartIssue(symbol, node); // it's no text, so it must be something different
                    }

                    case XmlElementEndTagSyntax endTag:
                    {
                        var tagName = endTag.GetName();

                        if (tagName == Constants.XmlTag.Para)
                        {
                            continue; // skip over the start tag and name syntax
                        }

                        if (endTag.Parent is XmlElementSyntax element)
                        {
                            if (ConsiderEmptyTextAsIssue(symbol))
                            {
                                return StartIssue(symbol, element.GetContentsLocation()); // it's an empty text
                            }

                            continue;
                        }

                        return StartIssue(symbol, node); // it's no text, so it must be something different
                    }

                    case XmlNameSyntax _:
                    case XmlElementSyntax e when e.GetName() == Constants.XmlTag.Para:
                    case XmlEmptyElementSyntax ee when ee.GetName() == Constants.XmlTag.Para:
                        continue; // skip over the start tag and name syntax

                    case XmlTextSyntax text:
                    {
                        // report the location of the first word(s) via the corresponding text token
                        var textTokens = text.TextTokens;

                        // keep in local variable to avoid multiple requests (see Roslyn implementation)
                        var textTokensCount = textTokens.Count;

                        for (var index = 0; index < textTokensCount; index++)
                        {
                            var token = textTokens[index];

                            if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                            {
                                continue;
                            }

                            var valueText = token.ValueText;

                            if (valueText.IsNullOrWhiteSpace())
                            {
                                // we found the first but empty /// line, so ignore it
                                continue;
                            }

                            if (valueText.Length == 1 && Constants.Comments.Delimiters.Contains(valueText[0]))
                            {
                                // this is a dot or something directly after the XML tag, so ignore that
                                continue;
                            }

                            // we found some text
                            if (AnalyzeTextStart(symbol, valueText, out var problematicText, out var comparison))
                            {
                                // it's no valid text, so we have an issue
                                var position = valueText.IndexOf(problematicText, comparison);

                                var start = token.SpanStart + position; // find start position for underlining
                                var end = start + problematicText.Length; // find end position for underlining

                                var location = CreateLocation(token, start, end);

                                return StartIssue(symbol, location);
                            }

                            // it's a valid text, so we quit
                            return null;
                        }

                        // we found a completely empty /// line, so ignore it
                        continue;
                    }

                    default:
                        return StartIssue(symbol, node); // it's no text, so it must be something different
                }
            }

            // nothing to report
            return null;
        }

#pragma warning disable CA1021
        protected virtual bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = null;
            comparison = StringComparison.Ordinal;

            return false;
        }
#pragma warning restore CA1021

        protected virtual bool ConsiderEmptyTextAsIssue(ISymbol symbol) => true;

        private static IReadOnlyList<Location> GetAllLocations(string text, SyntaxTree syntaxTree, int spanStart, string value, StringComparison comparison, int startOffset, int endOffset)
        {
            var textLength = text.Length;

            if (textLength <= Constants.MinimumCharactersThreshold && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                return Array.Empty<Location>();
            }

            if (textLength < value.Length)
            {
                // nothing to inspect as the text is too short
                return Array.Empty<Location>();
            }

            var allIndices = text.AllIndicesOf(value, comparison);

            if (allIndices is int[] array && array.Length == 0)
            {
                // nothing to inspect
                return Array.Empty<Location>();
            }

            return GetAllLocationsWithLoop(syntaxTree, spanStart, value, startOffset, endOffset, allIndices);
        }

        private static IReadOnlyList<Location> GetAllLocations(string text, SyntaxTree syntaxTree, int spanStart, string[] values, StringComparison comparison, int startOffset, int endOffset)
        {
            var textLength = text.Length;

            if (textLength <= Constants.MinimumCharactersThreshold && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                return Array.Empty<Location>();
            }

            return GetAllLocationsWithLoop(text, syntaxTree, spanStart, values, comparison, startOffset, endOffset, textLength);
        }

        private static IReadOnlyList<Location> GetAllLocations(string text, SyntaxTree syntaxTree, int spanStart, string value, Func<char, bool> nextCharValidationCallback, StringComparison comparison, int startOffset, int endOffset)
        {
            var textLength = text.Length;

            if (textLength <= Constants.MinimumCharactersThreshold && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                return Array.Empty<Location>();
            }

            if (textLength < value.Length)
            {
                // nothing to inspect as the text is too short
                return Array.Empty<Location>();
            }

            var allIndices = text.AllIndicesOf(value, comparison);

            if (allIndices is int[] array && array.Length == 0)
            {
                // nothing to inspect
                return Array.Empty<Location>();
            }

            return GetAllLocationsWithLoop(text, syntaxTree, spanStart, value, nextCharValidationCallback, startOffset, endOffset, textLength, allIndices);
        }

        private static IReadOnlyList<Location> GetAllLocationsWithLoop(SyntaxTree syntaxTree, int spanStart, string value, int startOffset, int endOffset, IReadOnlyList<int> allIndices)
        {
            List<Location> alreadyReportedLocations = null;
            var count = allIndices.Count;

            List<Location> results = null;

            for (var index = 0; index < count; index++)
            {
                var position = allIndices[index];

                var location = CreateLocation(value, syntaxTree, spanStart, position, startOffset, endOffset);

                if (location is null)
                {
                    continue;
                }

                if (alreadyReportedLocations is null)
                {
                    alreadyReportedLocations = new List<Location>(1);
                }
                else
                {
                    if (alreadyReportedLocations.Exists(_ => location.IntersectsWith(_)))
                    {
                        // already reported, so ignore it
                        continue;
                    }
                }

                alreadyReportedLocations.Add(location);

                if (results is null)
                {
                    results = new List<Location>(1);
                }

                results.Add(location);
            }

            return results ?? (IReadOnlyList<Location>)Array.Empty<Location>();
        }

        private static IReadOnlyList<Location> GetAllLocationsWithLoop(string text, SyntaxTree syntaxTree, int spanStart, string[] values, StringComparison comparison, int startOffset, int endOffset, int textLength)
        {
            List<Location> alreadyReportedLocations = null;
            var valuesCount = values.Length;

            List<Location> results = null;

            for (var valueIndex = 0; valueIndex < valuesCount; valueIndex++)
            {
                var value = values[valueIndex];

                if (textLength < value.Length)
                {
                    // nothing to inspect as the text is too short
                    continue;
                }

                var allIndices = text.AllIndicesOf(value, comparison);

                if (allIndices is int[] array && array.Length == 0)
                {
                    // nothing to inspect
                    continue;
                }

                var count = allIndices.Count;

                for (var index = 0; index < count; index++)
                {
                    var position = allIndices[index];

                    var location = CreateLocation(value, syntaxTree, spanStart, position, startOffset, endOffset);

                    if (location is null)
                    {
                        continue;
                    }

                    if (alreadyReportedLocations is null)
                    {
                        alreadyReportedLocations = new List<Location>(1);
                    }
                    else
                    {
                        if (alreadyReportedLocations.Exists(_ => location.IntersectsWith(_)))
                        {
                            // already reported, so ignore it
                            continue;
                        }
                    }

                    alreadyReportedLocations.Add(location);

                    if (results is null)
                    {
                        results = new List<Location>(1);
                    }

                    results.Add(location);
                }
            }

            return results ?? (IReadOnlyList<Location>)Array.Empty<Location>();
        }

        private static IReadOnlyList<Location> GetAllLocationsWithLoop(string text, SyntaxTree syntaxTree, int spanStart, string value, Func<char, bool> nextCharValidationCallback, int startOffset, int endOffset, int textLength, IReadOnlyList<int> allIndices)
        {
            var lastPosition = textLength - 1;

            List<Location> alreadyReportedLocations = null;
            var count = allIndices.Count;

            List<Location> results = null;

            for (var index = 0; index < count; index++)
            {
                var position = allIndices[index];
                var afterPosition = position + value.Length;

                if (afterPosition <= lastPosition)
                {
                    if (nextCharValidationCallback(text[afterPosition]) is false)
                    {
                        continue;
                    }
                }

                var location = CreateLocation(value, syntaxTree, spanStart, position, startOffset, endOffset);

                if (location is null)
                {
                    continue;
                }

                if (alreadyReportedLocations is null)
                {
                    alreadyReportedLocations = new List<Location>(1);
                }
                else
                {
                    if (alreadyReportedLocations.Exists(_ => location.IntersectsWith(_)))
                    {
                        // already reported, so ignore it
                        continue;
                    }
                }

                alreadyReportedLocations.Add(location);

                if (results is null)
                {
                    results = new List<Location>(1);
                }

                results.Add(location);
            }

            return results ?? (IReadOnlyList<Location>)Array.Empty<Location>();
        }

        private IEnumerable<Diagnostic> AnalyzeTypeWithLoop(INamedTypeSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax[] comments)
        {
            var commentsLength = comments.Length;

            for (var index = 0; index < commentsLength; index++)
            {
                var issues = AnalyzeType(symbol, compilation, commentXml, comments[index]);

                if (issues.IsEmptyArray())
                {
                    continue;
                }

                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var issue in issues)
                {
                    if (issue != null)
                    {
                        yield return issue;
                    }
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeMethodWithLoop(IMethodSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax[] comments)
        {
            var commentsLength = comments.Length;

            for (var index = 0; index < commentsLength; index++)
            {
                var issues = AnalyzeMethod(symbol, compilation, commentXml, comments[index]);

                if (issues.IsEmptyArray())
                {
                    continue;
                }

                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var issue in issues)
                {
                    if (issue != null)
                    {
                        yield return issue;
                    }
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeEventWithLoop(IEventSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax[] comments)
        {
            var commentsLength = comments.Length;

            for (var index = 0; index < commentsLength; index++)
            {
                var issues = AnalyzeEvent(symbol, compilation, commentXml, comments[index]);

                if (issues.IsEmptyArray())
                {
                    continue;
                }

                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var issue in issues)
                {
                    if (issue != null)
                    {
                        yield return issue;
                    }
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzePropertyWithLoop(IPropertySymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax[] comments)
        {
            var commentsLength = comments.Length;

            for (var index = 0; index < commentsLength; index++)
            {
                var issues = AnalyzeProperty(symbol, compilation, commentXml, comments[index]);

                if (issues.IsEmptyArray())
                {
                    continue;
                }

                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var issue in issues)
                {
                    if (issue != null)
                    {
                        yield return issue;
                    }
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeFieldWithLoop(IFieldSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax[] comments)
        {
            var commentsLength = comments.Length;

            for (var index = 0; index < commentsLength; index++)
            {
                var issues = AnalyzeField(symbol, compilation, commentXml, comments[index]);

                if (issues.IsEmptyArray())
                {
                    continue;
                }

                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var issue in issues)
                {
                    if (issue != null)
                    {
                        yield return issue;
                    }
                }
            }
        }
    }
}