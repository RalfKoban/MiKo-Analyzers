using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6049_EventRegistrationsSurroundedByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_6049";

        private static readonly SyntaxKind[] PossibleEventRegistrations = { SyntaxKind.AddAssignmentExpression, SyntaxKind.SubtractAssignmentExpression };

        public MiKo_6049_EventRegistrationsSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, PossibleEventRegistrations);

        private Diagnostic AnalyzeAssignment(AssignmentExpressionSyntax assignment, SemanticModel semanticModel)
        {
            if (assignment.IsEventRegistration(semanticModel))
            {
                if (assignment.Parent?.Parent is BlockSyntax block)
                {
                    // we found an event
                    var callLineSpan = assignment.GetLocation().GetLineSpan();

                    var noBlankLinesBefore = block.Statements
                                                  .Where(_ => HasNoBlankLinesBefore(callLineSpan, _))
                                                  .Any(_ => _.IsEventRegistration(semanticModel) is false);

                    var noBlankLinesAfter = block.Statements
                                                 .Where(_ => HasNoBlankLinesAfter(callLineSpan, _))
                                                 .Any(_ => _.IsEventRegistration(semanticModel) is false);

                    if (noBlankLinesBefore || noBlankLinesAfter)
                    {
                        return Issue(assignment.OperatorToken, noBlankLinesBefore, noBlankLinesAfter);
                    }
                }
            }

            return null;
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var assignment = (AssignmentExpressionSyntax)context.Node;

            ReportDiagnostics(context, AnalyzeAssignment(assignment, context.SemanticModel));
        }
    }
}