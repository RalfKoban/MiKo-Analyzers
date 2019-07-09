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
        public const string Id = "MiKo_2040";

        private static readonly KeyValuePair<string, string>[] Items = CreateItems();

        public MiKo_2040_LangwordAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml)
        {
            List<Diagnostic> findings = null;

            var comment = symbol.GetComment();

            foreach (var item in Items.Where(_ => comment.Contains(_.Key, StringComparison.OrdinalIgnoreCase)))
            {
                if (findings is null)
                {
                    findings = new List<Diagnostic>(1);
                }

                findings.Add(Issue(symbol, item.Key, item.Value));
            }

            return findings ?? Enumerable.Empty<Diagnostic>();
        }

        private static KeyValuePair<string, string>[] CreateItems()
        {
            var results = new Dictionary<string, string>();

            var items = new[] { "true", "false", "null" };
            var attributes = new[] { "langref", "langowrd", "langwrod" };

            foreach (var item in items)
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

                foreach (var attribute in attributes)
                {
                    results.Add($"<see {attribute}=\"{item}\"/>", proposal);
                    results.Add($"<see {attribute}=\"{item}\" />", proposal);
                    results.Add($"<see {attribute}=\"{item}\"></see>", proposal);
                    results.Add($"<see {attribute}=\"{item}\" ></see>", proposal);
                    results.Add($"<seealso {attribute}=\"{item}\"/>", proposal);
                    results.Add($"<seealso {attribute}=\"{item}\" />", proposal);
                    results.Add($"<seealso {attribute}=\"{item}\"></seealso>", proposal);
                    results.Add($"<seealso {attribute}=\"{item}\" ></seealso>", proposal);
                }
            }

            return results.ToArray();
        }
    }
}