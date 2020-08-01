using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1043_CancellationTokenLocalVariableAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1043";

        public const string ExpectedName = "token";

        public MiKo_1043_CancellationTokenLocalVariableAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol?.IsCancellationToken() is true;

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers) => from identifier in identifiers
                                                                                                                                        where identifier.ValueText != ExpectedName
                                                                                                                                        select identifier.GetSymbol(semanticModel) into symbol
                                                                                                                                        select Issue(symbol, ExpectedName);
    }
}