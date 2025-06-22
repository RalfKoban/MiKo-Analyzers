using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3086_DoNotNestConditionalExpressionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3086";

        public MiKo_3086_DoNotNestConditionalExpressionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpression, SyntaxKind.ConditionalExpression);

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (ConditionalExpressionSyntax)context.Node;

            if (node.Condition.FirstDescendant<BinaryExpressionSyntax>(SyntaxKind.CoalesceExpression) != null)
            {
                ReportDiagnostics(context, Issue(node));
            }
            else
            {
                foreach (var ancestor in node.Ancestors())
                {
                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    switch (ancestor)
                    {
                        case EqualsValueClauseSyntax _:
                        case ArrowExpressionClauseSyntax _:
                        case ReturnStatementSyntax _:
                        case ArgumentSyntax _:
                        case BaseMethodDeclarationSyntax _: // found the surrounding method
                        case LocalFunctionStatementSyntax _: // found the surrounding local function
                        case BasePropertyDeclarationSyntax _: // found the surrounding property, so we already skipped the getters or setters
                            return; // no need to search further

                        case ConditionalExpressionSyntax _:
                        {
                            ReportDiagnostics(context, Issue(node));

                            return;
                        }
                    }
                }
            }
        }
    }
}