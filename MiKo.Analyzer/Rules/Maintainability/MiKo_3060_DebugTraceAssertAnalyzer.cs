﻿using System.Collections.Generic;
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

        private const string DebugInvocation = nameof(Debug) + "." + nameof(Debug.Assert);

        private const string TraceInvocation = nameof(Trace) + "." + nameof(Trace.Assert);

        public MiKo_3060_DebugTraceAssertAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol)
        {
            var methodCalls = symbol.GetSyntax().DescendantNodes().OfType<MemberAccessExpressionSyntax>();

            List<Diagnostic> diagnostics = null;
            foreach (var methodCall in methodCalls)
            {
                var call = methodCall.ToCleanedUpString();
                switch (call)
                {
                    case "System.Diagnostics." + DebugInvocation:
                    case "System.Diagnostics." + TraceInvocation:
                    case DebugInvocation:
                    case TraceInvocation:
                        if (diagnostics is null)
                        {
                            diagnostics = new List<Diagnostic>(1);
                        }

                        diagnostics.Add(Issue(symbol.Name, methodCall, call));
                        break;
                }
            }

            return diagnostics ?? Enumerable.Empty<Diagnostic>();
        }
    }
}