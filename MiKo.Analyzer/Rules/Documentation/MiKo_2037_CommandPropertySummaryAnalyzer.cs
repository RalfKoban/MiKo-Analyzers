using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2037_CommandPropertySummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2037";

        internal const string GetterSetterSummaryStartingPhrase = "Gets or sets the ";
        internal const string GetterOnlySummaryStartingPhrase = "Gets the ";
        internal const string SetterOnlySummaryStartingPhrase = "Sets the ";
        internal const string ContinuePhrase = " that can ";

        public MiKo_2037_CommandPropertySummaryAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override bool ShallAnalyze(IPropertySymbol symbol) => symbol.Type.IsCommand() && base.ShallAnalyze(symbol);

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, GetProposal(symbol));

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var startPhrase = GetStartingPhrase(symbol);

            if (textToken.ValueText.StartsWith(startPhrase))
            {
                return null;
            }

            return Issue(symbol.Name, textToken, GetProposal(symbol));
        }

        protected override Diagnostic AnalyzeSummaryContinue(ISymbol symbol, IEnumerable<SyntaxNode> remainingNodes)
        {
            var node = remainingNodes.FirstOrDefault();

            if (node is XmlTextSyntax text)
            {
                foreach (var token in text.TextTokens)
                {
                    if (token.ValueText.IsNullOrWhiteSpace())
                    {
                        continue;
                    }

                    if (token.ValueText.StartsWith(ContinuePhrase))
                    {
                        return null;
                    }

                    break;
                }
            }

            return (node != null)
                    ? Issue(symbol.Name, node, GetProposal(symbol))
                    : Issue(symbol, GetProposal(symbol));
        }

        private static string GetStartingPhrase(ISymbol symbol)
        {
            switch (symbol)
            {
                case IPropertySymbol p when p.IsWriteOnly: return SetterOnlySummaryStartingPhrase;
                case IPropertySymbol p when p.IsReadOnly: return GetterOnlySummaryStartingPhrase;
                default: return GetterSetterSummaryStartingPhrase;
            }
        }

        private static string GetProposal(ISymbol symbol) => GetStartingPhrase(symbol) + @"<see cref=""ICommand""/>" + ContinuePhrase;
    }
}