using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3103_TestMethodsDoNotUseGuidNewGuidAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3103";

        private const string Invocation = nameof(Guid) + "." + nameof(Guid.NewGuid);

        public MiKo_3103_TestMethodsDoNotUseGuidNewGuidAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsTestMethod() || symbol.ContainingType.IsTestClass();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol)
        {
            var methodName = symbol.Name;
            var conditions = symbol.GetSyntax()
                                   .DescendantNodes().OfType<MemberAccessExpressionSyntax>()
                                   .Where(_ => _.ToCleanedUpString() == Invocation)
                                   .Select(_ => Issue(methodName, _))
                                   .ToList();

            return conditions;
        }
    }
}