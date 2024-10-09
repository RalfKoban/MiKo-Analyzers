using System.Collections.Generic;
using System.Composition;
using System.Linq;

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

                    if (argument.Expression is InterpolatedStringExpressionSyntax interpolated)
                    {
                        var interpolations = interpolated.Contents.OfType<InterpolationSyntax>().ToList();

                        // filter for format clauses as they are not needed inside the resulting text
                        var formatClauses = interpolations.Select(_ => _.FormatClause).Where(_ => _ != null);

                        var text = interpolated.Without(formatClauses).Contents.ToString();

                        // find interpolated arguments and convert them
                        var argumentsFromInterpolation = interpolations.ToArray(_ => Argument(ConvertToExpression(_)));

                        return argumentList.ReplaceNode(argument, Argument(StringLiteral(text)))
                                           .AddArguments(argumentsFromInterpolation);
                    }
                }
            }

            return syntax;
        }

        private static ExpressionSyntax ConvertToExpression(InterpolationSyntax syntax)
        {
            var clause = syntax.FormatClause;

            return clause is null
                   ? syntax.Expression
                   : Invocation(SimpleMemberAccess(syntax.Expression, nameof(ToString)), Argument(StringLiteral(clause.FormatStringToken.ValueText)));
        }
    }
}