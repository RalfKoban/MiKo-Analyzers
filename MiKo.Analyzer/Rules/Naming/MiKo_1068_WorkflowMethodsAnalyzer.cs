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

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var proposedMethodName = GetProposedMethodName(method);

            return method.Name != proposedMethodName
                       ? new[] { Issue(method, proposedMethodName) }
                       : Enumerable.Empty<Diagnostic>();
        }

        private static string GetProposedMethodName(IMethodSymbol method)
        {
            var returnType = method.ReturnType;

            if (returnType.SpecialType == SpecialType.System_Boolean)
                return "CanRun";

            var isTask = returnType.IsTask();
            if (isTask is false)
                return "Run";

            return returnType.TryGetGenericArgumentType(out var argumentType) && argumentType.SpecialType == SpecialType.System_Boolean
                       ? "CanRunAsync"
                       : "RunAsync";
        }
    }
}