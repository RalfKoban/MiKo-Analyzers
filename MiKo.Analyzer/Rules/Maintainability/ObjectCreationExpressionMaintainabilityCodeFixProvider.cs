using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class ObjectCreationExpressionMaintainabilityCodeFixProvider : MaintainabilityCodeFixProvider
    {
        protected static ArgumentSyntax ToDo() => Argument(StringLiteral("TODO"));

        protected static ArgumentSyntax ParamName(ParameterSyntax parameter) => Argument(NameOf(parameter.GetName()));

        protected static ParameterSyntax FindUsedParameter(ObjectCreationExpressionSyntax syntax)
        {
            var parameters = CollectParameters(syntax);
            if (parameters.Any())
            {
                // there might be multiple parameters, so we have to find out which parameter is meant
                var parameter = parameters.Count() > 1
                                    ? FindUsedParameter(syntax.ArgumentList, parameters)
                                    : parameters.First();
                return parameter;
            }

            return null;
        }

        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ObjectCreationExpressionSyntax>().FirstOrDefault();

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic) => GetUpdatedSyntax((ObjectCreationExpressionSyntax)syntax);

        protected abstract ArgumentListSyntax GetUpdatedArgumentListSyntax(ObjectCreationExpressionSyntax syntax);

        private static ParameterSyntax FindUsedParameter(SyntaxNode node, IEnumerable<ParameterSyntax> parameters)
        {
            // most probably it's a if/else, but it might be a switch statement as well
            var condition = node.GetRelatedIfStatement()?.Condition ?? node.GetEnclosing<SwitchStatementSyntax>()?.Expression;

            if (condition is null)
            {
                // nothing found
                return null;
            }

            var identifiers = condition.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Select(_ => _.GetName()).ToHashSet();

            return parameters.FirstOrDefault(_ => identifiers.Contains(_.GetName()));
        }

        private static IEnumerable<ParameterSyntax> CollectParameters(ObjectCreationExpressionSyntax syntax)
        {
            var method = syntax.GetEnclosing<BaseMethodDeclarationSyntax>();
            if (method != null)
            {
                return method.ParameterList.Parameters;
            }

            var indexer = syntax.GetEnclosing<IndexerDeclarationSyntax>();
            if (indexer != null)
            {
                var parameters = new List<ParameterSyntax>(indexer.ParameterList.Parameters);

                // 'value' is a special parameter that is not part of the parameter list
                parameters.Insert(0, Parameter(indexer.Type));

                return parameters;
            }

            var property = syntax.GetEnclosing<PropertyDeclarationSyntax>();
            if (property != null)
            {
                // 'value' is a special parameter that is not part of the parameter list
                return new[] { Parameter(property.Type) };
            }

            return Enumerable.Empty<ParameterSyntax>();
        }

        private SyntaxNode GetUpdatedSyntax(ObjectCreationExpressionSyntax syntax)
        {
            var originalArguments = syntax.ArgumentList;
            if (originalArguments is null)
            {
                return syntax;
            }

            var arguments = GetUpdatedArgumentListSyntax(syntax);

            return SyntaxFactory.ObjectCreationExpression(syntax.Type, arguments, null);
        }
    }
}