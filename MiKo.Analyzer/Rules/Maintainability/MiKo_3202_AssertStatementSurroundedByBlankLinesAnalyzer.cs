using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3202_AssertStatementSurroundedByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_3202";

        public MiKo_3202_AssertStatementSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override bool IsCall(ITypeSymbol type) => type.Name.EndsWith("Assert", StringComparison.Ordinal);
    }
}