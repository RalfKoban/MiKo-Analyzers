using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1508_VariablesWithStructuralDesignPatternSuffixAnalyzer : LocalVariableNamingAnalyzer
    {
        public const string Id = "MiKo_1508";

        private static readonly Dictionary<string, string> StructuralDesignPatternNames = new Dictionary<string, string>
                                                                                          {
                                                                                              { Constants.Names.Patterns.Adapter, "adapted" },
                                                                                              { Constants.Names.Patterns.Wrapper, "wrapped" },
                                                                                              { Constants.Names.Patterns.Decorator, "decorated" },
                                                                                          };

        public MiKo_1508_VariablesWithStructuralDesignPatternSuffixAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            var length = identifiers.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    var identifier = identifiers[index];
                    var name = identifier.ValueText;

                    if (name.EndsWithAny(StructuralDesignPatternNames.Keys, StringComparison.OrdinalIgnoreCase))
                    {
                        var betterName = FindBetterName(name);

                        yield return Issue(name, identifier, betterName, CreateBetterNameProposal(betterName));
                    }
                }
            }
        }

        private static string FindBetterName(string name)
        {
            foreach (var pair in StructuralDesignPatternNames)
            {
                if (name.EndsWith(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    var count = name.Length - pair.Key.Length;

                    if (count is 0)
                    {
                        return pair.Value;
                    }

                    return pair.Value.AsCachedBuilder()
                                     .Append(name, 0, count)
                                     .ToUpperCaseAt(pair.Value.Length)
                                     .ToString();
                }
            }

            return name;
        }
    }
}