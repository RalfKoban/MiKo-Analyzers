using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1531_VariablesWithShouldPrefixAnalyzer : LocalVariableNamingAnalyzer
    {
        public const string Id = "MiKo_1531";

        public MiKo_1531_VariablesWithShouldPrefixAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            List<Diagnostic> issues = null;

            for (int index = 0, length = identifiers.Length; index < length; index++)
            {
                var identifier = identifiers[index];
                var name = identifier.ValueText;

                if (name.StartsWithAny(Constants.Names.IntentPrefixes, StringComparison.OrdinalIgnoreCase))
                {
                    var betterName = FindBetterNameForShouldPrefix(name);

                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(name, identifier, betterName, CreateBetterNameProposal(betterName)));
                }
            }

            return issues ?? Enumerable.Empty<Diagnostic>();
        }
    }
}