using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3011_CodeFixProvider)), Shared]
    public sealed class MiKo_3011_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3011_ArgumentExceptionsParamNameAnalyzer.Id;

        protected override string Title => Resources.MiKo_3011_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ObjectCreationExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic) => GetUpdatedSyntax((ObjectCreationExpressionSyntax)syntax);

        private static SyntaxNode GetUpdatedSyntax(ObjectCreationExpressionSyntax o)
        {
            var arguments = GetUpdatedArgumentListSyntax(o);

            return SyntaxFactory.ObjectCreationExpression(o.Type, arguments, null);
        }

        private static ArgumentListSyntax GetUpdatedArgumentListSyntax(ObjectCreationExpressionSyntax o)
        {
            var originalArguments = o.ArgumentList;
            if (originalArguments is null)
            {
                return null;
            }

            var parameters = CollectParameters(o);
            if (parameters.Any())
            {
                // there might be multiple parameters, so we have to find out which parameter is meant
                var parameter = parameters.Count() > 1
                                    ? FindUsedParameter(originalArguments, parameters)
                                    : parameters.First();

                if (parameter != null)
                {
                    switch (o.Type.GetNameOnlyPart())
                    {
                        case nameof(ArgumentException):
                            return GetUpdatedArgumentListForArgumentException(originalArguments, parameter);

                        case nameof(ArgumentNullException):
                            return GetUpdatedArgumentListForArgumentNullException(originalArguments, parameter);
                    }
                }
            }

            return originalArguments;
        }

        private static ArgumentListSyntax GetUpdatedArgumentListForArgumentException(ArgumentListSyntax originalArguments, ParameterSyntax parameter)
        {
            var arguments = originalArguments.Arguments;

            switch (arguments.Count)
            {
                case 0: // missing message, so add a TODO
                    return ArgumentList(ToDo(), ParamName(parameter));

                case 1: // it's only the message
                {
                    if (arguments[0].ToString() == parameter.GetName().SurroundedWithDoubleQuote())
                    {
                        // seems like the 'message' parameter has been misused for the parameter name
                        return ArgumentList(ToDo(), ParamName(parameter));
                    }

                    return ArgumentList(arguments[0], ParamName(parameter));
                }

                case 2: // switched message and parameter
                    return ArgumentList(arguments[1], ParamName(parameter));
            }

            return originalArguments;
        }

        private static ArgumentListSyntax GetUpdatedArgumentListForArgumentNullException(ArgumentListSyntax originalArguments, ParameterSyntax parameter)
        {
            var arguments = originalArguments.Arguments;

            switch (arguments.Count)
            {
                case 0: // missing message, so add a TODO
                    return ArgumentList(ParamName(parameter), ToDo());

                case 1: // it's only the message
                case 2: // switched message and parameter
                    return ArgumentList(ParamName(parameter), arguments[0]);
            }

            return originalArguments;
        }

        private static IEnumerable<ParameterSyntax> CollectParameters(ObjectCreationExpressionSyntax o)
        {
            var method = o.GetEnclosing<BaseMethodDeclarationSyntax>();
            if (method != null)
            {
                return method.ParameterList.Parameters;
            }

            var indexer = o.GetEnclosing<IndexerDeclarationSyntax>();
            if (indexer != null)
            {
                var parameters = new List<ParameterSyntax>(indexer.ParameterList.Parameters);

                // 'value' is a special parameter that is not part of the parameter list
                parameters.Insert(0, SyntaxFactory.Parameter(SyntaxFactory.Identifier("value")));

                return parameters;
            }

            var property = o.GetEnclosing<PropertyDeclarationSyntax>();
            if (property != null)
            {
                // 'value' is a special parameter that is not part of the parameter list
                return new[] { SyntaxFactory.Parameter(SyntaxFactory.Identifier("value")) };
            }

            return Enumerable.Empty<ParameterSyntax>();
        }

        private static ParameterSyntax FindUsedParameter(SyntaxNode node, IEnumerable<ParameterSyntax> parameters)
        {
            var ifStatement = MiKo_3011_ArgumentExceptionsParamNameAnalyzer.FindIfStatementSyntax(node);
            if (ifStatement is null)
            {
                // nothing found
                return null;
            }

            var identifiers = ifStatement.Condition.DescendantNodes().OfType<IdentifierNameSyntax>().Select(_ => _.GetName()).ToHashSet();
            var parameter = parameters.FirstOrDefault(_ => identifiers.Contains(_.GetName()));
            return parameter;
        }

        private static ArgumentSyntax ToDo() => Argument(StringLiteral("TODO"));

        private static ArgumentSyntax ParamName(ParameterSyntax parameter) => Argument(NameOf(parameter.GetName()));
    }
}