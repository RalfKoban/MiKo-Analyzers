using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.MethodKind == MethodKind.Ordinary && !symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol method)
        {
            if (method.IsInterfaceImplementation())
            {
                return Enumerable.Empty<Diagnostic>();
            }

            switch (method.Parameters.Length)
            {
                case 1 when method.Name == nameof(IDisposable.Dispose) && method.Parameters[0].Name == "disposing":
                case 2 when method.HasDependencyObjectParameter():
                    return Enumerable.Empty<Diagnostic>();
            }

            return method.Parameters
                         .Where(_ => _.Type.IsBoolean())
                         .Select(_ => (ParameterSyntax)_.DeclaringSyntaxReferences[0].GetSyntax())
                         .Select(_ => Issue(_.Identifier.ValueText, _.Type.GetLocation()))
                         .ToList();
        }
    }
}