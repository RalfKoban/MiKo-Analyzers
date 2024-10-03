using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3213_PublicDisposeMethodInvokesNonPublicDisposeMethodAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3213";

        public MiKo_3213_PublicDisposeMethodInvokesNonPublicDisposeMethodAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnsVoid
                                                                   && symbol.Parameters.Length == 0
                                                                   && symbol.Name == nameof(IDisposable.Dispose)
                                                                   && symbol.ContainingType.GetMembers(nameof(IDisposable.Dispose)).Length > 1;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var method = symbol.GetSyntax<MethodDeclarationSyntax>();

            if (method.Body is BlockSyntax block)
            {
                var statements = block.Statements;

                switch (statements.Count)
                {
                    case 1 when IsCallDisposeTrue(statements[0]):
                        return Enumerable.Empty<Diagnostic>();

                    case 2 when IsCallDisposeTrue(statements[0]) && IsCallGcFinalizeThis(statements[1]):
                        return Enumerable.Empty<Diagnostic>();
                }
            }
            else
            {
                if (IsCallDisposeTrue(method.ExpressionBody?.Expression))
                {
                    return Enumerable.Empty<Diagnostic>();
                }
            }

            return new[] { Issue(symbol) };
        }

        private static bool IsCallGcFinalizeThis(StatementSyntax statement) => statement is ExpressionStatementSyntax e && IsCallGcFinalizeThis(e.Expression);

        private static bool IsCallGcFinalizeThis(ExpressionSyntax expression)
        {
            if (expression is InvocationExpressionSyntax invocation)
            {
                var arguments = invocation.ArgumentList.Arguments;

                if (arguments.Count == 1 && arguments[0].Expression?.IsKind(SyntaxKind.ThisExpression) is true && invocation.GetIdentifierName() == nameof(GC) && invocation.GetName() == nameof(GC.SuppressFinalize))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsCallDisposeTrue(StatementSyntax statement) => statement is ExpressionStatementSyntax e && IsCallDisposeTrue(e.Expression);

        private static bool IsCallDisposeTrue(ExpressionSyntax expression)
        {
            if (expression is InvocationExpressionSyntax invocation)
            {
                var arguments = invocation.ArgumentList.Arguments;

                if (arguments.Count == 1 && arguments[0].Expression?.IsKind(SyntaxKind.TrueLiteralExpression) is true && invocation.GetName() == nameof(IDisposable.Dispose))
                {
                    return true;
                }
            }

            return false;
        }
    }
}