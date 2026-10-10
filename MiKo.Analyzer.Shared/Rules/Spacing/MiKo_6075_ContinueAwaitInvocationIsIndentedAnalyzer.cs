using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6075_ContinueAwaitInvocationIsIndentedAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6075";

        public MiKo_6075_ContinueAwaitInvocationIsIndentedAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var issue = FindIssue(context);

            if (issue != null)
            {
                ReportDiagnostics(context, issue);
            }
        }

        private Diagnostic FindIssue(in SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation && invocation.GetName() is nameof(Task.ConfigureAwait))
            {
                if (invocation.Expression is MemberAccessExpressionSyntax maes && maes.IsSpanningMultipleLines())
                {
                    var configureAwait = maes.Name;
                    var otherCall = maes.Expression;

                    if (configureAwait.IsOnSameLineAsEndOf(otherCall))
                    {
                        var identifier = otherCall.FirstDescendant<SimpleNameSyntax>();

                        switch (identifier.Parent)
                        {
                            case InvocationExpressionSyntax _:
                            {
                                var position = identifier.GetPositionWithinEndLine();

                                return Issue(configureAwait, CreateProposalForSpaces(position - Constants.Indentation));
                            }

                            case MemberAccessExpressionSyntax other:
                            {
                                var position = other.OperatorToken.GetPositionWithinStartLine();

                                return Issue(configureAwait, CreateProposalForSpaces(position));
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}