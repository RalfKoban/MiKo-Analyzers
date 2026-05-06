using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1071_BooleanLocalVariableNamedAsQuestionAnalyzer : LocalVariableNamingAnalyzer
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

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol?.IsBoolean() is true;

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            List<Diagnostic> issues = null;

            for (int index = 0, length = identifiers.Length; index < length; index++)
            {
                var identifier = identifiers[index];
                var name = identifier.ValueText;

                if (name.Length <= 5)
                {
                    // skip all short names (such as isIP)
                    continue;
                }

                if (name.StartsWithAny(Prefixes) && name.HasUpperCaseLettersAbove(1) && name.StartsWith("isInDesign", StringComparison.Ordinal) is false)
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(name, identifier));
                }
            }

            return issues ?? Enumerable.Empty<Diagnostic>();
        }
    }
}