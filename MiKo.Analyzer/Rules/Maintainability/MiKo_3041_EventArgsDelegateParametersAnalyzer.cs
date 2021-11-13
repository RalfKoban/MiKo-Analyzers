using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3041_EventArgsDelegateParametersAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3041";

        public MiKo_3041_EventArgsDelegateParametersAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ContainingType.IsEventArgs();

        protected override bool ShallAnalyze(IPropertySymbol symbol) => symbol.ContainingType.IsEventArgs();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol)
        {
            switch (symbol.MethodKind)
            {
                case MethodKind.Ordinary when symbol.IsOverride || symbol.IsInterfaceImplementation():
                    return Enumerable.Empty<Diagnostic>();

                case MethodKind.Constructor:
                case MethodKind.Ordinary:
                    return symbol.Parameters.Where(_ => _.Type.TypeKind == TypeKind.Delegate).Select(_ => Issue(_.Type)).ToList();

                default:
                    return Enumerable.Empty<Diagnostic>();
            }
        }

        protected override IEnumerable<Diagnostic> Analyze(IPropertySymbol symbol)
        {
            var returnType = symbol.GetReturnType();
            if (returnType?.TypeKind == TypeKind.Delegate)
            {
                yield return Issue(returnType);
            }
        }
    }
}