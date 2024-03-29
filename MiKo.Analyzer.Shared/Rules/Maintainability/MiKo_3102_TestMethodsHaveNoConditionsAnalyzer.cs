﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            foreach (var token in symbol.GetSyntax().DescendantTokens().Where(_ => ConditionTokens.Contains(_.RawKind)))
            {
                if (token.IsKind(SyntaxKind.QuestionToken) && token.Parent is NullableTypeSyntax)
                {
                    continue;
                }

                yield return Issue(methodName, token);
            }
        }
    }
}