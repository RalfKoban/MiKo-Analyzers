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
        private static readonly string[] EndingPhrases = { " event.", " events." };

        public MiKo_2002_EventArgsSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsEventArgs() && base.ShallAnalyze(symbol);

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            if (textToken.ValueText.StartsWith(StartingPhrase))
            {
                return null;
            }

            return Issue(symbol.Name, textToken, Proposal);
        }

        protected override Diagnostic AnalyzeSummaryContinue(ISymbol symbol, IEnumerable<SyntaxNode> remainingNodes)
        {
            var node = remainingNodes.ElementAtOrDefault(0);
            var continueNode = remainingNodes.ElementAtOrDefault(1);

            if (node.IsSeeCref() && continueNode is XmlTextSyntax text)
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

            return node != null
                    ? Issue(symbol.Name, node, Proposal)
                    : Issue(symbol, Proposal);
        }
    }
}