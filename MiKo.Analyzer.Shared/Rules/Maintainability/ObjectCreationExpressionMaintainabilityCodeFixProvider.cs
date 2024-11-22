using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class ObjectCreationExpressionMaintainabilityCodeFixProvider : MaintainabilityCodeFixProvider
    {
        protected static ArgumentSyntax ToDo() => Argument(StringLiteral(Constants.TODO));

        protected static ArgumentSyntax ParamName(ParameterSyntax parameter) => Argument(NameOf(parameter.GetName()));

        protected static ArgumentSyntax ParamName(IdentifierNameSyntax identifier) => Argument(NameOf(identifier.GetName()));

        protected static ArgumentSyntax GetUpdatedErrorMessage(ArgumentListSyntax argumentList) => GetUpdatedErrorMessage(argumentList.Arguments);

        protected static ArgumentSyntax GetUpdatedErrorMessage(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            foreach (var argument in arguments)
            {
                if (argument.Contains(' '))
                {
                    // textual (string) message
                    return argument;
                }

                switch (argument.Expression)
                {
                    case IdentifierNameSyntax _: return argument; // const (string) message
                    case MemberAccessExpressionSyntax _: return argument; // localized (resource) message
                }
            }

            return ToDo();
        }

        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ObjectCreationExpressionSyntax>().FirstOrDefault();

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntax((ObjectCreationExpressionSyntax)syntax);

        protected virtual TypeSyntax GetUpdatedSyntaxType(ObjectCreationExpressionSyntax syntax) => syntax.Type;

        protected abstract ArgumentListSyntax GetUpdatedArgumentListSyntax(ObjectCreationExpressionSyntax syntax);

        private ObjectCreationExpressionSyntax GetUpdatedSyntax(ObjectCreationExpressionSyntax syntax)
        {
            var originalArguments = syntax.ArgumentList;

            if (originalArguments is null)
            {
                return syntax;
            }

            var arguments = GetUpdatedArgumentListSyntax(syntax);

            if (arguments != originalArguments)
            {
                var type = GetUpdatedSyntaxType(syntax);

                return SyntaxFactory.ObjectCreationExpression(type, arguments, null);
            }

            return syntax;
        }
    }
}