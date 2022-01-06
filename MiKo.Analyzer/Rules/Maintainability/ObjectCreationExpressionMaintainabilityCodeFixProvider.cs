using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class ObjectCreationExpressionMaintainabilityCodeFixProvider : MaintainabilityCodeFixProvider
    {
        protected static ArgumentSyntax ToDo() => Argument(StringLiteral("TODO"));

        protected static ArgumentSyntax ParamName(ParameterSyntax parameter) => Argument(NameOf(parameter.GetName()));

        protected static ArgumentSyntax ParamName(IdentifierNameSyntax identifier) => Argument(NameOf(identifier.GetName()));

        protected static ArgumentSyntax GetUpdatedErrorMessage(IEnumerable<ArgumentSyntax> arguments)
        {
            foreach (var argument in arguments)
            {
                if (argument.Contains(' '))
                {
                    // textual (string) message
                    return argument;
                }

                if (argument.Expression is IdentifierNameSyntax)
                {
                    // const (string) message
                    return argument;
                }

                if (argument.Expression is MemberAccessExpressionSyntax)
                {
                    // localized (resource) message
                    return argument;
                }
            }

            return ToDo();
        }

        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ObjectCreationExpressionSyntax>().FirstOrDefault();

        protected sealed override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntax((ObjectCreationExpressionSyntax)syntax);

        protected virtual TypeSyntax GetUpdatedSyntaxType(ObjectCreationExpressionSyntax syntax) => syntax.Type;

        protected abstract ArgumentListSyntax GetUpdatedArgumentListSyntax(ObjectCreationExpressionSyntax syntax);

        private SyntaxNode GetUpdatedSyntax(ObjectCreationExpressionSyntax syntax)
        {
            var originalArguments = syntax.ArgumentList;
            if (originalArguments is null)
            {
                return syntax;
            }

            var arguments = GetUpdatedArgumentListSyntax(syntax);
            var type = GetUpdatedSyntaxType(syntax);

            return SyntaxFactory.ObjectCreationExpression(type, arguments, null);
        }
    }
}