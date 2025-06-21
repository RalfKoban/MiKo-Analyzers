using System;
using System.Collections.Generic;
using System.Collections.Immutable;

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

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.MethodKind is MethodKind.Ordinary && symbol.IsTestMethod() is false;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.IsInterfaceImplementation())
            {
                return Array.Empty<Diagnostic>();
            }

            var parameters = symbol.Parameters;

            switch (parameters.Length)
            {
                case 0:
                case 1 when symbol.Name == nameof(IDisposable.Dispose) && parameters[0].Name is "disposing":
                case 2 when symbol.HasDependencyObjectParameter():
                    return Array.Empty<Diagnostic>();
            }

            return Analyze(parameters);
        }

        private IEnumerable<Diagnostic> Analyze(ImmutableArray<IParameterSymbol> parameters)
        {
            for (int index = 0, length = parameters.Length; index < length; index++)
            {
                var parameter = parameters[index];

                if (parameter.Type.IsBoolean())
                {
                    var syntax = parameter.GetSyntax();

                    yield return Issue(syntax.GetName(), syntax.Type);
                }
            }
        }
    }
}