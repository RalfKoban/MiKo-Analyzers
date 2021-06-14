using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3202_AssertStatementSurroundedByBlankLinesAnalyzer : CallSurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_3202";

        public MiKo_3202_AssertStatementSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override bool IsCall(ITypeSymbol type) => IsCall(type.Name);

        private static bool IsCall(string typeName)
        {
            if (typeName is null)
            {
                return false;
            }

            if (typeName.EndsWith("Assert", StringComparison.Ordinal) is true)
            {
                return true;
            }

            if (typeName.EndsWith("Assume", StringComparison.Ordinal) is true)
            {
                return true;
            }

            return false;
        }
    }
}