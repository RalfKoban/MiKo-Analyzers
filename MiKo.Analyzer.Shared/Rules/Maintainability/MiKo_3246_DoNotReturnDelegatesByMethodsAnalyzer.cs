using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3246_DoNotReturnDelegatesByMethodsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3246";

        public MiKo_3246_DoNotReturnDelegatesByMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol, Compilation compilation) => symbol.MethodKind is MethodKind.Ordinary
                                                                                            && symbol.ReturnsVoid is false
                                                                                            && base.ShallAnalyze(symbol, compilation)
                                                                                            && symbol.IsInterfaceImplementation() is false;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation) => symbol.ReturnType.IsDelegate()
                                                                                                             ? new[] { Issue(symbol) }
                                                                                                             : Array.Empty<Diagnostic>();
    }
}