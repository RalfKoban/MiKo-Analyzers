using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3016_CodeFixProvider)), Shared]
    public sealed class MiKo_3016_CodeFixProvider : ObjectCreationExpressionMaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3016";

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

        protected override TypeSyntax GetUpdatedSyntaxType(ObjectCreationExpressionSyntax syntax)
        {
            var parameter = syntax.GetUsedParameter();
            var exceptionName = parameter is null
                                ? nameof(InvalidOperationException)
                                : nameof(ArgumentException);

            return exceptionName.AsTypeSyntax();
        }

        protected override ArgumentListSyntax GetUpdatedArgumentListSyntax(ObjectCreationExpressionSyntax syntax)
        {
            var parameter = syntax.GetUsedParameter();
            var errorMessage = GetUpdatedErrorMessage(syntax.ArgumentList);

            return parameter != null
                   ? ArgumentList(errorMessage, ParamName(parameter))
                   : ArgumentList(errorMessage);
        }

        private static StatementSyntax GetUpdatedSyntax(ExpressionStatementSyntax syntax)
        {
            if (syntax.Expression is InvocationExpressionSyntax invocation)
            {
                var argument = invocation.ArgumentList.Arguments[0];
                var expression = argument.Expression;

                switch (invocation.GetName())
                {
                    case "ThrowIfNull":
                        return IfNullThrow(expression);

                    case "ThrowIfNullOrEmpty":
                        return IfMemberThrow(nameof(string.IsNullOrEmpty), expression);

                    case "ThrowIfNullOrWhiteSpace":
                        return IfMemberThrow(nameof(string.IsNullOrWhiteSpace), expression);
                }
            }

            return syntax;
        }

        private static IfStatementSyntax IfMemberThrow(string methodName, ExpressionSyntax expression)
        {
            if (expression is IdentifierNameSyntax)
            {
                return IfMemberThrow(methodName, expression, nameof(InvalidOperationException), ToDo());
            }

            var identifier = expression.GetStartingIdentifierName();

            if (identifier.IsNullOrEmpty())
            {
                return IfMemberThrow(methodName, expression, nameof(InvalidOperationException), ToDo());
            }

            return IfMemberThrow(methodName, expression, nameof(ArgumentException), ToDo(), ParamName(identifier));
        }

        private static IfStatementSyntax IfMemberThrow(string methodName, ExpressionSyntax expression, string exceptionName, params ArgumentSyntax[] arguments)
        {
            var invocation = Invocation(Member(PredefinedType(SyntaxKind.StringKeyword), methodName), Argument(expression));

            return IfThrow(invocation, exceptionName, arguments);
        }

        private static IfStatementSyntax IfNullThrow(ExpressionSyntax expression)
        {
            if (expression is IdentifierNameSyntax)
            {
                return IfNullThrow(expression, nameof(InvalidOperationException), ToDo());
            }

            var identifier = expression.GetStartingIdentifierName();

            if (identifier.IsNullOrEmpty())
            {
                return IfNullThrow(expression, nameof(InvalidOperationException), ToDo());
            }

            return IfNullThrow(expression, nameof(ArgumentException), ToDo(), ParamName(identifier));
        }

        private static IfStatementSyntax IfNullThrow(ExpressionSyntax expression, string exceptionName, params ArgumentSyntax[] arguments)
        {
            return IfThrow(IsNullPattern(expression), exceptionName, arguments);
        }
    }
}