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

        private static readonly HashSet<SyntaxKind> ConditionTokens = new HashSet<SyntaxKind>
                                                                          {
                                                                              SyntaxKind.SwitchKeyword,
                                                                              SyntaxKind.IfKeyword,
                                                                              SyntaxKind.QuestionToken, // that is a "SyntaxKind.ConditionalExpression" or a "SyntaxKind.ConditionalAccessExpression"
                                                                              SyntaxKind.QuestionQuestionToken, // that is a "SyntaxKind.CoalesceExpression"
                                                                          };

        public MiKo_3102_TestMethodsHaveNoConditionsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol method)
        {
            var methodName = method.Name;

            var conditions = method.GetSyntax().DescendantTokens()
                                   .Where(_ => ConditionTokens.Contains(_.Kind()))
                                   .Select(_ => Issue(methodName, _.GetLocation()))
                                   .ToList();
            return conditions;
        }
    }
}