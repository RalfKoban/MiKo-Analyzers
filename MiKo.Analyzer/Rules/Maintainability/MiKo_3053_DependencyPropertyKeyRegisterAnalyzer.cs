
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3053_DependencyPropertyKeyRegisterAnalyzer : DependencyPropertyRegisterMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3053";

        public MiKo_3053_DependencyPropertyKeyRegisterAnalyzer() : base(Id, "DependencyProperty.RegisterReadOnly")
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyPropertyKey();
    }
}