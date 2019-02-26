using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2040_LangwordAnalyzer : OverallDocumentationAnalyzer
    {
        private static readonly KeyValuePair<string, string>[] Items  = CreateItems();

        public const string Id = "MiKo_2040";

        public MiKo_2040_LangwordAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol) => AnalyzeComment(symbol); // TODO: RKN refactor to use code from base class

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol) => AnalyzeComment(symbol); // TODO: RKN refactor to use code from base class

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol) => AnalyzeComment(symbol); // TODO: RKN refactor to use code from base class

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => AnalyzeComment(symbol); // TODO: RKN refactor to use code from base class

        protected override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol) => AnalyzeComment(symbol); // TODO: RKN refactor to use code from base class

        private IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol)
        {
            List<Diagnostic> findings = null;

            var comment = GetComment(symbol);

            foreach (var item in Items.Where(_ => comment.Contains(_.Key, StringComparison.OrdinalIgnoreCase)))
            {
                if (findings == null) findings = new List<Diagnostic>();
                findings.Add(ReportIssue(symbol, item.Key, item.Value));
            }

            return findings ?? Enumerable.Empty<Diagnostic>();
        }

        private static KeyValuePair<string, string>[] CreateItems()
        {
            var results = new Dictionary<string, string>();

            foreach (var item in new[] { "true", "false", "null" })
            {
                results.Add($"({item} ", $"(<see langword=\"{item}\"/> ");
                results.Add($"({item})", $"(<see langword=\"{item}\"/>)");
                results.Add($" {item})", $" <see langword=\"{item}\"/>)");
                results.Add($" {item} ", $" <see langword=\"{item}\"/> ");
                results.Add($" {item}.", $" <see langword=\"{item}\"/>.");
                results.Add($" {item},", $" <see langword=\"{item}\"/>,");
                results.Add($" {item};", $" <see langword=\"{item}\"/>;");
                results.Add($"<c>{item}</c>", $"<see langword=\"{item}\"/>");
            }

            return results.ToArray();
        }
    }
}