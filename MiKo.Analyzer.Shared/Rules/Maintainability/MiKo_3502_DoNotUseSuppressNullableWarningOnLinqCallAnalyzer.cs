using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3502_DoNotUseSuppressNullableWarningOnLinqCallAnalyzer : DoNotUseSuppressNullableWarningAnalyzer
    {
        public const string Id = "MiKo_3502";

        private static readonly HashSet<string> LinqNames = new HashSet<string>
                                                                {
                                                                    nameof(Enumerable.FirstOrDefault),
                                                                    nameof(Enumerable.SingleOrDefault),
                                                                    nameof(Enumerable.DefaultIfEmpty),
                                                                    nameof(Enumerable.ElementAtOrDefault),
                                                                    nameof(Enumerable.LastOrDefault),
                                                                };

        public MiKo_3502_DoNotUseSuppressNullableWarningOnLinqCallAnalyzer() : base(Id)
        {
        }

        protected override bool HasIssue(PostfixUnaryExpressionSyntax warningExpression) => warningExpression.Operand is InvocationExpressionSyntax invocation
                                                                                         && LinqNames.Contains(invocation.Expression.GetName());
    }
}