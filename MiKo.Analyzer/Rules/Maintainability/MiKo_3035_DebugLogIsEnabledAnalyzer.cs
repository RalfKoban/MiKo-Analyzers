using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3035_DebugLogIsEnabledAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3035";

        private const string Debug = "Debug";
        private const string DebugFormat = "DebugFormat";
        private const string IsDebugEnabled = "IsDebugEnabled";

        public MiKo_3035_DebugLogIsEnabledAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol method)
        {
            var methodCalls = method.DeclaringSyntaxReferences // get the syntax tree
                                    .SelectMany(_ => _.GetSyntax().DescendantNodes().OfType<MemberAccessExpressionSyntax>());

            List<Diagnostic> diagnostics = null;
            foreach (var methodCall in methodCalls)
            {
                var methodName = methodCall.Name.ToString();
                switch (methodName)
                {
                    case Debug:
                    case DebugFormat:
                    {
                        // check if inside IsDebugEnabled call for if or block
                        if (methodCall.IsInsideIfStatementWithCallTo(IsDebugEnabled))
                            continue;

                        if (diagnostics is null)
                            diagnostics = new List<Diagnostic>();

                        diagnostics.Add(ReportIssue(method.Name, methodCall.GetLocation(), methodName, IsDebugEnabled));
                        break;
                    }
                }
            }

            return diagnostics ?? Enumerable.Empty<Diagnostic>();
        }
    }
}