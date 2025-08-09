using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2040_LangwordAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2040";

        private static readonly HashSet<string> WrongAttributes = Constants.Comments.LangwordWrongAttributes;

        private static readonly string[] Phrases = Constants.Comments.LangwordReferences;

        private static readonly Pair[] StartParts = CreateStartParts();

        private static readonly Pair[] MiddleParts = CreateMiddleParts();

        private static readonly Pair[] EndParts = CreateEndParts();

        public MiKo_2040_LangwordAnalyzer() : base(Id)
        {
        }

//// ncrunch: rdi off

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var results = new List<Diagnostic>(1);

            foreach (var descendant in comment.DescendantNodes(DescendIntoChildren))
            {
                switch (descendant)
                {
                    case XmlElementSyntax e when e.IsWrongBooleanTag() || e.IsWrongNullTag():
                    {
                        var wrongText = e.Content.ToString();
                        var proposal = Proposal(wrongText);

                        results.Add(Issue(e, wrongText, proposal));

                        break;
                    }

                    case XmlEmptyElementSyntax ee when ee.IsSee(WrongAttributes) || ee.IsSeeAlso(WrongAttributes):
                    {
                        var wrongText = GetWrongText(ee.Attributes);
                        var proposal = Proposal(wrongText);

                        results.Add(Issue(ee, wrongText, proposal));

                        break;
                    }

                    case XmlElementSyntax e when e.IsSee(WrongAttributes) || e.IsSeeAlso(WrongAttributes):
                    {
                        var wrongText = GetWrongText(e.StartTag.Attributes);
                        var proposal = Proposal(wrongText);

                        results.Add(Issue(e, wrongText, proposal));

                        break;
                    }

                    case XmlTextSyntax textNode:
                    {
                        results.AddRange(AnalyzeTextTokens(textNode));

                        break;
                    }
                }
            }

            return results;
        }

        private static Pair[] CreateStartParts()
        {
            var phrasesLength = Phrases.Length;

            var results = new List<Pair>(6 * phrasesLength);

            for (var i = 0; i < phrasesLength; i++)
            {
                var phrase = Phrases[i];
                var proposal = Proposal(phrase);

                results.Add(new Pair(phrase.ConcatenatedWith('.'), proposal.ConcatenatedWith('.')));
                results.Add(new Pair(phrase.ConcatenatedWith('?'), proposal.ConcatenatedWith('?')));
                results.Add(new Pair(phrase.ConcatenatedWith('!'), proposal.ConcatenatedWith('!')));
                results.Add(new Pair(phrase.ConcatenatedWith(','), proposal.ConcatenatedWith(',')));
                results.Add(new Pair(phrase.ConcatenatedWith(';'), proposal.ConcatenatedWith(';')));
                results.Add(new Pair(phrase.ConcatenatedWith(':'), proposal.ConcatenatedWith(':')));
            }

            return results.ToArray();
        }

        private static Pair[] CreateMiddleParts()
        {
            var phrasesLength = Phrases.Length;

            var results = new List<Pair>(11 * phrasesLength);

            for (var i = 0; i < phrasesLength; i++)
            {
                var phrase = Phrases[i];
                var proposal = Proposal(phrase);

                results.Add(new Pair('('.ConcatenatedWith(phrase, ' '), '('.ConcatenatedWith(proposal, ' ')));
                results.Add(new Pair('('.ConcatenatedWith(phrase, ')'), '('.ConcatenatedWith(proposal, ')')));
                results.Add(new Pair(' '.ConcatenatedWith(phrase, ')'), ' '.ConcatenatedWith(proposal, ')')));
                results.Add(new Pair(' '.ConcatenatedWith(phrase, ' '), ' '.ConcatenatedWith(proposal, ' ')));
                results.Add(new Pair(' '.ConcatenatedWith(phrase, '.'), ' '.ConcatenatedWith(proposal, '.')));
                results.Add(new Pair(' '.ConcatenatedWith(phrase, '?'), ' '.ConcatenatedWith(proposal, '?')));
                results.Add(new Pair(' '.ConcatenatedWith(phrase, '!'), ' '.ConcatenatedWith(proposal, '!')));
                results.Add(new Pair(' '.ConcatenatedWith(phrase, ','), ' '.ConcatenatedWith(proposal, ',')));
                results.Add(new Pair(' '.ConcatenatedWith(phrase, ';'), ' '.ConcatenatedWith(proposal, ';')));
                results.Add(new Pair(' '.ConcatenatedWith(phrase, ':'), ' '.ConcatenatedWith(proposal, ':')));
                results.Add(new Pair('\''.ConcatenatedWith(phrase, '\''), proposal));
            }

            return results.ToArray();
        }

        private static Pair[] CreateEndParts()
        {
            var phrasesLength = Phrases.Length;

            var results = new Pair[phrasesLength];

            for (var i = 0; i < phrasesLength; i++)
            {
                var phrase = Phrases[i];
                var proposal = Proposal(phrase);

                results[i] = new Pair(' '.ConcatenatedWith(phrase), ' '.ConcatenatedWith(proposal));
            }

            return results;
        }

        private static string Proposal(string phrase) => string.Concat("<see " + Constants.XmlTag.Attribute.Langword + "=\"", phrase, "\"/>");

