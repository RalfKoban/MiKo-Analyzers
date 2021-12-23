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
    public sealed class MiKo_3021_TaskRunAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3021";

        private const string Invocation = nameof(Task) + "." + nameof(Task.Run);

        public MiKo_3021_TaskRunAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnType.IsTask();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            var descendantNodes = symbol.GetSyntax().DescendantNodes();

            foreach (var taskRunExpression in descendantNodes.OfType<MemberAccessExpressionSyntax>().Where(_ => _.ToCleanedUpString() == Invocation))
            {
                var expression = taskRunExpression.GetEnclosing<InvocationExpressionSyntax>();
                var node = expression.GetEnclosing(SyntaxKind.AwaitExpression, SyntaxKind.ReturnStatement, SyntaxKind.VariableDeclarator, SyntaxKind.ArrowExpressionClause);
                switch (node?.Kind())
                {
                    case SyntaxKind.ReturnStatement:
                    case SyntaxKind.ArrowExpressionClause:
                        yield return ReportIssue(taskRunExpression, methodName);

                        break;

                    case SyntaxKind.VariableDeclarator:
                        var variable = (VariableDeclaratorSyntax)node;
                        var variableName = variable.GetName();

                        foreach (var unused in descendantNodes
                                               .OfType<ReturnStatementSyntax>()
                                               .Select(_ => _.Expression)
                                               .OfType<IdentifierNameSyntax>()
                                               .Where(_ => _.GetName() == variableName))
                        {
                            yield return ReportIssue(taskRunExpression, methodName);
                        }

                        break;
                }
            }
        }

        private Diagnostic ReportIssue(CSharpSyntaxNode expression, string methodName) => Issue(Invocation, expression, methodName);
    }
}