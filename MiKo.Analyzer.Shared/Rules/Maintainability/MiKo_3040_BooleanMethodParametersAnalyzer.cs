using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3040_BooleanMethodParametersAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3040";

        public MiKo_3040_BooleanMethodParametersAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.MethodKind == MethodKind.Ordinary && symbol.IsTestMethod() is false;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.IsInterfaceImplementation())
            {
                return Enumerable.Empty<Diagnostic>();
            }

            switch (symbol.Parameters.Length)
            {
                case 1 when symbol.Name == nameof(IDisposable.Dispose) && symbol.Parameters[0].Name == "disposing":
                case 2 when symbol.HasDependencyObjectParameter():
                    return Enumerable.Empty<Diagnostic>();
            }

            return symbol.Parameters
                         .Where(_ => _.Type.IsBoolean())
                         .Select(_ => _.GetSyntax())
                         .Select(_ => Issue(_.GetName(), _.Type))
                         .ToList();
        }
    }
}