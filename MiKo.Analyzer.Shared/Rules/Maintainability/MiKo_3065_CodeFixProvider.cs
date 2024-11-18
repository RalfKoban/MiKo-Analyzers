using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3065_CodeFixProvider)), Shared]
    public sealed class MiKo_3065_CodeFixProvider : StringMaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3065";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ArgumentListSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ArgumentListSyntax argumentList)
            {
                var arguments = argumentList.Arguments;

                if (arguments.Count > 0)
                {
                    var argument = arguments[0];

                    switch (argument.Expression)
                    {
                        case InterpolatedStringExpressionSyntax interpolated:
                            return GetUpdatedSyntax(interpolated, argument, argumentList);

                        case InvocationExpressionSyntax invocation: // string.Format
                            return GetUpdatedSyntax(invocation, argument, argumentList);
                    }
                }
            }

            return syntax;
        }

        private static ArgumentListSyntax GetUpdatedSyntax(InterpolatedStringExpressionSyntax interpolated, ArgumentSyntax argument, ArgumentListSyntax argumentList)
        {
            var interpolations = interpolated.Contents.OfType<InterpolationSyntax>().ToList();

            // filter for format clauses as they are not needed inside the resulting text
            var formatClauses = interpolations.Select(_ => _.FormatClause).WhereNotNull();

            var text = interpolated.Without(formatClauses).Contents.ToString();

            // find interpolated arguments and convert them
            var argumentsFromInterpolation = interpolations.ToArray(_ => Argument(ConvertToExpression(_)));

            return argumentList.ReplaceNode(argument, Argument(StringLiteral(text)))
                               .AddArguments(argumentsFromInterpolation);
        }

        private static ArgumentListSyntax GetUpdatedSyntax(InvocationExpressionSyntax invocation, ArgumentSyntax argument, ArgumentListSyntax argumentList)
        {
            var invocationArguments = invocation.ArgumentList.Arguments;

            if (invocationArguments.Count > 0 && invocationArguments[0].Expression is LiteralExpressionSyntax literal)
            {
                var formatArguments = invocationArguments.Skip(1).ToArray();

                var updatedLiteral = UpdateStringFormatLiteral(literal, formatArguments);

                return argumentList.ReplaceNode(argument, Argument(updatedLiteral))
                                   .AddArguments(formatArguments);
            }

            return argumentList;
        }

        private static LiteralExpressionSyntax UpdateStringFormatLiteral(LiteralExpressionSyntax literal, ArgumentSyntax[] arguments)
        {
            var token = literal.Token;

            // replace {0}... with {someParameter} and use the formatArguments
            var pairs = new Pair[arguments.Length];

            for (var i = 0; i < arguments.Length; i++)
            {
                var argument = arguments[i];

                var indexString = i.ToString();
                var identifier = argument.Expression.GetIdentifierName() ?? indexString;

                pairs[i] = new Pair("{" + indexString + "}", "{" + identifier + "}");
            }

            var updatedText = token.ValueText.AsBuilder().ReplaceAllWithCheck(pairs);

            return StringLiteral(updatedText.ToString());
        }

        private static ExpressionSyntax ConvertToExpression(InterpolationSyntax syntax)
        {
            var clause = syntax.FormatClause;

            if (clause is null)
            {
                return syntax.Expression;
            }

            return Invocation(SimpleMemberAccess(syntax.Expression, nameof(ToString)), Argument(StringLiteral(clause.FormatStringToken.ValueText)));
        }
    }
}