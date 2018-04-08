using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3102_TestMethodsHaveNoConditionsAnalyzer : TestsMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3102";

        public MiKo_3102_TestMethodsHaveNoConditionsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeTestType(INamedTypeSymbol symbol)
        {
            var testMethods = GetTestMethods(symbol).ToList();

            return testMethods.Any()
                       ? testMethods.SelectMany(AnalyzeMethod).ToList()
                       : Enumerable.Empty<Diagnostic>();
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method) => method.DeclaringSyntaxReferences // get the syntax tree
                                                                                                .SelectMany(_ => _.GetSyntax().DescendantNodes(__ => true))
                                                                                                .Any(IsCondition)
                                                                                              ? new[] { ReportIssue(method) }
                                                                                              : Enumerable.Empty<Diagnostic>();

        private static bool IsCondition(SyntaxNode syntaxNode)
        {
            switch (syntaxNode.Kind())
            {
                case SyntaxKind.SwitchStatement:
                case SyntaxKind.SwitchKeyword:
                case SyntaxKind.IfStatement:
                case SyntaxKind.ConditionalExpression:
                case SyntaxKind.CoalesceExpression:
                case SyntaxKind.ConditionalAccessExpression:
                    return true;

                default:
                    return false;
            }
        }
    }
}