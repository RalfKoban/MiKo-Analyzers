using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1068_WorkflowMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1068";

        public MiKo_1068_WorkflowMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.DeclaredAccessibility == Accessibility.Public && symbol.ContainingType.Name.EndsWith("Workflow", StringComparison.OrdinalIgnoreCase);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var proposedMethodName = GetProposedMethodName(symbol);

            if (symbol.Name == proposedMethodName)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return new[] { Issue(symbol, proposedMethodName) };
        }

        private static string GetProposedMethodName(IMethodSymbol method)
        {
            var returnType = method.ReturnType;

            if (returnType.IsBoolean())
            {
                return "CanRun";
            }

            var isTask = returnType.IsTask();

            if (isTask is false)
            {
                return "Run";
            }

            var isBoolean = returnType.TryGetGenericArgumentType(out var argumentType) && argumentType.IsBoolean();

            return isBoolean
                   ? "CanRunAsync"
                   : "RunAsync";
        }
    }
}