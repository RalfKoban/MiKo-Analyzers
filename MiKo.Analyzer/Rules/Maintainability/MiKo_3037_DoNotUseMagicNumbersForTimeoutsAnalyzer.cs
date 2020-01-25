﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3037_DoNotUseMagicNumbersForTimeoutsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3037";

        private static readonly HashSet<string> Names = new HashSet<string>
                                                            {
                                                                nameof(Task.Wait),
                                                                nameof(WaitHandle.WaitOne),
                                                                nameof(WaitHandle.WaitAll),
                                                                nameof(WaitHandle.WaitAny),
                                                                nameof(WaitHandle.SignalAndWait),
                                                                nameof(Process.WaitForExit),
                                                                nameof(Process.WaitForInputIdle),
                                                            };

        public MiKo_3037_DoNotUseMagicNumbersForTimeoutsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private IEnumerable<Diagnostic> AnalyzeIssue(MemberAccessExpressionSyntax node, ISymbol method)
        {
            if (Names.Contains(node.GetName()))
            {
                if (node.Parent is InvocationExpressionSyntax i)
                {
                    var argument = i.ArgumentList?.Arguments.FirstOrDefault(_ => _.Expression.IsKind(SyntaxKind.NumericLiteralExpression));
                    if (argument != null)
                    {
                        yield return Issue(method.Name, argument.GetLocation());
                    }
                }
            }
        }

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;
            var method = context.GetEnclosingMethod();

            foreach (var issue in AnalyzeIssue(node, method))
            {
                context.ReportDiagnostic(issue);
            }
        }
    }
}