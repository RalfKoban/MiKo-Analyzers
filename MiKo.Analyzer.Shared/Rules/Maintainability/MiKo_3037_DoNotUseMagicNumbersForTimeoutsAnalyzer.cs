using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);

        private Diagnostic[] AnalyzeIssue(InvocationExpressionSyntax node, ISymbol method)
        {
            if (Names.Contains(node.GetName()))
            {
                var argument = node.ArgumentList?.Arguments.FirstOrDefault(_ => _.Expression.IsKind(SyntaxKind.NumericLiteralExpression));

                if (argument != null)
                {
                    return new[] { Issue(method.Name, argument) };
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var methodSymbol = context.GetEnclosingMethod();

            if (methodSymbol is null)
            {
                // nameof() is also a InvocationExpressionSyntax, so assignments of lists etc. may cause an NRE to be thrown
                return;
            }

            var node = (InvocationExpressionSyntax)context.Node;
            var issues = AnalyzeIssue(node, methodSymbol);

            if (issues.Length > 0)
            {
                ReportDiagnostics(context, issues);
            }
        }
    }
}