using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3127_AssertThatDoesNotUseLiteralForActualAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3127";

        public MiKo_3127_AssertThatDoesNotUseLiteralForActualAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        // !!! Attention !!!:
        // Visual Studio will not allow the code fix to show up in case it is not for a location within the analyzed syntax node.
        // So, we have to register for the invocation here (instead of the simple member access) as we are interested in reporting the element of the contained argument (actually it's expression).
        // Otherwise, when we would register for the SimpleMemberAccessExpression, the argument would not belong to that access (it belongs to the invocation), and therefore it will be ignored by Visual Studio.
        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation && invocation.Is("Assert", "That"))
            {
                var issue = AnalyzeInvocationExpression(context, invocation, context.SemanticModel);

                if (issue != null)
                {
                    ReportDiagnostics(context, issue);
                }
            }
        }

        private Diagnostic AnalyzeInvocationExpression(in SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            var arguments = invocation.ArgumentList.Arguments;

            if (arguments.Count is 0)
            {
                return null;
            }

            var expression = arguments[0].Expression;

            switch (expression)
            {
                case PrefixUnaryExpressionSyntax unary when unary.Operand is LiteralExpressionSyntax:
                case LiteralExpressionSyntax _:
                case MemberAccessExpressionSyntax maes when maes.IsEnumMember(semanticModel):
                    return Issue(expression);

                case MemberAccessExpressionSyntax maes:
                    return AnalyzeAssertion(context, invocation, arguments, maes); // it looks like the values could be swapped based on other asserts, so we have to take a more costly look around

                default:
                    return null;
            }
        }

        private Diagnostic AnalyzeAssertion(in SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, in SeparatedSyntaxList<ArgumentSyntax> arguments, MemberAccessExpressionSyntax expression)
        {
            if (arguments.Count > 1 && arguments[1].Expression is InvocationExpressionSyntax constraint)
            {
                // TODO RKN: What about 'Is.Not.EqualTo' and similar checks?
                if (constraint.Is("Is", "EqualTo"))
                {
                    var constraintExpression = constraint.ArgumentList.Arguments.FirstOrDefault()?.Expression;

                    if (constraintExpression is LiteralExpressionSyntax || constraintExpression.IsConst(context))
                    {
                        // seems everything is OK
                        return null;
                    }

                    if (constraintExpression is InvocationExpressionSyntax)
                    {
                        // seems we found a method call, so we should report that as it is likely that this belongs into the 'actual' argument
                        return Issue(expression);
                    }

                    if (constraintExpression is MemberAccessExpressionSyntax maes && invocation.Parent is ExpressionStatementSyntax statement)
                    {
                        // we have to dig deeper into other asserts, as 'actual' and 'expected' might be swapped
                        var siblings = statement.Siblings<ExpressionStatementSyntax>();
                        siblings.Remove(statement); // do not inspect the same statement

                        foreach (var sibling in siblings)
                        {
                            if (sibling.Expression is InvocationExpressionSyntax i && i.Is("Assert", "That") && i.ArgumentList.Arguments.FirstOrDefault()?.Expression is MemberAccessExpressionSyntax otherAssert)
                            {
                                // let's inspect if we have a similar assertion
                                var otherIdentifierName = otherAssert.GetIdentifierName();
                                var identifierName = expression.GetIdentifierName();

                                if (otherIdentifierName != identifierName)
                                {
                                    // seems we have a discrepancy here, so dig deeper
                                    var comparedIdentifierName = maes.GetIdentifierName();

                                    if (otherIdentifierName == comparedIdentifierName)
                                    {
                                        // seems we found a swapped value
                                        return Issue(expression);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}