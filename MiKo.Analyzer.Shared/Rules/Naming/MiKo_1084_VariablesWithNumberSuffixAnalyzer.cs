using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1084_VariablesWithNumberSuffixAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1084";

        public MiKo_1084_VariablesWithNumberSuffixAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(ISymbol symbol) => symbol.Name.WithoutNumberSuffix();

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.Name.EndsWithNumber();

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers) => from identifier in identifiers
                                                                                                                                        let name = identifier.ValueText
                                                                                                                                        where name.EndsWithCommonNumber()
                                                                                                                                        select Issue(name, identifier);
    }
}