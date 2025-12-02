using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    /// <summary>
    /// Provides the base class for analyzers that enforce documentation rules.
    /// </summary>
    public abstract class DocumentationAnalyzer : Analyzer
    {
        /// <summary>
        /// Contains the XML tags that represents code.
        /// This field is read-only.
        /// </summary>
        protected static readonly string[] CodeTags = { Constants.XmlTag.Code, Constants.XmlTag.C };

        private static readonly SyntaxKind[] DocumentationCommentTrivia = { SyntaxKind.SingleLineDocumentationCommentTrivia }; // we do not want to analyze 'SyntaxKind.MultiLineDocumentationCommentTrivia'

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationAnalyzer"/> class with the unique identifier of the diagnostic.
        /// </summary>
        /// <param name="diagnosticId">
        /// The diagnostic identifier.
        /// </param>
        protected DocumentationAnalyzer(string diagnosticId) : base(nameof(Documentation), diagnosticId, (SymbolKind)(-1))
        {
        }

        /// <summary>
        /// Creates a proposal that contains the specified starting phrase.
        /// </summary>
        /// <param name="phrase">
        /// The starting phrase to include in the proposal.
        /// </param>
        /// <returns>
        /// An array of key-value pairs containing the starting phrase proposal.
        /// </returns>
        protected static Pair[] CreateStartingPhraseProposal(string phrase) => new[] { new Pair(Constants.AnalyzerCodeFixSharedData.StartingPhrase, phrase) };

        /// <summary>
        /// Creates a proposal that contains the specified starting and ending phrases.
        /// </summary>
        /// <param name="startPhrase">
        /// The starting phrase to include in the proposal.
        /// </param>
        /// <param name="endingPhrase">
        /// The ending phrase to include in the proposal.
        /// </param>
        /// <returns>
        /// An array of key-value pairs containing the starting and ending phrase proposals.
        /// </returns>
        protected static Pair[] CreateStartingEndingPhraseProposal(string startPhrase, string endingPhrase) => new[]
                                                                                                                   {
                                                                                                                       new Pair(Constants.AnalyzerCodeFixSharedData.StartingPhrase, startPhrase),
                                                                                                                       new Pair(Constants.AnalyzerCodeFixSharedData.EndingPhrase, endingPhrase),
                                                                                                                   };

        /// <summary>
        /// Creates a proposal that contains the specified ending phrase.
        /// </summary>
        /// <param name="phrase">
        /// The ending phrase to include in the proposal.
        /// </param>
        /// <returns>
        /// An array of key-value pairs containing the ending phrase proposal.
        /// </returns>
        protected static Pair[] CreateEndingPhraseProposal(string phrase) => new[] { new Pair(Constants.AnalyzerCodeFixSharedData.EndingPhrase, phrase) };

        /// <summary>
        /// Creates a proposal that contains the specified phrase.
        /// </summary>
        /// <param name="phrase">
        /// The phrase to include in the proposal.
        /// </param>
        /// <returns>
        /// An array of key-value pairs containing the phrase proposal.
        /// </returns>
        protected static Pair[] CreatePhraseProposal(string phrase) => new[] { new Pair(Constants.AnalyzerCodeFixSharedData.Phrase, phrase) };

        /// <summary>
        /// Creates a proposal that contains the specified text and its replacement.
        /// </summary>
        /// <param name="text">
        /// The text to be replaced.
        /// </param>
        /// <param name="replacement">
        /// The replacement text.
        /// </param>
        /// <returns>
        /// An array of key-value pairs containing the text and replacement proposal.
        /// </returns>
        protected static Pair[] CreateReplacementProposal(string text, string replacement) => new[]
                                                                                                  {
                                                                                                      new Pair(Constants.AnalyzerCodeFixSharedData.TextKey, text),
                                                                                                      new Pair(Constants.AnalyzerCodeFixSharedData.TextReplacementKey, replacement),
                                                                                                  };

        /// <summary>
        /// Gets the location of the first occurrence of the specified value within the token's text.
        /// </summary>
        /// <param name="textToken">
        /// The syntax token to search.
        /// </param>
        /// <param name="value">
        /// The value to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// The location of the first occurrence, or <see langword="null"/> if not found.
        /// </returns>
        protected static Location GetFirstLocation(in SyntaxToken textToken, string value, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return CreateLocation(value, textToken.SyntaxTree, textToken.SpanStart, textToken.ValueText.IndexOf(value, comparison), startOffset, endOffset);
        }

        /// <summary>
        /// Gets the location of the first occurrence of the specified value within the trivia's text.
        /// </summary>
        /// <param name="trivia">
        /// The syntax trivia to search.
        /// </param>
        /// <param name="value">
        /// The value to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// The location of the first occurrence, or <see langword="null"/> if not found.
        /// </returns>
        protected static Location GetFirstLocation(in SyntaxTrivia trivia, string value, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return CreateLocation(value, trivia.SyntaxTree, trivia.SpanStart, trivia.ToFullString().IndexOf(value, comparison), startOffset, endOffset);
        }

        /// <summary>
        /// Gets the location of the last occurrence of the specified value within the trivia's text.
        /// </summary>
        /// <param name="trivia">
        /// The syntax trivia to search.
        /// </param>
        /// <param name="value">
        /// The value to seek.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// The location of the last occurrence, or <see langword="null"/> if not found.
        /// </returns>
        protected static Location GetLastLocation(in SyntaxTrivia trivia, in char value, in int startOffset = 0, in int endOffset = 0)
        {
            return CreateLocation(value, trivia.SyntaxTree, trivia.SpanStart, trivia.ToFullString().LastIndexOf(value), startOffset, endOffset);
        }

        /// <summary>
        /// Gets the location of the last occurrence of the specified value within the token's text.
        /// </summary>
        /// <param name="textToken">
        /// The syntax token to search.
        /// </param>
        /// <param name="value">
        /// The value to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// The location of the last occurrence, or <see langword="null"/> if not found.
        /// </returns>
        protected static Location GetLastLocation(in SyntaxToken textToken, string value, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return CreateLocation(value, textToken.SyntaxTree, textToken.SpanStart, textToken.ValueText.LastIndexOf(value, comparison), startOffset, endOffset);
        }

        /// <summary>
        /// Gets the locations of all occurrences of the specified value within the token's text.
        /// </summary>
        /// <param name="textToken">
        /// The syntax token to search.
        /// </param>
        /// <param name="value">
        /// The value to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// A collection of locations for all occurrences found.
        /// </returns>
        protected static IReadOnlyList<Location> GetAllLocations(in SyntaxToken textToken, string value, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return GetAllLocations(textToken.ValueText, textToken.SyntaxTree, textToken.SpanStart, value, comparison, startOffset, endOffset);
        }

        /// <summary>
        /// Gets the locations of all occurrences of the specified values within the token's text.
        /// </summary>
        /// <param name="textToken">
        /// The syntax token to search.
        /// </param>
        /// <param name="values">
        /// The values to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// A collection of locations for all occurrences found.
        /// </returns>
        protected static IReadOnlyList<Location> GetAllLocations(in SyntaxToken textToken, in ReadOnlySpan<string> values, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return GetAllLocations(textToken.ValueText, textToken.SyntaxTree, textToken.SpanStart, values, comparison, startOffset, endOffset);
        }

        /// <summary>
        /// Gets the locations of all occurrences of the specified value within the trivia's text.
        /// </summary>
        /// <param name="trivia">
        /// The syntax trivia to search.
        /// </param>
        /// <param name="value">
        /// The value to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// A collection of locations for all occurrences found.
        /// </returns>
        protected static IReadOnlyList<Location> GetAllLocations(in SyntaxTrivia trivia, string value, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return GetAllLocations(trivia.ToFullString(), trivia.SyntaxTree, trivia.SpanStart, value, comparison, startOffset, endOffset);
        }

        /// <summary>
        /// Gets the locations of all occurrences of the specified values within the trivia's text.
        /// </summary>
        /// <param name="trivia">
        /// The syntax trivia to search.
        /// </param>
        /// <param name="values">
        /// The values to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// A collection of locations for all occurrences found.
        /// </returns>
        protected static IReadOnlyList<Location> GetAllLocations(in SyntaxTrivia trivia, in ReadOnlySpan<string> values, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return GetAllLocations(trivia.ToFullString(), trivia.SyntaxTree, trivia.SpanStart, values, comparison, startOffset, endOffset);
        }

        /// <summary>
        /// Gets the locations of all occurrences of the specified value within the token's text where the next character passes the validation.
        /// </summary>
        /// <param name="textToken">
        /// The syntax token to search.
        /// </param>
        /// <param name="value">
        /// The value to seek.
        /// </param>
        /// <param name="nextCharValidationCallback">
        /// A callback to validate the character following the found value.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// A collection of locations for all occurrences found that pass validation.
        /// </returns>
        protected static IReadOnlyList<Location> GetAllLocations(in SyntaxToken textToken, string value, Func<char, bool> nextCharValidationCallback, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return GetAllLocations(textToken.ValueText, textToken.SyntaxTree, textToken.SpanStart, value, nextCharValidationCallback, comparison, startOffset, endOffset);
        }

        /// <summary>
        /// Gets the location of the first text issue within the specified XML nodes.
        /// </summary>
        /// <param name="nodes">
        /// The XML nodes to search.
        /// </param>
        /// <returns>
        /// The location of the first text issue found.
        /// </returns>
        protected static Location GetFirstTextIssueLocation(in SyntaxList<XmlNodeSyntax> nodes)
        {
            var node = nodes[0];

            if (node is XmlTextSyntax text)
            {
                var textTokens = text.TextTokens;

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                for (int index = 0, textTokensCount = textTokens.Count; index < textTokensCount; index++)
                {
                    var token = textTokens[index];

                    if (token.IsKind(SyntaxKind.XmlTextLiteralToken))
                    {
                        return token.GetLocation();
                    }
                }
            }

            return node.GetLocation();
        }

        /// <summary>
        /// Gets the starting phrases for documentation based on the return type symbol.
        /// </summary>
        /// <param name="returnTypeSymbol">
        /// The return type symbol to generate phrases for.
        /// </param>
        /// <param name="startingPhrases">
        /// The template phrases to format with the return type information.
        /// </param>
        /// <returns>
        /// An array of starting phrases formatted with the return type information.
        /// </returns>
        protected static string[] GetStartingPhrases(ITypeSymbol returnTypeSymbol, string[] startingPhrases)
        {
            var returnType = returnTypeSymbol.ToString();

            var returnTypeFullyQualified = returnTypeSymbol.FullyQualifiedName();

            if (returnTypeFullyQualified.Contains('.') is false)
            {
                returnTypeFullyQualified = returnTypeSymbol.FullyQualifiedName(false);
            }

            var intermediateResult = Enumerable.Empty<string>();

            if (returnTypeSymbol.TryGetGenericArguments(out var arguments))
            {
                var genericArguments = returnTypeSymbol.GetGenericArgumentsAsTs();

                var length = returnType.IndexOf('<'); // just until the first one

                var firstPart = returnType.AsSpan(0, length);

                var returnTypeWithTs = firstPart.ConcatenatedWith('{', genericArguments, '}');
                var returnTypeWithGenericCount = firstPart.ConcatenatedWith('`', arguments.Length.ToString());

                var argument = arguments[0].FullyQualifiedName(false); // we are interested in the fully qualified name without any alias used, such as 'System.Int32' instead of 'int'

//// ncrunch: rdi off
                intermediateResult = intermediateResult.Concat(startingPhrases.Select(_ => _.FormatWith(returnTypeWithTs, argument)).Take(1)) // for the phrases to show to the user
                                                       .Concat(startingPhrases.Select(_ => _.FormatWith(returnTypeWithGenericCount, argument))); // for the real check
            }
            else
            {
                intermediateResult = intermediateResult.Concat(startingPhrases.Select(_ => _.FormatWith(returnType, Constants.TODO)).Take(1)) // for the phrases to show to the user
                                                       .Concat(startingPhrases.Select(_ => _.FormatWith(returnTypeFullyQualified, Constants.TODO))); // for the real check
            }

            var result = intermediateResult.Distinct()
                                           .ToArray();

//// ncrunch: rdi default

            return result;
        }

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeNamespace(INamespaceSymbol symbol, Compilation compilation) => throw new NotSupportedException("Namespaces are not supported.");

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation) => throw new NotSupportedException("Types are not supported.");

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation) => throw new NotSupportedException("Events are not supported.");

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation) => throw new NotSupportedException("Fields are not supported.");

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation) => throw new NotSupportedException("Methods are not supported.");

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation) => throw new NotSupportedException("Properties are not supported.");

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol, Compilation compilation) => throw new NotSupportedException("Parameters are not supported.");

        /// <inheritdoc/>
        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, DocumentationCommentTrivia);

        /// <summary>
        /// Determines whether the specified symbol shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The symbol to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the symbol shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyze(ISymbol symbol) => true;

        /// <summary>
        /// Analyzes the specified documentation comment for the given symbol.
        /// </summary>
        /// <param name="comment">
        /// The documentation comment to analyze.
        /// </param>
        /// <param name="symbol">
        /// The symbol associated with the documentation comment.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model for the analysis.
        /// </param>
        /// <returns>
        /// A collection of diagnostics found during the analysis.
        /// </returns>
        protected virtual IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel) => Array.Empty<Diagnostic>();

