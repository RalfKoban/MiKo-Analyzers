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

        internal static readonly string[] Phrases = { "true", "false", "null" };

        internal static readonly HashSet<string> WrongAttributes = new HashSet<string>
                                                                       {
                                                                           Constants.XmlTag.Attribute.Langref,
                                                                           "langowrd", // find typos
                                                                           "langwrod", // find typos
                                                                           "langwowd", // find typos
                                                                       };

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

            foreach (var phrase in Phrases)
            {
                var proposal = $"<see {Constants.XmlTag.Attribute.Langword}=\"{phrase}\"/>";

                results.Add($"({phrase} ", $"({proposal} ");
                results.Add($"({phrase})", $"({proposal})");
                results.Add($" {phrase})", $" {proposal})");
                results.Add($" {phrase} ", $" {proposal} ");
                results.Add($" {phrase}.", $" {proposal}.");
                results.Add($" {phrase}?", $" {proposal}.");
                results.Add($" {phrase}!", $" {proposal}.");
                results.Add($" {phrase},", $" {proposal},");
                results.Add($" {phrase};", $" {proposal};");
                results.Add($"<b>{phrase}</b>", proposal);
                results.Add($"<c>{phrase}</c>", proposal);
                results.Add($"<value>{phrase}</value>", proposal);

                foreach (var attribute in WrongAttributes)
                {
                    results.Add($"<see {attribute}=\"{phrase}\"/>", proposal);
                    results.Add($"<see {attribute}=\"{phrase}\" />", proposal);
                    results.Add($"<see {attribute}=\"{phrase}\"></see>", proposal);
                    results.Add($"<see {attribute}=\"{phrase}\" ></see>", proposal);
                    results.Add($"<seealso {attribute}=\"{phrase}\"/>", proposal);
                    results.Add($"<seealso {attribute}=\"{phrase}\" />", proposal);
                    results.Add($"<seealso {attribute}=\"{phrase}\"></seealso>", proposal);
                    results.Add($"<seealso {attribute}=\"{phrase}\" ></seealso>", proposal);
                }
            }

            return results.ToArray();
        }
    }
}