//// ncrunch: rdi default

        private static string GetWrongText(in SyntaxList<XmlAttributeSyntax> attributes)
        {
            var attribute = attributes.First(_ => WrongAttributes.Contains(_.GetName()));
            var token = attribute.FirstChildToken(SyntaxKind.XmlTextLiteralToken);

            return token.ValueText;
        }

        private static bool DescendIntoChildren(SyntaxNode descendant)
        {
            switch (descendant)
            {
                case DocumentationCommentTriviaSyntax _:
                    return true;

                case XmlTextSyntax _:
                    return false;

                case XmlElementSyntax e:
                {
                    switch (e.GetName())
                    {
                        case "b":
                        case Constants.XmlTag.C:
                        case Constants.XmlTag.Code:
                            return false; // do not dig deeper

                        default:
                            return true;
                    }
                }

                default:
                    return false;
            }
        }

        private IEnumerable<Diagnostic> AnalyzeTextTokens(XmlTextSyntax textNode)
        {
            var alreadyFoundLocations = new HashSet<Location>();

            var textTokens = textNode.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            for (int index = 0, textTokensCount = textTokens.Count; index < textTokensCount; index++)
            {
                var textToken = textTokens[index];

                if (textToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var text = textToken.ValueText;

                if (text.Length <= Constants.MinimumCharactersThreshold && text.IsNullOrWhiteSpace())
                {
                    // nothing to inspect as the text is too short and consists of whitespaces only
                    continue;
                }

                // loop over text to see if we have to report at all
                // (might be done a second time in case of report, but in case of no report we do not need to do all the other loops)
                if (text.ContainsAny(Phrases, StringComparison.OrdinalIgnoreCase) is false)
                {
                    // no need to loop further
                    continue;
                }

                // middle parts
                foreach (var item in MiddleParts)
                {
                    var wrongText = item.Key;
                    var proposal = item.Value;

                    const int StartOffset = 1; // we do not want to underline the first char
                    const int EndOffset = 1; // we do not want to underline the last char

                    var allLocations = GetAllLocations(textToken, wrongText, StringComparison.OrdinalIgnoreCase, StartOffset, EndOffset);
                    var allLocationsCount = allLocations.Count;

                    if (allLocationsCount > 0)
                    {
                        for (var locationsIndex = 0; locationsIndex < allLocationsCount; locationsIndex++)
                        {
                            var location = allLocations[locationsIndex];

                            if (alreadyFoundLocations.Add(location))
                            {
                                yield return Issue(location, wrongText, proposal);
                            }
                        }
                    }
                }

                // start
                foreach (var item in StartParts)
                {
                    var wrongText = item.Key;
                    var proposal = item.Value;

                    const int StartOffset = 0;
                    const int EndOffset = 1; // we do not want to underline the last char

                    var location = GetFirstLocation(textToken, wrongText, StringComparison.OrdinalIgnoreCase, StartOffset, EndOffset);

                    if (location != null && alreadyFoundLocations.Add(location))
                    {
                        yield return Issue(location, wrongText, proposal);
                    }
                }

                // end
                foreach (var item in EndParts)
                {
                    var wrongText = item.Key;
                    var proposal = item.Value;

                    const int StartOffset = 1; // we do not want to underline the first char
                    const int EndOffset = 0;

                    var location = GetLastLocation(textToken, wrongText, StringComparison.OrdinalIgnoreCase, StartOffset, EndOffset);

                    if (location != null && alreadyFoundLocations.Add(location))
                    {
                        yield return Issue(location, wrongText, proposal);
                    }
                }
            }
        }
    }
}