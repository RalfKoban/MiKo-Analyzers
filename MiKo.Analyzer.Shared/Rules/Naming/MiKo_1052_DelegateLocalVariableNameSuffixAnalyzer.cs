using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1052_DelegateLocalVariableNameSuffixAnalyzer : LocalVariableNamingAnalyzer
    {
        public const string Id = "MiKo_1052";

        private static readonly string[] WrongNames = { "Action", "Delegate", "Func" };

        public MiKo_1052_DelegateLocalVariableNameSuffixAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers) => AnalyzeIdentifiers(identifiers);

        private IEnumerable<Diagnostic> AnalyzeIdentifiers(in ReadOnlySpan<SyntaxToken> identifiers)
        {
            List<Diagnostic> issues = null;

            for (int index = 0, length = identifiers.Length; index < length; index++)
            {
                var identifier = identifiers[index];
                var name = identifier.ValueText;

                if (name.EndsWithAny(WrongNames, StringComparison.OrdinalIgnoreCase) && name.EndsWith("ransaction", StringComparison.OrdinalIgnoreCase) is false)
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(identifier, CreateBetterNameProposal(Constants.Names.callback)));
                }
            }

            return issues ?? Enumerable.Empty<Diagnostic>();
        }
    }
}