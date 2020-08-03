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

        public const string ExpectedName = "callback";

        private static readonly string[] WrongNames = { "Action", "Delegate", "Func" };

        public MiKo_1052_DelegateLocalVariableNameSuffixAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers) => AnalyzeIdentifiers(semanticModel, identifiers);

        private IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, IEnumerable<SyntaxToken> identifiers) => from identifier in identifiers
                                                                                                                                 where identifier.ValueText.EndsWithAny(WrongNames)
                                                                                                                                 select Issue(identifier.GetSymbol(semanticModel));
    }
}