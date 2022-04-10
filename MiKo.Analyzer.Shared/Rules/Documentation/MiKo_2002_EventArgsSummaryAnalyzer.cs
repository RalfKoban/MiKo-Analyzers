using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2002_EventArgsSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2002";

        private const string Proposal = "Provides data for the <see cref=\" ... \"/> event.";
        private const string StartingPhrase = "Provides data for the ";
        private const string MultipleStartingPhrase = "Provides data for ";
        private const string MultipleEndingPhrase = " events.";
        private static readonly string[] EndingPhrases = { " event.", " events." };

        public MiKo_2002_EventArgsSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsEventArgs() && base.ShallAnalyze(symbol);

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var text = textToken.ValueText.TrimStart();

            if (text.StartsWith(StartingPhrase, StringComparison.Ordinal) && text.ContainsAny(EndingPhrases) is false)
            {
                return null;
            }

            if (text.StartsWith(MultipleStartingPhrase, StringComparison.Ordinal) && text.Contains(MultipleEndingPhrase))
            {
                return null;
            }

            return Issue(symbol.Name, textToken, Proposal);
        }

        protected override Diagnostic AnalyzeStartContinue(ISymbol symbol, IEnumerable<SyntaxNode> remainingNodes)
        {
            var node = remainingNodes.ElementAtOrDefault(0);

            if (node == null)
            {
                // we are at the end, so nothing to claim about
                return null;
            }

            if (node.IsSeeCref())
            {
                // skip over the complete <see cref="" /> node
                var continueNode = remainingNodes.Skip(1).SkipWhile(_ => _.Ancestors().Contains(node)).ElementAtOrDefault(0);

                if (continueNode is XmlTextSyntax text)
                {
                    foreach (var token in text.TextTokens)
                    {
                        var continueText = token.ValueText;

                        if (continueText.IsNullOrWhiteSpace())
                        {
                            continue;
                        }

                        if (continueText.StartsWithAny(EndingPhrases))
                        {
                            // no issue
                            return null;
                        }

                        // we found an issue
                        break;
                    }
                }
            }

            return Issue(symbol.Name, node, Proposal);
        }
    }
}