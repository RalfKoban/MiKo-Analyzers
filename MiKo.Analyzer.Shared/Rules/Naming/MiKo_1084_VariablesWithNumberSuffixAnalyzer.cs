using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1084_VariablesWithNumberSuffixAnalyzer : LocalVariableNamingAnalyzer
    {
        public const string Id = "MiKo_1084";

        public MiKo_1084_VariablesWithNumberSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol)
        {
            if (symbol is null)
            {
                // code seems to be obfuscated or contains no valid symbol, so ignore it silently
                return false;
            }

            return symbol.Name.EndsWithNumber();
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            List<Diagnostic> issues = null;

            for (int index = 0, length = identifiers.Length; index < length; index++)
            {
                var identifier = identifiers[index];
                var name = identifier.ValueText;

                if (name.EndsWithCommonNumber())
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(name, identifier, CreateBetterNameProposal(name.WithoutNumberSuffix())));
                }
            }

            return issues ?? Enumerable.Empty<Diagnostic>();
        }
    }
}