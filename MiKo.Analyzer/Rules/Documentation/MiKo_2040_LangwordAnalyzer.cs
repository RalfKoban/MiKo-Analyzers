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

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml)
        {
            List<Diagnostic> findings = null;

            var comment = symbol.GetComment();

            foreach (var item in Items.Where(_ => comment.Contains(_.Key, StringComparison.OrdinalIgnoreCase)))
            {
                if (findings == null) findings = new List<Diagnostic>();
                findings.Add(Issue(symbol, item.Key, item.Value));
            }

            return findings ?? Enumerable.Empty<Diagnostic>();
        }

        private static KeyValuePair<string, string>[] CreateItems()
        {
            var results = new Dictionary<string, string>();

            foreach (var item in new[] { "true", "false", "null" })
            {
                var proposal = $"<see langword=\"{item}\"/>";

                results.Add($"({item} ", $"({proposal} ");
                results.Add($"({item})", $"({proposal})");
                results.Add($" {item})", $" {proposal})");
                results.Add($" {item} ", $" {proposal} ");
                results.Add($" {item}.", $" {proposal}.");
                results.Add($" {item},", $" {proposal},");
                results.Add($" {item};", $" {proposal};");
                results.Add($"<c>{item}</c>", proposal);
                results.Add($"<see langref=\"{item}\"/>", proposal);
                results.Add($"<see langref=\"{item}\" />", proposal);
                results.Add($"<see langref=\"{item}\"></see>", proposal);
                results.Add($"<see langref=\"{item}\" ></see>", proposal);
                results.Add($"<seealso langref=\"{item}\"/>", proposal);
                results.Add($"<seealso langref=\"{item}\" />", proposal);
                results.Add($"<seealso langref=\"{item}\"></seealso>", proposal);
                results.Add($"<seealso langref=\"{item}\" ></seealso>", proposal);
            }

            return results.ToArray();
        }
    }
}