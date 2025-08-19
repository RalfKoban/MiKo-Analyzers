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

        protected static Location GetFirstLocation(in SyntaxToken textToken, string value, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return CreateLocation(value, textToken.SyntaxTree, textToken.SpanStart, textToken.ValueText.IndexOf(value, comparison), startOffset, endOffset);
        }

        protected static Location GetFirstLocation(in SyntaxTrivia trivia, string value, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return CreateLocation(value, trivia.SyntaxTree, trivia.SpanStart, trivia.ToFullString().IndexOf(value, comparison), startOffset, endOffset);
        }

        protected static Location GetLastLocation(in SyntaxTrivia trivia, in char value, in int startOffset = 0, in int endOffset = 0)
        {
            return CreateLocation(value, trivia.SyntaxTree, trivia.SpanStart, trivia.ToFullString().LastIndexOf(value), startOffset, endOffset);
        }

        protected static Location GetLastLocation(in SyntaxToken textToken, string value, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return CreateLocation(value, textToken.SyntaxTree, textToken.SpanStart, textToken.ValueText.LastIndexOf(value, comparison), startOffset, endOffset);
        }

        protected static IReadOnlyList<Location> GetAllLocations(in SyntaxToken textToken, string value, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return GetAllLocations(textToken.ValueText, textToken.SyntaxTree, textToken.SpanStart, value, comparison, startOffset, endOffset);
        }

        protected static IReadOnlyList<Location> GetAllLocations(in SyntaxToken textToken, in ReadOnlySpan<string> values, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return GetAllLocations(textToken.ValueText, textToken.SyntaxTree, textToken.SpanStart, values, comparison, startOffset, endOffset);
        }

        protected static IReadOnlyList<Location> GetAllLocations(in SyntaxTrivia trivia, string value, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return GetAllLocations(trivia.ToFullString(), trivia.SyntaxTree, trivia.SpanStart, value, comparison, startOffset, endOffset);
        }

        protected static IReadOnlyList<Location> GetAllLocations(in SyntaxTrivia trivia, in ReadOnlySpan<string> values, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return GetAllLocations(trivia.ToFullString(), trivia.SyntaxTree, trivia.SpanStart, values, comparison, startOffset, endOffset);
        }

        protected static IReadOnlyList<Location> GetAllLocations(in SyntaxToken textToken, string value, Func<char, bool> nextCharValidationCallback, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return GetAllLocations(textToken.ValueText, textToken.SyntaxTree, textToken.SpanStart, value, nextCharValidationCallback, comparison, startOffset, endOffset);
        }

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

        protected static IEnumerable<string> GetStartingPhrases(ITypeSymbol returnTypeSymbol, string[] startingPhrases)
        {
            var returnType = returnTypeSymbol.ToString();

            var returnTypeFullyQualified = returnTypeSymbol.FullyQualifiedName();

            if (returnTypeFullyQualified.Contains('.') is false)
            {
                returnTypeFullyQualified = returnTypeSymbol.FullyQualifiedName(false);
            }

            if (returnTypeSymbol.TryGetGenericArguments(out var arguments))
            {
                var genericArguments = returnTypeSymbol.GetGenericArgumentsAsTs();

                var length = returnType.IndexOf('<'); // just until the first one

                var firstPart = returnType.AsSpan(0, length);

                var returnTypeWithTs = firstPart.ConcatenatedWith('{', genericArguments, '}');
                var returnTypeWithGenericCount = firstPart.ConcatenatedWith('`', arguments.Length.ToString());

                var argument = arguments[0].FullyQualifiedName(false); // we are interested in the fully qualified name without any alias used, such as 'System.Int32' instead of 'int'

//// ncrunch: rdi off
                return Array.Empty<string>()
                            .Concat(startingPhrases.Take(1).Select(_ => _.FormatWith(returnTypeWithTs, argument))) // for the phrases to show to the user
                            .Concat(startingPhrases.Select(_ => _.FormatWith(returnTypeWithGenericCount, argument))); // for the real check
            }

            return Array.Empty<string>()
                        .Concat(startingPhrases.Take(1).Select(_ => _.FormatWith(returnType, Constants.TODO))) // for the phrases to show to the user
                        .Concat(startingPhrases.Select(_ => _.FormatWith(returnTypeFullyQualified, Constants.TODO))); // for the real check
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