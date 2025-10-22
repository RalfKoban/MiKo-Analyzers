using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2245_DocumentationUsesNumbersInCodeAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2245";

        public MiKo_2245_DocumentationUsesNumbersInCodeAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            List<Diagnostic> issues = null;

            foreach (var xmlText in comment.DescendantNodes<XmlTextSyntax>())
            {
                var parent = xmlText.Parent;

                if (parent.IsCode())
                {
                    continue;
                }

                if (parent.IsC())
                {
                    continue;
                }

                var textTokens = xmlText.TextTokens;

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var textTokensCount = textTokens.Count;

                if (textTokensCount > 0)
                {
                    for (var index = 0; index < textTokensCount; index++)
                    {
                        var token = textTokens[index];

                        if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                        {
                            continue;
                        }

                        var text = token.ValueText;

                        foreach (ReadOnlySpan<char> word in text.AsSpan().WordsAsSpan(WordBoundary.WhiteSpaces))
                        {
                            if (word.IsNumber())
                            {
                                var number = word.ToString();

                                var locations = GetAllLocations(token, number);

                                if (issues is null)
                                {
                                    issues = new List<Diagnostic>(locations.Count);
                                }

                                foreach (var location in locations)
                                {
                                    issues.Add(Issue(location, number));
                                }
                            }
                        }
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }
    }
}