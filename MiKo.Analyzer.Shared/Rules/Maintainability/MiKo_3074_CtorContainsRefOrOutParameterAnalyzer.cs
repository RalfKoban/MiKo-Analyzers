using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3074_CtorContainsRefOrOutParameterAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3074";

        public MiKo_3074_CtorContainsRefOrOutParameterAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsConstructor();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            foreach (var parameter in symbol.Parameters)
            {
                switch (parameter.RefKind)
                {
                    case RefKind.Ref:
                        {
                            var keyword = parameter.GetSyntax().ChildTokens().First(__ => __.IsKind(SyntaxKind.RefKeyword));

                            yield return Issue(methodName, keyword, keyword.ToString());

                            break;
                        }

                    case RefKind.Out:
                        {
                            var keyword = parameter.GetSyntax().ChildTokens().First(__ => __.IsKind(SyntaxKind.OutKeyword));

                            yield return Issue(methodName, keyword, keyword.ToString());

                            break;
                        }
                }
            }
        }
    }
}
