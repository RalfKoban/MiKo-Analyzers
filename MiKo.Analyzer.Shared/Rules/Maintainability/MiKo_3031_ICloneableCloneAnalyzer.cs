using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3031_ICloneableCloneAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3031";

        public MiKo_3031_ICloneableCloneAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnType.SpecialType == SpecialType.System_Object && symbol.IsStatic is false;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            switch (symbol.Name)
            {
                case nameof(ICloneable.Clone):
                case nameof(ICloneable) + "." + nameof(ICloneable.Clone):
                case nameof(System) + "." + nameof(ICloneable) + "." + nameof(ICloneable.Clone):
                    return new[] { Issue(symbol) };

                default:
                    return Enumerable.Empty<Diagnostic>();
            }
        }
    }
}