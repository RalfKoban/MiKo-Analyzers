using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;

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

        private static readonly SyntaxKind[] Expressions = { SyntaxKind.AddAssignmentExpression, SyntaxKind.SubtractAssignmentExpression };

        public MiKo_6049_EventRegistrationsSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, Expressions);

        private static bool IsEventRegistration(StatementSyntax statement, SyntaxNodeAnalysisContext context)
        {
            if (statement is ExpressionStatementSyntax e && e.Expression is AssignmentExpressionSyntax assignment)
            {
                return IsEventRegistration(assignment, context);
            }

            return false;
        }

        private static bool IsEventRegistration(AssignmentExpressionSyntax assignment, SyntaxNodeAnalysisContext context)
        {
            if (assignment.Right is IdentifierNameSyntax)
            {
                switch (assignment.Left)
                {
                    case MemberAccessExpressionSyntax maes:
                        return maes.GetSymbol(context.SemanticModel) is IEventSymbol;

                    case IdentifierNameSyntax identifier:
                        return identifier.GetSymbol(context.SemanticModel) is IEventSymbol;
                }
            }

            return false;
        }

        private Diagnostic AnalyzeAssignment(AssignmentExpressionSyntax assignment, SyntaxNodeAnalysisContext context)
        {
            if (IsEventRegistration(assignment, context))
            {
                if (assignment.Parent?.Parent is BlockSyntax block)
                {
                    // we found an event
                    var callLineSpan = assignment.GetLocation().GetLineSpan();

                    var noBlankLinesBefore = block.Statements
                                                  .Where(_ => HasNoBlankLinesBefore(callLineSpan, _))
                                                  .Any(_ => IsEventRegistration(_, context) is false);

                    var noBlankLinesAfter = block.Statements
                                                 .Where(_ => HasNoBlankLinesAfter(callLineSpan, _))
                                                 .Any(_ => IsEventRegistration(_, context) is false);

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

            ReportDiagnostics(context, AnalyzeAssignment(assignment, context));
        }
    }
}