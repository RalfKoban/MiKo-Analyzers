using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3102_TestMethodsHaveNoConditionsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3102";

        public MiKo_3102_TestMethodsHaveNoConditionsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol method)
        {
            var methodName = method.Name;
            var conditions = method.DeclaringSyntaxReferences // get the syntax tree
                                   .SelectMany(_ => _.GetSyntax().DescendantNodes())
                                   .Where(IsCondition)
                                   .Select(_ => ReportIssue(methodName, _.GetLocation()))
                                   .ToList();
            return conditions;
        }

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