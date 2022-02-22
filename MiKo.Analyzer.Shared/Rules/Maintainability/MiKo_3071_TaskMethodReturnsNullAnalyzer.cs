using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3071_TaskMethodReturnsNullAnalyzer : MethodReturnsNullAnalyzer
    {
        public const string Id = "MiKo_3071";

        public MiKo_3071_TaskMethodReturnsNullAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnsVoid is false && symbol.IsAsync is false && symbol.ReturnType.IsTask();
    }
}