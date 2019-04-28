using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3032_ICloneableCloneAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3032";

        public MiKo_3032_ICloneableCloneAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnType.SpecialType == SpecialType.System_Object;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol method)
        {
            switch (method.Name)
            {
                case nameof(ICloneable.Clone):
                case nameof(ICloneable) + "." + nameof(ICloneable.Clone):
                case nameof(System) + "." + nameof(ICloneable) + "." + nameof(ICloneable.Clone):
                    return new[] { Issue(method) };

                default:
                    return Enumerable.Empty<Diagnostic>();
            }
        }
    }
}