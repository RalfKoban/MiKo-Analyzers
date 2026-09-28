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

        private Diagnostic AnalyzeInvocationExpression(in SyntaxNodeAnalysisContext context, InvocationExpressionSyntax assertThat, SemanticModel semanticModel)
        {
            var arguments = assertThat.ArgumentList.Arguments;

            if (arguments.Count is 0)
            {
                return null;
            }

            var actualPart = arguments[0].Expression;

            switch (actualPart)
            {
                case PrefixUnaryExpressionSyntax u when u.Operand is LiteralExpressionSyntax:
                case LiteralExpressionSyntax _:
                case MemberAccessExpressionSyntax part when part.IsEnumMember(semanticModel):
                    return Issue(actualPart);

                case MemberAccessExpressionSyntax part:
                    return AnalyzeAssertion(context, assertThat, arguments, part); // it looks like the values could be swapped based on other asserts, so we have to take a more costly look around

                default:
                    return null;
            }
        }

        private Diagnostic AnalyzeAssertion(in SyntaxNodeAnalysisContext context, InvocationExpressionSyntax assertThat, in SeparatedSyntaxList<ArgumentSyntax> assertArguments, MemberAccessExpressionSyntax actualPart)
        {
            if (assertArguments.Count > 1 && assertArguments[1].Expression is InvocationExpressionSyntax constraint)
            {
                var identifierName = actualPart.GetStartingIdentifierName();

                if (Constants.Names.ObjectUnderTestNames.Contains(identifierName))
                {
                    // seems everything is OK as we invoke something on our testee
                    return null;
                }

                // TODO RKN: What about 'Is.Not.EqualTo' and similar checks?
                if (constraint.Is("Is", "EqualTo"))
                {
                    var constraintExpression = constraint.ArgumentList.Arguments.FirstOrDefault()?.Expression;

                    if (constraintExpression is LiteralExpressionSyntax || constraintExpression.IsConst(context))
                    {
                        // seems everything is OK
                        return null;
                    }

                    var otherIdentifierName = constraintExpression.GetStartingIdentifierName();

                    if (Constants.Names.ObjectUnderTestNames.Contains(otherIdentifierName))
                    {
                        // seems someone switched the testee
                        return Issue(actualPart);
                    }

                    if (constraintExpression is InvocationExpressionSyntax constraintInvocation)
                    {
                        if (identifierName == otherIdentifierName)
                        {
                            // seems everything is OK (code seems strange, but this analyzer is not responsible for reporting that)
                            return null;
                        }

                        if (constraintInvocation.Expression is MemberAccessExpressionSyntax constraintMember)
                        {
                            switch (constraintMember.GetName())
                            {
                                case nameof(ToString) when constraintMember.Expression is LiteralExpressionSyntax || constraintMember.Expression.IsConst(context):
                                    return null; // seems everything is OK (code seems strange, but this analyzer is not responsible for reporting that)

                                case nameof(string.Format) when otherIdentifierName is "string" || otherIdentifierName is "String":
                                    return null; // seems we have a 'String.Format' call which we currently accept
                            }
                        }

                        // seems we found a method call, so we should report that as it is likely that this belongs into the 'actual' argument
                        return Issue(actualPart);
                    }

                    if (constraintExpression is MemberAccessExpressionSyntax expectedPart && assertThat.Parent is ExpressionStatementSyntax statement)
                    {
                        // we have to dig deeper into other asserts, as 'actual' and 'expected' might be swapped
                        var siblings = statement.Siblings<ExpressionStatementSyntax>();
                        siblings.Remove(statement); // do not inspect the same statement

                        foreach (var sibling in siblings)
                        {
                            if (sibling.Expression is InvocationExpressionSyntax i && i.Is("Assert", "That") && i.ArgumentList.Arguments.FirstOrDefault()?.Expression is MemberAccessExpressionSyntax otherActualPart)
                            {
                                // let's inspect if we have a similar assertion
                                var otherAssertIdentifierName = otherActualPart.GetStartingIdentifierName();

                                if (otherAssertIdentifierName != identifierName)
                                {
                                    // seems we have a discrepancy here, so dig deeper
                                    var comparedIdentifierName = expectedPart.GetStartingIdentifierName();

                                    if (otherAssertIdentifierName == comparedIdentifierName)
                                    {
                                        // seems we found a swapped value
                                        return Issue(actualPart);
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