using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MiKo_1506_VariablesWithCounterSuffixAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1506";

        public MiKo_1506_VariablesWithCounterSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => base.ShallAnalyze(symbol) && symbol.TypeKind == TypeKind.Struct;

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            var length = identifiers.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    var identifier = identifiers[index];
                    var name = identifier.ValueText;

                    if (name.EndsWith("Counter", StringComparison.OrdinalIgnoreCase))
                    {
                        var betterName = "counted" + name.WithoutSuffix("Counter").ToUpperCaseAt(0);

                        yield return Issue(name, identifier, betterName, CreateBetterNameProposal(betterName));
                    }
                }
            }
        }
    }
}