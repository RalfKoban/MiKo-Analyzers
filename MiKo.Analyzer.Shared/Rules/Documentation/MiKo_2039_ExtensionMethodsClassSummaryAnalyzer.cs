using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2039_ExtensionMethodsClassSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2039";

        internal const string StartingPhrase = "Provides a set of ";
        internal const string ContinuePhrase = " methods for ";

        private const string Proposal = StartingPhrase + "<see langword=\"static\"/>" + ContinuePhrase;

        public MiKo_2039_ExtensionMethodsClassSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.ContainsExtensionMethods() && base.ShallAnalyze(symbol);

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            if (textToken.ValueText.StartsWith(StartingPhrase))
            {
                return null;
            }

            return Issue(symbol.Name, textToken, Proposal);
        }

        protected override Diagnostic AnalyzeStartContinue(ISymbol symbol, IEnumerable<SyntaxNode> remainingNodes)
        {
            var staticNode = remainingNodes.ElementAtOrDefault(0);
            var continueNode = remainingNodes.ElementAtOrDefault(1);

            if (staticNode.IsSeeLangword("static") && continueNode is XmlTextSyntax text)
            {
                foreach (var token in text.TextTokens)
                {
                    if (token.ValueText.IsNullOrWhiteSpace())
                    {
                        continue;
                    }

                    if (token.ValueText.StartsWith(ContinuePhrase))
                    {
                        // no issue
                        return null;
                    }

                    // we found an issue
                    break;
                }
            }

            return staticNode != null
                    ? Issue(symbol.Name, staticNode, Proposal)
                    : Issue(symbol, Proposal);
        }
    }
}