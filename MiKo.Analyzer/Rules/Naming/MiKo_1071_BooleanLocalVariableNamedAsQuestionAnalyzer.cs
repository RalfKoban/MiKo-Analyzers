using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1071_BooleanLocalVariableNamedAsQuestionAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1071";

        private static readonly string[] Prefixes =
            {
                "is",
                "are",
            };

        public MiKo_1071_BooleanLocalVariableNamedAsQuestionAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.SpecialType == SpecialType.System_Boolean;

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers)
        {
            foreach (var identifier in identifiers)
            {
                var name = identifier.ValueText;

                if (name.Length <= 3)
                {
                    // skip all short names
                    continue;
                }

                if (name.StartsWithAny(Prefixes, StringComparison.Ordinal) && name.HasUpperCaseLettersAbove(1))
                {
                    yield return Issue(name, identifier.GetLocation());
                }
            }
        }
    }
}