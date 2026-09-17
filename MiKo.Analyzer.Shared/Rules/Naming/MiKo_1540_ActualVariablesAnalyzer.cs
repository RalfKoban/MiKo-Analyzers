using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1540_ActualVariablesAnalyzer : LocalVariableNamingAnalyzer
    {
        public const string Id = "MiKo_1540";

        private const string Prefix = "actual";

        public MiKo_1540_ActualVariablesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            List<Diagnostic> issues = null;

            for (int index = 0, length = identifiers.Length; index < length; index++)
            {
                var identifier = identifiers[index];
                var name = identifier.ValueText;

                if (name.StartsWith(Prefix, StringComparison.Ordinal))
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

        private static string FindBetterName(string name) => name.Length > Prefix.Length
                                                             ? name.AsCachedBuilder().Remove(0, Prefix.Length).ToLowerCaseAt(0).ToStringAndRelease()
                                                             : name;
    }
}