using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1506_VariablesWithCounterSuffixAnalyzer : LocalVariableNamingAnalyzer
    {
        public const string Id = "MiKo_1506";

        public MiKo_1506_VariablesWithCounterSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => base.ShallAnalyze(symbol) && symbol.TypeKind is TypeKind.Struct;

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            var length = identifiers.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    var identifier = identifiers[index];
                    var name = identifier.ValueText;

                    if (name.EndsWith(Constants.Names.Counter, StringComparison.OrdinalIgnoreCase))
                    {
                        var betterName = FindBetterName(name);

                        yield return Issue(name, identifier, betterName, CreateBetterNameProposal(betterName));
                    }
                }
            }
        }

        private static string FindBetterName(string name)
        {
            if (name.Equals(Constants.Names.Counter, StringComparison.OrdinalIgnoreCase))
            {
                return "count";
            }

            return "counted" + Pluralizer.MakePluralName(name.WithoutSuffix(Constants.Names.Counter).ToUpperCaseAt(0));
        }
    }
}