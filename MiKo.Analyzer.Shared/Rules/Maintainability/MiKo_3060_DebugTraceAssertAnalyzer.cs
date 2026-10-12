using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3060_DebugTraceAssertAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3060";

        public MiKo_3060_DebugTraceAssertAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (symbol.ContainingType?.TypeKind is TypeKind.Class)
            {
                if (symbol.IsAbstract || symbol.IsPartialDefinition || symbol.IsExtern || symbol.IsImplicitlyDeclared)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var syntax = symbol.GetSyntax();

            var methodCalls = syntax.DescendantNodes<MemberAccessExpressionSyntax>(SyntaxKind.SimpleMemberAccessExpression);

            if (methodCalls.Count is 0)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            List<Diagnostic> issues = null;

            foreach (var methodCall in methodCalls)
            {
                if (methodCall.Is(nameof(Debug), nameof(Debug.Assert)) || methodCall.Is(nameof(Trace), nameof(Trace.Assert)))
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(symbol.Name, methodCall, methodCall.ToCleanedUpString()));
                }
            }

            return issues ?? Enumerable.Empty<Diagnostic>();
        }
    }
}