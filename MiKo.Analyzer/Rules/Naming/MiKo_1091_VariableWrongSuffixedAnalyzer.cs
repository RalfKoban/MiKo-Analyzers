using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1091_VariableWrongSuffixedAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1091";

        private const StringComparison Comparison = StringComparison.Ordinal;

        private static readonly Dictionary<string, string> WrongSuffixes = new Dictionary<string, string>
                                                                               {
                                                                                   { "Comparer", "comparer" },
                                                                                   { "Editor", "editor" },
                                                                                   { "Item", "item" },
                                                                                   { "View", "view" },
                                                                               };

        public MiKo_1091_VariableWrongSuffixedAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(ISymbol symbol)
        {
            var name = symbol.Name;

            if (name.EndsWith("Entity", Comparison))
            {
                return name.WithoutSuffix("Entity");
            }

            if (name.EndsWith("Element", Comparison))
            {
                return name.WithoutSuffix("Element");
            }

            foreach (var pair in WrongSuffixes)
            {
                if (name.EndsWith(pair.Key, Comparison))
                {
                    return pair.Value;
                }
            }

            return name;
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers)
        {
            foreach (var identifier in identifiers)
            {
                var name = identifier.ValueText;
                var location = identifier.GetLocation();

                if (name.EndsWith("Entity", Comparison))
                {
                    yield return Issue(name, location, name.WithoutSuffix("Entity"));
                }
                else if (name.EndsWith("Element", Comparison))
                {
                    yield return Issue(name, location, name.WithoutSuffix("Element"));
                }
                else
                {
                    foreach (var pair in WrongSuffixes)
                    {
                        if (name.EndsWith(pair.Key, Comparison))
                        {
                            yield return Issue(name, location, pair.Value);
                        }
                    }
                }
            }
        }
    }
}