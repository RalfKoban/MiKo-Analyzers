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

        // Ordinal is important here as we want to have only those with a suffix and not all (e.g. we want 'comparisonView' but not 'view')
        private const StringComparison Comparison = StringComparison.Ordinal;

        private static readonly string[] AcceptedPrefixes =
                                                            {
                                                                "old",
                                                                "new",
                                                                "source",
                                                                "target",
                                                                "xml",
                                                            };

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

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            foreach (var identifier in identifiers)
            {
                var name = identifier.ValueText;

                if (name.StartsWithAny(AcceptedPrefixes, Comparison))
                {
                    continue;
                }

                var location = identifier.GetLocation();

                if (name.EndsWith(Constants.Entity, Comparison))
                {
                    var betterName = name.WithoutSuffix(Constants.Entity);

                    yield return Issue(name, location, betterName, CreateBetterNameProposal(betterName));
                }
                else if (name.EndsWith(Constants.Element, Comparison))
                {
                    var betterName = name == Constants.frameworkElement ? Constants.element : name.WithoutSuffix(Constants.Element);

                    yield return Issue(name, location, betterName, CreateBetterNameProposal(betterName));
                }
                else
                {
                    foreach (var pair in WrongSuffixes)
                    {
                        if (name.EndsWith(pair.Key, Comparison))
                        {
                            var betterName = pair.Value;

                            yield return Issue(name, location, betterName, CreateBetterNameProposal(betterName));
                        }
                    }
                }
            }
        }
    }
}