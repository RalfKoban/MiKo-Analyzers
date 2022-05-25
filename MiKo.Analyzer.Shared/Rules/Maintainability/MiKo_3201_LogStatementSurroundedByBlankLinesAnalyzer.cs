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

        // it may happen that in some broken code Roslyn is unable to detect a type (eg. due to missing code paths), hence 'type' could be null here
        protected override bool IsCall(ITypeSymbol type) => type?.Name == Constants.ILog.TypeName;
    }
}