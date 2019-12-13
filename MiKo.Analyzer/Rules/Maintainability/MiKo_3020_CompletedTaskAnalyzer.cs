using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnType.IsTask() && (symbol.ReturnType as INamedTypeSymbol)?.TypeArguments.Length == 0; // allow only plain tasks

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol method) => method.GetSyntax()
                                                                                          .DescendantNodes()
                                                                                          .OfType<MemberAccessExpressionSyntax>()
                                                                                          .Where(_ => _.ToCleanedUpString() == Invocation)
                                                                                          .Select(_ => _.GetEnclosing<InvocationExpressionSyntax>())
                                                                                          .Where(_ => _.Parent.IsKind(SyntaxKind.Argument) is false)
                                                                                          .Select(_ => _.GetLocation())
                                                                                          .Select(_ => Issue(Invocation, _))
                                                                                          .ToList();
    }
}