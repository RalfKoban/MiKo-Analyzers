using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3090_FinallyBlockThrowsExceptionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3090";

        public MiKo_3090_FinallyBlockThrowsExceptionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeFinallyClause, SyntaxKind.FinallyClause);

        private void AnalyzeFinallyClause(SyntaxNodeAnalysisContext context)
        {
            var finallyBlock = ((FinallyClauseSyntax)context.Node).Block;
            if (finallyBlock is null)
            {
                return;
            }

            var method = context.GetEnclosingMethod();

            var issues = finallyBlock.DescendantNodesAndSelf().OfType<ThrowStatementSyntax>()
                                     .Select(_ => Issue(method, _.GetLocation()));

            ReportDiagnostics(context, issues);
        }
    }
}