using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;
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

        protected override bool ShallAnalyze(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var syntax = symbol.GetSyntax();

            List<Diagnostic> issues = null;

            foreach (var methodCall in syntax.DescendantNodes<MemberAccessExpressionSyntax>())
            {
                if (methodCall.GetName() is nameof(Debug.Assert))
                {
                    var identifierName = methodCall.GetIdentifierName();

                    if (identifierName is nameof(Debug) || identifierName is nameof(Trace))
                    {
                        if (issues is null)
                        {
                            issues = new List<Diagnostic>(1);
                        }

                        issues.Add(Issue(symbol.Name, methodCall, methodCall.ToCleanedUpString()));
                    }
                }
            }

            return issues ?? Enumerable.Empty<Diagnostic>();
        }
    }
}