#pragma warning disable CA1021

        /// <summary>
        /// Determines whether the text start of the documentation shall be analyzed for issues.
        /// </summary>
        /// <param name="symbol">
        /// The symbol to check.
        /// </param>
        /// <param name="valueText">
        /// The value text to analyze.
        /// </param>
        /// <param name="problematicText">
        /// On successful return, contains the problematic text found.
        /// </param>
        /// <param name="comparison">
        /// On successful return, contains the <see cref="string"/> comparison mode to use.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the text start shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = null;
            comparison = StringComparison.Ordinal;

            return false;
        }
#pragma warning restore CA1021

        /// <summary>
        /// Determines whether empty text shall be considered as an issue.
        /// </summary>
        /// <param name="symbol">
        /// The symbol to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if empty text shall be considered as an issue; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ConsiderEmptyTextAsIssue(ISymbol symbol) => true;

        private static IReadOnlyList<Location> GetAllLocations(string text, SyntaxTree syntaxTree, in int spanStart, string value, in StringComparison comparison, in int startOffset, in int endOffset)
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

            if (allIndices.Length is 0)
            {
                // nothing to inspect
                return Array.Empty<Location>();
            }

            return GetAllLocationsWithLoop(syntaxTree, spanStart, value, startOffset, endOffset, allIndices);
        }

        private static IReadOnlyList<Location> GetAllLocations(string text, SyntaxTree syntaxTree, in int spanStart, in ReadOnlySpan<string> values, in StringComparison comparison, in int startOffset, in int endOffset)
        {
            var textLength = text.Length;

            if (textLength <= Constants.MinimumCharactersThreshold && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                return Array.Empty<Location>();
            }

            return GetAllLocationsWithLoop(text, syntaxTree, spanStart, values, comparison, startOffset, endOffset, textLength);
        }

        private static IReadOnlyList<Location> GetAllLocations(string text, SyntaxTree syntaxTree, in int spanStart, string value, Func<char, bool> nextCharValidationCallback, in StringComparison comparison, in int startOffset, in int endOffset)
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

            if (allIndices.Length is 0)
            {
                // nothing to inspect
                return Array.Empty<Location>();
            }

            return GetAllLocationsWithLoop(text, syntaxTree, spanStart, value, nextCharValidationCallback, startOffset, endOffset, textLength, allIndices);
        }

        private static IReadOnlyList<Location> GetAllLocationsWithLoop(SyntaxTree syntaxTree, in int spanStart, string value, in int startOffset, in int endOffset, in ReadOnlySpan<int> allIndices)
        {
            List<Location> alreadyReportedLocations = null;
            List<Location> results = null;

            for (int index = 0, length = allIndices.Length; index < length; index++)
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

        private static IReadOnlyList<Location> GetAllLocationsWithLoop(string text, SyntaxTree syntaxTree, in int spanStart, in ReadOnlySpan<string> values, in StringComparison comparison, in int startOffset, in int endOffset, in int textLength)
        {
            List<Location> alreadyReportedLocations = null;
            List<Location> results = null;

            for (int valueIndex = 0, valuesCount = values.Length; valueIndex < valuesCount; valueIndex++)
            {
                var value = values[valueIndex];

                if (textLength < value.Length)
                {
                    // nothing to inspect as the text is too short
                    continue;
                }

                var allIndices = text.AllIndicesOf(value, comparison);

                if (allIndices.Length is 0)
                {
                    // nothing to inspect
                    continue;
                }

                for (int index = 0, length = allIndices.Length; index < length; index++)
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

        private static IReadOnlyList<Location> GetAllLocationsWithLoop(string text, SyntaxTree syntaxTree, in int spanStart, string value, Func<char, bool> nextCharValidationCallback, in int startOffset, in int endOffset, in int textLength, in ReadOnlySpan<int> allIndices)
        {
            var lastPosition = textLength - 1;

            List<Location> alreadyReportedLocations = null;
            List<Location> results = null;

            for (int index = 0, length = allIndices.Length; index < length; index++)
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

        private void AnalyzeComment(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is DocumentationCommentTriviaSyntax comment)
            {
                var symbol = context.ContainingSymbol;

                if (ShallAnalyze(symbol))
                {
                    var issues = AnalyzeComment(comment, symbol, context.SemanticModel);

                    if (issues.Count > 0)
                    {
                        ReportDiagnostics(context, issues);
                    }
                }
            }
        }
    }
}