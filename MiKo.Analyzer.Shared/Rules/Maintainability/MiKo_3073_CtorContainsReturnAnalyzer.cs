using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3073_CtorContainsReturnAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3073";

        public MiKo_3073_CtorContainsReturnAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsConstructor();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            return symbol.GetSyntax()
                         .DescendantNodes<ReturnStatementSyntax>(_ => _.Ancestors<LambdaExpressionSyntax>().None()) // filter callbacks inside constructors
                         .Select(_ => Issue(methodName, _));
        }
    }
}