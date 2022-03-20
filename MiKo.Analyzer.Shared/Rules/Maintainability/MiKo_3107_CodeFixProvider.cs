using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3107_CodeFixProvider)), Shared]
    public class MiKo_3107_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public sealed override string FixableDiagnosticId => MiKo_3107_OnlyMocksUseConditionMatchersAnalyzer.Id;

        protected sealed override string Title => Resources.MiKo_3107_CodeFixTitle;

        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            return syntaxNodes.OfType<InvocationExpressionSyntax>().First();
        }

        protected sealed override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var invocation = (InvocationExpressionSyntax)syntax;

            return ReplacementFor(context, invocation).WitTriviaFrom(invocation);
        }

        private static SyntaxNode ReplacementFor(CodeFixContext context, InvocationExpressionSyntax invocation)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax maes && maes.Name is GenericNameSyntax generic)
            {
                return ReplacementFor(context, generic.TypeArgumentList.Arguments.First());
            }

            return Literal(SyntaxKind.NullLiteralExpression);
        }

        private static SyntaxNode ReplacementFor(PredefinedTypeSyntax type)
        {
            var kind = type.Keyword.Kind();
            switch (kind)
            {
                case SyntaxKind.BoolKeyword: return Literal(SyntaxKind.FalseLiteralExpression);
                case SyntaxKind.CharKeyword: return Literal(SyntaxFactory.Literal(char.MinValue));
                case SyntaxKind.DecimalKeyword: return Literal(SyntaxFactory.Literal(decimal.Zero));
                case SyntaxKind.DoubleKeyword: return SimpleMemberAccess(PredefinedType(kind), nameof(double.NaN));
                case SyntaxKind.FloatKeyword: return SimpleMemberAccess(PredefinedType(kind), nameof(float.NaN));
                case SyntaxKind.ObjectKeyword: return Literal(SyntaxKind.NullLiteralExpression);
                case SyntaxKind.StringKeyword: return Literal(SyntaxKind.NullLiteralExpression);
                default: return Literal(SyntaxFactory.Literal(0));
            }
        }

        private static SyntaxNode ReplacementFor(CodeFixContext context, TypeSyntax typeSyntax)
        {
            if (typeSyntax is PredefinedTypeSyntax predefined)
            {
                return ReplacementFor(predefined);
            }

            // it might be a struct or an an enum, so let's check that
            var semanticModel = GetSemanticModel(context);
            var type = typeSyntax.GetTypeSymbol(semanticModel);
            if (type.IsValueType)
            {
                if (type.IsEnum())
                {
                    // take the first value
                    return SimpleMemberAccess(typeSyntax, type.GetFields().First().Name);
                }

                // we have a struct
                return SyntaxFactory.DefaultExpression(typeSyntax);
            }

            return Literal(SyntaxKind.NullLiteralExpression);
        }
    }
}