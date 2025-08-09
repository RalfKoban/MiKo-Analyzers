using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1092_AbilityTypeWrongSuffixedAnalyzer : TypeSyntaxNamingAnalyzer
    {
        public const string Id = "MiKo_1092";

        private static readonly string[] TypeSuffixes =
                                                        {
                                                            Constants.Element,
                                                            Constants.Entity,
                                                            "Item",
                                                            "Info",
                                                            "Information",
                                                        };

        public MiKo_1092_AbilityTypeWrongSuffixedAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(string typeName, BaseTypeDeclarationSyntax declaration) => typeName.EndsWithAny(TypeSuffixes);

        protected override Diagnostic[] AnalyzeName(string typeName, in SyntaxToken typeNameIdentifier, BaseTypeDeclarationSyntax declaration)
        {
            if (typeName.Contains("able") && typeName.EndsWithAny(TypeSuffixes))
            {
                var proposedName = GetProposedName(typeName);

                return new[] { Issue(typeNameIdentifier, proposedName) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static string GetProposedName(string name)
        {
            var proposedName = name.AsSpan();

            do
            {
                foreach (var suffix in TypeSuffixes)
                {
                    if (proposedName.EndsWith(suffix))
                    {
                        proposedName = proposedName.WithoutSuffix(suffix);
                    }
                }
            }
            while (proposedName.EndsWithAny(TypeSuffixes));

            return proposedName.ToString();
        }
    }
}