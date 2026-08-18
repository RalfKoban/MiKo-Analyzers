using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3130_CodeFixProvider)), Shared]
    public sealed class MiKo_3130_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3130";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<IfStatementSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken) => Task.FromResult(syntax);

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var updatedRoot = GetUpdatedSyntaxRoot(root, syntax, issue);

            return Task.FromResult(updatedRoot);
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root, SyntaxNode syntax, Diagnostic issue)
        {
            var node = root.FindNode(issue.Location.SourceSpan);

            if (syntax is IfStatementSyntax ifStatement && node is InvocationExpressionSyntax invocation && invocation.Parent is ExpressionStatementSyntax assertFailStatement)
            {
                var whenTrue = ifStatement.Statement;

                var belowIf = whenTrue.DescendantNodesAndSelf().OfType<StatementSyntax>().Any(_ => _.IsEquivalentTo(assertFailStatement));
                var isTrue = belowIf is false;

                var condition = ifStatement.Condition;

                if (condition is PrefixUnaryExpressionSyntax unary && unary.IsKind(SyntaxKind.LogicalNotExpression))
                {
                    // we are not interested in the unary not condition, so we have to switch both the condition and the 'Is' part
                    condition = unary.Operand;
                    isTrue = !isTrue;
                }

                var assert = AssertThat(Argument(condition), Is(isTrue ? "True" : "False"), invocation.ArgumentList.Arguments, 0);
                var assertStatement = Statement(assert).WithTriviaFrom(ifStatement);

                if (belowIf)
                {
                    return root.ReplaceNode(ifStatement, assertStatement);
                }

                if (whenTrue is BlockSyntax block)
                {
                    // copy the statements from the 'true' block as we still need them
                    var allStatements = block.Statements.Select(_ => _.WithIndentation()).ToSyntaxList()
                                             .Insert(0, assertStatement); // place assert first

                    return root.ReplaceNode(ifStatement, allStatements);
                }

                return root.ReplaceNode(ifStatement, new[] { assertStatement, whenTrue });
            }

            return root;
        }
    }
}