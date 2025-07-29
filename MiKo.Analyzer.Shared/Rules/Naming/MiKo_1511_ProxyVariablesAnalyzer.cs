using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1511_ProxyVariablesAnalyzer : LocalVariableNamingAnalyzer
    {
        public const string Id = "MiKo_1511";

        public MiKo_1511_ProxyVariablesAnalyzer() : base(Id)
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

                    if (name.Contains("Proxy", StringComparison.OrdinalIgnoreCase))
                    {
                        var betterName = FindBetterName(name);

                        if (betterName != name)
                        {
                            yield return Issue(name, identifier, betterName, CreateBetterNameProposal(betterName));
                        }
                    }
                }
            }
        }

        private static string FindBetterName(string name) => name.Length > 5
                                                             ? name.Without("proxy").Without("Proxy").ToLowerCaseAt(0) // simply remove both as we need to check them anyway (so we save some calls)
                                                             : name;
    }
}