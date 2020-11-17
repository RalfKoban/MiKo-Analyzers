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

        private static readonly HashSet<int> ConditionTokens = new HashSet<int>
                                                                   {
                                                                       (int)SyntaxKind.SwitchKeyword,
                                                                       (int)SyntaxKind.IfKeyword,
                                                                       (int)SyntaxKind.QuestionToken, // that is a "SyntaxKind.ConditionalExpression" or a "SyntaxKind.ConditionalAccessExpression"
                                                                       (int)SyntaxKind.QuestionQuestionToken, // that is a "SyntaxKind.CoalesceExpression"
                                                                       (int)SyntaxKind.QuestionQuestionEqualsToken, // that is a "SyntaxKind.CoalesceAssignmentExpression"
                                                                   };

        public MiKo_3102_TestMethodsHaveNoConditionsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol)
        {
            var methodName = symbol.Name;

            var conditions = symbol.GetSyntax().DescendantTokens()
                                   .Where(_ => ConditionTokens.Contains(_.RawKind))
                                   .Select(_ => Issue(methodName, _))
                                   .ToList();
            return conditions;
        }
    }
}