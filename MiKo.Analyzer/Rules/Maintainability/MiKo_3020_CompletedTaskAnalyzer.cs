using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3020_CompletedTaskAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3020";

        private const string Invocation = nameof(Task) + "." + nameof(Task.FromResult);

        public MiKo_3020_CompletedTaskAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            var returnType = method.ReturnType;

            if (returnType.IsTask() && (returnType as INamedTypeSymbol)?.TypeArguments.Length == 0)
            {
                // we have a plain task
                return method.DeclaringSyntaxReferences
                             .SelectMany(_ => _.GetSyntax().DescendantNodes())
                             .OfType<MemberAccessExpressionSyntax>()
                             .Where(_ => _.ToCleanedUpString() == Invocation)
                             .Select(_ => _.GetEnclosing<InvocationExpressionSyntax>().GetLocation())
                             .Select(_ => ReportIssue(Invocation, _))
                             .ToList();
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}