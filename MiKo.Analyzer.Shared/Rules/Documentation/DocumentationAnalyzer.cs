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

        protected static Dictionary<string, string> CreateStartingPhraseProposal(string phrase) => new Dictionary<string, string> { { Constants.AnalyzerCodeFixSharedData.StartingPhrase, phrase } };

        protected static Dictionary<string, string> CreateStartingEndingPhraseProposal(string startPhrase, string endingPhrase) => new Dictionary<string, string>
                                                                                                                                       {
                                                                                                                                           { Constants.AnalyzerCodeFixSharedData.StartingPhrase, startPhrase },
                                                                                                                                           { Constants.AnalyzerCodeFixSharedData.EndingPhrase, endingPhrase },
                                                                                                                                       };

        protected static Dictionary<string, string> CreateEndingPhraseProposal(string phrase) => new Dictionary<string, string> { { Constants.AnalyzerCodeFixSharedData.EndingPhrase, phrase } };

        protected static Dictionary<string, string> CreatePhraseProposal(string phrase) => new Dictionary<string, string> { { Constants.AnalyzerCodeFixSharedData.Phrase, phrase } };

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
            return GetAllLocations(textToken.ValueText, textToken.SyntaxTree, textToken.SpanStart, value, comparison, startOffset, endOffset);
        }

        protected static IEnumerable<Location> GetAllLocations(SyntaxToken textToken, IEnumerable<string> values, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return GetAllLocations(textToken.ValueText, textToken.SyntaxTree, textToken.SpanStart, values, comparison, startOffset, endOffset);
        }

        protected static IEnumerable<Location> GetAllLocations(SyntaxTrivia trivia, string value, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return GetAllLocations(trivia.ToFullString(), trivia.SyntaxTree, trivia.SpanStart, value, comparison, startOffset, endOffset);
        }

        protected static IEnumerable<Location> GetAllLocations(SyntaxTrivia trivia, IEnumerable<string> values, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
        {
            return GetAllLocations(trivia.ToFullString(), trivia.SyntaxTree, trivia.SpanStart, values, comparison, startOffset, endOffset);
        }

        protected static IEnumerable<Location> GetAllLocations(SyntaxToken textToken, string value, Func<char, bool> nextCharValidationCallback, StringComparison comparison = StringComparison.Ordinal, int startOffset = 0, int endOffset = 0)
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
                return Enumerable.Empty<string>()
                                 .Concat(startingPhrases.Select(_ => _.FormatWith(returnTypeWithTs))) // for the phrases to show to the user
                                 .Concat(startingPhrases.Select(_ => _.FormatWith(returnTypeWithGenericCount))); // for the real check
            }

            return Enumerable.Empty<string>()
                             .Concat(startingPhrases.Select(_ => _.FormatWith(returnType)))
                             .Concat(startingPhrases.Select(_ => _.FormatWith(returnTypeFullyQualified)));
//// ncrunch: rdi default
        }

        protected virtual bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.GetDocumentationCommentId() != null;

        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => symbol.GetDocumentationCommentId() != null;

        protected virtual bool ShallAnalyze(IEventSymbol symbol) => symbol.GetDocumentationCommentId() != null;

        protected virtual bool ShallAnalyze(IPropertySymbol symbol) => symbol.GetDocumentationCommentId() != null;

        protected virtual bool ShallAnalyze(IFieldSymbol symbol) => symbol.GetDocumentationCommentId() != null;

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol))
            {
                foreach (var comment in symbol.GetDocumentationCommentTriviaSyntax())
                {
                    var issues = AnalyzeType(symbol, compilation, symbol.GetDocumentationCommentXml(), comment);

                    foreach (var issue in issues)
                    {
                        yield return issue;
                    }
                }
            }
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, commentXml, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol))
            {
                foreach (var comment in symbol.GetDocumentationCommentTriviaSyntax())
                {
                    var issues = AnalyzeMethod(symbol, compilation, symbol.GetDocumentationCommentXml(), comment);

                    foreach (var issue in issues)
                    {
                        yield return issue;
                    }
                }
            }
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, commentXml, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol))
            {
                foreach (var comment in symbol.GetDocumentationCommentTriviaSyntax())
                {
                    var issues = AnalyzeEvent(symbol, compilation, symbol.GetDocumentationCommentXml(), comment);

                    foreach (var issue in issues)
                    {
                        yield return issue;
                    }
                }
            }
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, commentXml, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol))
            {
                foreach (var comment in symbol.GetDocumentationCommentTriviaSyntax())
                {
                    var issues = AnalyzeProperty(symbol, compilation, symbol.GetDocumentationCommentXml(), comment);

                    foreach (var issue in issues)
                    {
                        yield return issue;
                    }
                }
            }
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, commentXml, comment);

        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol))
            {
                foreach (var comment in symbol.GetDocumentationCommentTriviaSyntax())
                {
                    var issues = AnalyzeField(symbol, compilation, symbol.GetDocumentationCommentXml(), comment);

                    foreach (var issue in issues)
                    {
                        yield return issue;
                    }
                }
            }
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, compilation, commentXml, comment);

        protected virtual IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => Enumerable.Empty<Diagnostic>();

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

        protected virtual bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = null;
            comparison = StringComparison.Ordinal;

            return false;
        }

        protected virtual bool ConsiderEmptyTextAsIssue(ISymbol symbol) => true;

        private static IEnumerable<Location> GetAllLocations(string text, SyntaxTree syntaxTree, int spanStart, string value, StringComparison comparison, int startOffset, int endOffset)
        {
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

            List<Location> alreadyReportedLocations = null;

            foreach (var position in text.AllIndicesOf(value, comparison))
            {
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

                yield return location;
            }
        }

        private static IEnumerable<Location> GetAllLocations(string text, SyntaxTree syntaxTree, int spanStart, IEnumerable<string> values, StringComparison comparison, int startOffset, int endOffset)
        {
            if (text.Length <= 2 && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                yield break;
            }

            List<Location> alreadyReportedLocations = null;

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

                    yield return location;
                }
            }
        }

        private static IEnumerable<Location> GetAllLocations(string text, SyntaxTree syntaxTree, int spanStart, string value, Func<char, bool> nextCharValidationCallback, StringComparison comparison, int startOffset, int endOffset)
        {
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

            var lastPosition = text.Length - 1;

            List<Location> alreadyReportedLocations = null;

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

                yield return location;
            }
        }
    }
}