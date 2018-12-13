using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3103_TestMethodsDoNotUseGuidNewGuidAnalyzer : TestsMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3103";

        private const string Invocation = nameof(Guid) + "." + nameof(Guid.NewGuid);

        public MiKo_3103_TestMethodsDoNotUseGuidNewGuidAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method) => method.IsTestMethod() || method.ContainingType.IsTestClass()
                                                                                              ? AnalyzeTestMethod(method)
                                                                                              : Enumerable.Empty<Diagnostic>();

        private IEnumerable<Diagnostic> AnalyzeTestMethod(IMethodSymbol method)
        {
            var methodName = method.Name;
            var conditions = method.DeclaringSyntaxReferences // get the syntax tree
                                   .SelectMany(_ => _.GetSyntax().DescendantNodes().OfType<MemberAccessExpressionSyntax>())
                                   .Where(_ => _.ToCleanedUpString() == Invocation)
                                   .Select(_ => ReportIssue(methodName, _.GetLocation()))
                                   .ToList();
            return conditions;
        }
    }
}