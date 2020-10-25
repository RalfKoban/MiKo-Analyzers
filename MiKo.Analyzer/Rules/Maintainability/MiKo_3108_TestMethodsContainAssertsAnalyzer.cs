using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MiKo_3108_TestMethodsContainAssertsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3108";

        public MiKo_3108_TestMethodsContainAssertsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol) => ContainsAssertion(symbol)
                                                                                        ? Enumerable.Empty<Diagnostic>()
                                                                                        : new[] { Issue(symbol) };

        private static bool ContainsAssertion(IMethodSymbol symbol)
        {
            var syntax = symbol.GetSyntax();

            foreach (var node in syntax.DescendantNodes().Where(_ => _.IsKind(SyntaxKind.SimpleMemberAccessExpression)).OfType<MemberAccessExpressionSyntax>())
            {
                if (node.Expression is IdentifierNameSyntax invokedClass && Constants.Names.AssertionTypes.Contains(invokedClass.GetName()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}