
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3051_DependencyPropertyRegisterAnalyzer : DependencyPropertyRegisterMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3051";

        public MiKo_3051_DependencyPropertyRegisterAnalyzer() : base(Id, Constants.Invocations.DependencyProperty.Register)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyProperty();
    }
}