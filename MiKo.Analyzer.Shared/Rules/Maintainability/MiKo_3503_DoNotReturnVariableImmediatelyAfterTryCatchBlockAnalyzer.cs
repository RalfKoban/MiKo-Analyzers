using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3503_DoNotReturnVariableImmediatelyAfterTryCatchBlockAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3503";

        public MiKo_3503_DoNotReturnVariableImmediatelyAfterTryCatchBlockAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeTryStatement, SyntaxKind.TryStatement);

        private void AnalyzeTryStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is TryStatementSyntax node)
            {
                var issue = Analyze(node);

                if (issue != null)
                {
                    context.ReportDiagnostic(issue);
                }
            }
        }

        private Diagnostic Analyze(TryStatementSyntax tryCatchBlock)
        {
            if (tryCatchBlock.PreviousSibling() is LocalDeclarationStatementSyntax declaration)
            {
                if (tryCatchBlock.NextSibling() is ReturnStatementSyntax returnStatement && returnStatement.Expression is IdentifierNameSyntax identifier)
                {
                    var identifierName = identifier.GetName();

                    if (declaration.Declaration.Variables.Any(_ => _.GetName() == identifierName))
                    {
                        var statements = tryCatchBlock.Block.Statements;
                        var statementsCount = statements.Count;

                        if (statementsCount > 0)
                        {
                            for (var index = 0; index < statementsCount; index++)
                            {
                                if (statements[index] is ExpressionStatementSyntax e && e.Expression is AssignmentExpressionSyntax assignment && assignment.IsKind(SyntaxKind.SimpleAssignmentExpression))
                                {
                                    if (assignment.Left is IdentifierNameSyntax left && left.GetName() == identifierName)
                                    {
                                        return Issue(identifier);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}