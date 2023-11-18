using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3021_TaskRunAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3021";

        private static readonly SyntaxKind[] EnclosingInvocationSyntaxKinds = {
                                                                                  SyntaxKind.AwaitExpression,
                                                                                  SyntaxKind.ReturnStatement,
                                                                                  SyntaxKind.VariableDeclarator,
                                                                                  SyntaxKind.ArrowExpressionClause,
                                                                              };

        public MiKo_3021_TaskRunAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnType.IsTask();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            var taskRunExpressions = new List<MemberAccessExpressionSyntax>();
            var identifierNames = new HashSet<string>();

            foreach (var node in symbol.GetSyntax().DescendantNodes())
            {
                switch (node)
                {
                    case MemberAccessExpressionSyntax maes when maes.Expression.GetName() == nameof(Task) && maes.GetName() == nameof(Task.Run):
                        taskRunExpressions.Add(maes);

                        break;

                    case ReturnStatementSyntax rss when rss.Expression is IdentifierNameSyntax ins:
                        identifierNames.Add(ins.GetName());

                        break;
                }
            }

            foreach (var taskRunExpression in taskRunExpressions)
            {
                var expression = taskRunExpression.GetEnclosing<InvocationExpressionSyntax>();
                var node = expression.GetEnclosing(EnclosingInvocationSyntaxKinds);
                var syntaxKind = node?.Kind();

                switch (syntaxKind)
                {
                    case SyntaxKind.ReturnStatement:
                    case SyntaxKind.ArrowExpressionClause:
                    {
                        yield return ReportIssue(taskRunExpression, methodName);

                        break;
                    }

                    case SyntaxKind.VariableDeclarator:
                    {
                        var variable = (VariableDeclaratorSyntax)node;
                        var variableName = variable.GetName();

                        foreach (var identifierName in identifierNames)
                        {
                            if (identifierName == variableName)
                            {
                                yield return ReportIssue(taskRunExpression, methodName);
                            }
                        }

                        break;
                    }
                }
            }
        }

        private Diagnostic ReportIssue(CSharpSyntaxNode expression, string methodName) => Issue(nameof(Task) + "." + nameof(Task.Run), expression, methodName);
    }
}