﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3020_CompletedTaskAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3020";

        private static readonly Func<ExpressionSyntax, bool> IsLambda = IsLambdaCore;

        public MiKo_3020_CompletedTaskAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnType.IsTask() && symbol.ReturnType.IsGeneric() is false; // allow only plain tasks

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            foreach (var invocation in symbol.GetSyntax()
                                             .DescendantNodes<MemberAccessExpressionSyntax>(_ => _.IsKind(SyntaxKind.SimpleMemberAccessExpression) && _.Expression.GetName() == nameof(Task) && _.GetName() == nameof(Task.FromResult))
                                             .Select(_ => _.GetEnclosing<InvocationExpressionSyntax>()))
            {
                switch (invocation.Parent?.Kind())
                {
                    case SyntaxKind.Argument:
                        continue;

                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                    case SyntaxKind.ReturnStatement:
                    {
                        // seems it is a return, so we have to inspect for lambda's to see if the type is correct
                        var expression = invocation.FirstAncestor(IsLambda);

                        // investigate into lambda return types and ignore those that are of no interest to us
                        if (expression?.GetSymbol(compilation) is IMethodSymbol lambdaMethod && ShallAnalyze(lambdaMethod) is false)
                        {
                            continue;
                        }

                        break;
                    }
                }

                yield return Issue(nameof(Task) + "." + nameof(Task.FromResult), invocation);
            }
        }

        private static bool IsLambdaCore(ExpressionSyntax node)
        {
            switch (node.RawKind)
            {
                case (int)SyntaxKind.SimpleLambdaExpression:
                case (int)SyntaxKind.ParenthesizedLambdaExpression:
                    return true;

                default:
                    return false;
            }
        }
    }
}