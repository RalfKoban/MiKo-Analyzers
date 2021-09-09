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
            var comment = symbol.GetComment();

            return from item in Items
                   where comment.Contains(item.Key, StringComparison.OrdinalIgnoreCase)
                   select Issue(symbol, item.Key, item.Value);
        }

        private static KeyValuePair<string, string>[] CreateItems()
        {
            var results = new Dictionary<string, string>();

            var items = new[] { "true", "false", "null" };
            var attributes = new[] { Constants.XmlTag.Attribute.Langref, "langowrd", "langwrod", "langwowd" }; // find typos

            foreach (var item in items)
            {
                var proposal = $"<see {Constants.XmlTag.Attribute.Langword}=\"{item}\"/>";

                results.Add($"({item} ", $"({proposal} ");
                results.Add($"({item})", $"({proposal})");
                results.Add($" {item})", $" {proposal})");
                results.Add($" {item} ", $" {proposal} ");
                results.Add($" {item}.", $" {proposal}.");
                results.Add($" {item},", $" {proposal},");
                results.Add($" {item};", $" {proposal};");
                results.Add($"<c>{item}</c>", proposal);
                results.Add($"<value>{item}</value>", proposal);

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