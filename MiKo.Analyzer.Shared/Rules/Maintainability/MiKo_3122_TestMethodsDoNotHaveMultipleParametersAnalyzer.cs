using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3122_TestMethodsDoNotHaveMultipleParametersAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3122";

        public MiKo_3122_TestMethodsDoNotHaveMultipleParametersAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.Parameters.Length > 2)
            {
                var syntax = symbol.GetSyntax<MethodDeclarationSyntax>();

                yield return Issue(syntax.ParameterList);
            }
        }
    }
}