using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1052_DelegateLocalVariableNameSuffixAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1052";

        private static readonly string[] WrongNames = { "Action", "Delegate", "Func" };

        public MiKo_1052_DelegateLocalVariableNameSuffixAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers) => AnalyzeIdentifiers(identifiers);

        private IEnumerable<Diagnostic> AnalyzeIdentifiers(IEnumerable<SyntaxToken> identifiers) => identifiers.Where(_ => _.ValueText.EndsWithAny(WrongNames) && _.ValueText.EndsWith("ransaction", StringComparison.OrdinalIgnoreCase) is false)
                                                                                                               .Select(_ => Issue(_, CreateBetterNameProposal("callback")));
    }
}