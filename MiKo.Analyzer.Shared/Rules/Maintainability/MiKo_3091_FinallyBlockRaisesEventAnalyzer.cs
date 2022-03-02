using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3091_FinallyBlockRaisesEventAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3091";

        public MiKo_3091_FinallyBlockRaisesEventAnalyzer() : base(Id, (SymbolKind)(-1))
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

            var issues = AnalyzeFinallyClause(context, finallyBlock);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> AnalyzeFinallyClause(SyntaxNodeAnalysisContext context, BlockSyntax finallyBlock)
        {
            var semanticModel = context.SemanticModel;

            var method = context.GetEnclosingMethod();
            var events = method.ContainingType.GetMembersIncludingInherited<IEventSymbol>().ToHashSet(_ => _.Name);

            foreach (var token in finallyBlock.DescendantTokens(SyntaxKind.IdentifierToken))
            {
                var eventName = token.ValueText;

                if (events.Contains(eventName) && token.GetSymbol(semanticModel) is IEventSymbol)
                {
                    var ancestor = token.Parent?.Parent;

                    if (ancestor is AssignmentExpressionSyntax || ancestor?.Parent is AssignmentExpressionSyntax)
                    {
                        // add or remove from event, do not report an issue here
                    }
                    else
                    {
                        yield return Issue(method.Name, token, eventName);
                    }
                }
            }
        }
    }
}