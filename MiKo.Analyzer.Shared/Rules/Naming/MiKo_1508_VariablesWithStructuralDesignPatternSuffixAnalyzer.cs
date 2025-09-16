using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1508_VariablesWithStructuralDesignPatternSuffixAnalyzer : LocalVariableNamingAnalyzer
    {
        public const string Id = "MiKo_1508";

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

                    if (IsNameForStructuralDesignPattern(name))
                    {
                        var betterName = FindBetterNameForStructuralDesignPattern(name);

                        yield return Issue(name, identifier, betterName, CreateBetterNameProposal(betterName));
                    }
                }
            }
        }
    }
}