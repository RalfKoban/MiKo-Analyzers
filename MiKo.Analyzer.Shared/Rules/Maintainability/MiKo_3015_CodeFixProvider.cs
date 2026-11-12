using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3015_CodeFixProvider)), Shared]
    public sealed class MiKo_3015_CodeFixProvider : ObjectCreationExpressionMaintainabilityCodeFixProvider
    {
        private const string Namespace = "System.ComponentModel";

        private static readonly HashSet<string> ComponentModelTypes = typeof(InvalidEnumArgumentException).Assembly.GetTypes().Where(_ => _.Namespace == Namespace).ToHashSet(_ => _.Name);

        public override string FixableDiagnosticId => "MiKo_3015";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                switch (node)
                {
                    case ObjectCreationExpressionSyntax _:
                    case ExpressionStatementSyntax _:
                        return node;
                }
            }

            return null;
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            switch (syntax)
            {
                case ObjectCreationExpressionSyntax _:
                    return base.GetUpdatedSyntaxAsync(syntax, issue, document, cancellationToken);

                case ExpressionStatementSyntax statement: // this seems to be a '.Throw' statement
                {
                    var updatedStatement = GetUpdatedSyntax(statement);

                    return Task.FromResult<SyntaxNode>(updatedStatement);
                }

                default:
                    return Task.FromResult<SyntaxNode>(null);
            }
        }

        protected override TypeSyntax GetUpdatedSyntaxType(ObjectCreationExpressionSyntax syntax) => nameof(InvalidOperationException).AsTypeSyntax();

        protected override ArgumentListSyntax GetUpdatedArgumentListSyntax(ObjectCreationExpressionSyntax syntax)
        {
            var argumentList = syntax.ArgumentList;
            var arguments = argumentList.Arguments;

            var errorMessage = arguments.Count is 3
                                   ? GetUpdatedErrorMessage(arguments.RemoveAt(1)) // actual argument seems to be part of the exception, so we have to ignore it when trying to find the error message
                                   : GetUpdatedErrorMessage(argumentList);

            return ArgumentList(errorMessage);
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntaxRoot(root);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root)
        {
            if (root.DescendantNodes<IdentifierNameSyntax>().None(_ => ComponentModelTypes.Contains(_.GetName())))
            {
                return root.WithoutUsing(Namespace); // remove unused "using System.ComponentModel;"
            }

            return root;
        }

        private static StatementSyntax GetUpdatedSyntax(ExpressionStatementSyntax syntax)
        {
            if (syntax.Expression is InvocationExpressionSyntax invocation)
            {
                var arguments = invocation.ArgumentList.Arguments;

                switch (invocation.GetName())
                {
                    case "ThrowIfEqual": return IfThrow(SyntaxKind.IsExpression, arguments);
                    case "ThrowIfNotEqual": return IfThrow(SyntaxKind.NotEqualsExpression, arguments);
                    case "ThrowIfGreaterThan": return IfThrow(SyntaxKind.GreaterThanExpression, arguments);
                    case "ThrowIfGreaterThanOrEqual": return IfThrow(SyntaxKind.GreaterThanOrEqualExpression, arguments);
                    case "ThrowIfLessThan": return IfThrow(SyntaxKind.LessThanExpression, arguments);
                    case "ThrowIfLessThanOrEqual": return IfThrow(SyntaxKind.LessThanOrEqualExpression, arguments);
                    case "ThrowIfNegative": return IfZeroThrow(SyntaxKind.LessThanExpression, arguments);
                    case "ThrowIfNegativeOrZero": return IfZeroThrow(SyntaxKind.LessThanOrEqualExpression, arguments);
                    case "ThrowIfZero": return IfZeroThrow(SyntaxKind.IsExpression, arguments);
                    case "ThrowIfNull": return IfNullThrow(arguments);
                    case "ThrowIfNullOrEmpty": return IfMemberThrow(nameof(string.IsNullOrEmpty), arguments);
                    case "ThrowIfNullOrWhiteSpace": return IfMemberThrow(nameof(string.IsNullOrWhiteSpace), arguments);
                }
            }

            return syntax;
        }

        private static IfStatementSyntax IfMemberThrow(string methodName, in SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            var invocation = Invocation(Member(PredefinedType(SyntaxKind.StringKeyword), methodName), Argument(arguments[0].Expression));

            return IfThrow(invocation);
        }

        private static IfStatementSyntax IfNullThrow(in SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return IfThrow(IsNullPattern(arguments[0].Expression));
        }

        private static IfStatementSyntax IfThrow(ExpressionSyntax condition)
        {
            return IfThrow(condition, nameof(InvalidOperationException), ToDo());
        }

        private static IfStatementSyntax IfThrow(in SyntaxKind operation, in SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return IfThrow(operation, arguments[0].Expression, arguments[1].Expression);
        }

        private static IfStatementSyntax IfThrow(in SyntaxKind operation, ExpressionSyntax left, ExpressionSyntax right)
        {
            return IfThrow(SyntaxFactory.BinaryExpression(operation, left, right));
        }

        private static IfStatementSyntax IfZeroThrow(in SyntaxKind operation, in SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return IfThrow(operation, arguments[0].Expression, Literal(0));
        }
    }
}