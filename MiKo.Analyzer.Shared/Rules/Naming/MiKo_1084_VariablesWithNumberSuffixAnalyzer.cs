using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1084_VariablesWithNumberSuffixAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1084";

        public MiKo_1084_VariablesWithNumberSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.Name.EndsWithNumber();

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            var length = identifiers.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    var identifier = identifiers[index];
                    var name = identifier.ValueText;

                    if (name.EndsWithCommonNumber())
                    {
                        yield return Issue(name, identifier, CreateBetterNameProposal(name.WithoutNumberSuffix()));
                    }
                }
            }
        }
    }
}