using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3201_LogStatementSurroundedByBlankLinesAnalyzer : CallSurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_3201";

        public MiKo_3201_LogStatementSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override bool IsCall(ITypeSymbol type) => type.Name == Constants.ILog.TypeName;
    }
}