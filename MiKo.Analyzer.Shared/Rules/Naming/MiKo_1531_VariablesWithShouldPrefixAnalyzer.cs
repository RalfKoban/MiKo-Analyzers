using System;
using System.Collections.Generic;

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
            var length = identifiers.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    var identifier = identifiers[index];
                    var name = identifier.ValueText;

                    if (name.StartsWithAny(Constants.Names.IntentPrefixes, StringComparison.OrdinalIgnoreCase))
                    {
                        var betterName = FindBetterNameForShouldPrefix(name);

                        yield return Issue(name, identifier, betterName, CreateBetterNameProposal(betterName));
                    }
                }
            }
        }
    }
}