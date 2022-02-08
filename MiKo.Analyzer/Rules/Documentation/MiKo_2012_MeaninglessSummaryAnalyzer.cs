using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2012_MeaninglessSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2012";

        public MiKo_2012_MeaninglessSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsNamespace is false && symbol.IsEnum() is false && symbol.IsException() is false && base.ShallAnalyze(symbol);

        protected override Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml)
        {
            var issue = AnalyzeSummaryStart(symbol, summaryXml);

            if (issue is null)
            {
                issue = AnalyzeSummaryContains(symbol, (XmlElementSyntax)summaryXml, Constants.Comments.MeaninglessPhrase).FirstOrDefault();
            }

            return issue;
        }

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, "start with", node.ToString().HumanizedTakeFirst(200));

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var symbolNames = GetSelfSymbolNames(symbol);
            var phrases = GetPhrases(symbol);

            foreach (var phrase in symbolNames.Concat(phrases))
            {
                var location = GetFirstLocation(textToken, phrase);
                if (location != null)
                {
                    return Issue(symbol.Name, location, "start with", textToken.ValueText.HumanizedTakeFirst(200));
                }
            }

            return null;
        }

        protected override Diagnostic SummaryContainsIssue(ISymbol symbol, Location location, string phrase) => Issue(symbol.Name, location, "contain", phrase.HumanizedTakeFirst(200));

        private static IEnumerable<string> GetPhrases(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Field: return Constants.Comments.MeaninglessFieldStartingPhrase;
                case SymbolKind.NamedType: return Constants.Comments.MeaninglessTypeStartingPhrase;
                default: return Constants.Comments.MeaninglessStartingPhrase;
            }
        }

        private static IEnumerable<string> GetSelfSymbolNames(ISymbol symbol)
        {
            var names = new List<string> { symbol.Name };

            switch (symbol)
            {
                case INamedTypeSymbol s:
                {
                    names.AddRange(s.AllInterfaces.Select(_ => _.Name));
                    break;
                }

                case ISymbol s:
                {
                    names.Add(s.ContainingType.Name);
                    names.AddRange(s.ContainingType.AllInterfaces.Select(_ => _.Name));
                    break;
                }
            }

            return names.ToHashSet(_ => _ + " ");
        }
    }
}