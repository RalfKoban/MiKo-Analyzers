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

            return GetReplacementFor(invocation).WitTriviaFrom(invocation);
        }

        private static SyntaxNode GetReplacementFor(InvocationExpressionSyntax invocation)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax maes && maes.Name is GenericNameSyntax generic)
            {
                var typeSyntax = generic.TypeArgumentList.Arguments.First();
                if (typeSyntax is PredefinedTypeSyntax p)
                {
                    var kind = p.Keyword.Kind();
                    switch (kind)
                    {
                        case SyntaxKind.BoolKeyword: return Literal(SyntaxKind.FalseLiteralExpression);
                        case SyntaxKind.CharKeyword: return Literal(SyntaxFactory.Literal(Char.MinValue));
                        case SyntaxKind.DecimalKeyword: return Literal(SyntaxFactory.Literal(Decimal.Zero));
                        case SyntaxKind.DoubleKeyword: return SimpleMemberAccess(PredefinedType(kind), nameof(Double.NaN));
                        case SyntaxKind.FloatKeyword: return SimpleMemberAccess(PredefinedType(kind), nameof(Single.NaN));
                        case SyntaxKind.ObjectKeyword: return Literal(SyntaxKind.NullLiteralExpression);
                        case SyntaxKind.StringKeyword: return Literal(SyntaxKind.NullLiteralExpression);
                        default: return Literal(SyntaxFactory.Literal(0));
                    }
                }
            }

            return Literal(SyntaxKind.NullLiteralExpression);
        }
    }
}