using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class DocumentationAnalyzer : Analyzer
    {
        protected static readonly string[] CodeTags = { Constants.XmlTag.Code, Constants.XmlTag.C };

        protected static readonly SyntaxKind[] DocumentationCommentTrivia = { SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia };

        protected DocumentationAnalyzer(string diagnosticId) : base(nameof(Documentation), diagnosticId, (SymbolKind)(-1))
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

        protected static Location GetLastLocation(SyntaxTrivia trivia, char value, int startOffset = 0, int endOffset = 0)
        {
            return CreateLocation(value, trivia.SyntaxTree, trivia.SpanStart, trivia.ToFullString().LastIndexOf(value), startOffset, endOffset);
        }

        protected static Location GetLastLocation(SyntaxToken textToken, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return CreateLocation(value, textToken.SyntaxTree, textToken.SpanStart, textToken.ValueText.LastIndexOf(value, comparison), startOffset, endOffset);
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
                var returnTypeWithGenericCount = firstPart.ConcatenatedWith('`', count.ToString());

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

        protected sealed override IEnumerable<Diagnostic> AnalyzeNamespace(INamespaceSymbol symbol, Compilation compilation) => throw new NotSupportedException("Namespaces are not supported.");

        protected sealed override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation) => throw new NotSupportedException("Types are not supported.");

        protected sealed override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation) => throw new NotSupportedException("Events are not supported.");

        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation) => throw new NotSupportedException("Fields are not supported.");

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation) => throw new NotSupportedException("Methods are not supported.");

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation) => throw new NotSupportedException("Properties are not supported.");

        protected sealed override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol, Compilation compilation) => throw new NotSupportedException("Parameters are not supported.");

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, DocumentationCommentTrivia);

        protected virtual bool ShallAnalyze(ISymbol symbol) => true;

        protected virtual IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel) => Array.Empty<Diagnostic>();

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