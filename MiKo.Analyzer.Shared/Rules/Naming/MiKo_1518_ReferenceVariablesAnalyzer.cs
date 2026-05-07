using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1518_ReferenceVariablesAnalyzer : LocalVariableNamingAnalyzer
    {
        public const string Id = "MiKo_1518";

        public MiKo_1518_ReferenceVariablesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            List<Diagnostic> issues = null;

            for (int index = 0, length = identifiers.Length; index < length; index++)
            {
                var identifier = identifiers[index];
                var name = identifier.ValueText;

                if (name is "reference" || name is "references")
                {
                    // currently, we cannot rename them
                    continue;
                }

                if (name.Contains("Reference", StringComparison.OrdinalIgnoreCase))
                {
                    var betterName = FindBetterName(name);

                    if (betterName != name)
                    {
                        if (issues is null)
                        {
                            issues = new List<Diagnostic>(1);
                        }

                        issues.Add(Issue(name, identifier, betterName, CreateBetterNameProposal(betterName)));
                    }
                }
            }

            return issues ?? Enumerable.Empty<Diagnostic>();
        }

        private static string FindBetterName(string name) => name.Length > 9
                                                             ? name.AsCachedBuilder().Without("reference").Without("Reference").ToLowerCaseAt(0).ToStringAndRelease() // simply remove both as we need to check them anyway (so we save some calls)
                                                             : name;
    }
}