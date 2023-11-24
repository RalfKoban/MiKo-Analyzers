using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3212_DoNotProvideUnexpectedDisposeMethodsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3212";

        public MiKo_3212_DoNotProvideUnexpectedDisposeMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Name == nameof(IDisposable.Dispose);

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var parameters = symbol.Parameters;

            switch (parameters.Length)
            {
                case 0 when symbol.ReturnsVoid:
                case 1 when symbol.ReturnsVoid && parameters[0].Type.IsBoolean():
                    return Enumerable.Empty<Diagnostic>();

                default:
                    return new[] { Issue(symbol) };
            }
        }
    }
}