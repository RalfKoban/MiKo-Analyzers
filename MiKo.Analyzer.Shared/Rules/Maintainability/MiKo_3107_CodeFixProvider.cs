using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3107_CodeFixProvider)), Shared]
    public sealed class MiKo_3107_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3107";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            return syntaxNodes.OfType<InvocationExpressionSyntax>().First();
        }

        protected override async Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var invocation = (InvocationExpressionSyntax)syntax;

            var replacement = await ReplacementForAsync(invocation, document, cancellationToken).ConfigureAwait(false);

            return replacement.WithTriviaFrom(invocation);
        }

        private static SyntaxNode ReplacementFor(PredefinedTypeSyntax type)
        {
            var kind = type.Keyword.Kind();

            switch (kind)
            {
                case SyntaxKind.BoolKeyword: return FalseLiteral();
                case SyntaxKind.CharKeyword: return Literal(char.MinValue);
                case SyntaxKind.DecimalKeyword: return Literal(decimal.Zero);
                case SyntaxKind.DoubleKeyword: return Member(PredefinedType(kind), nameof(double.NaN));
                case SyntaxKind.FloatKeyword: return Member(PredefinedType(kind), nameof(float.NaN));
                case SyntaxKind.ObjectKeyword: return NullLiteral();
                case SyntaxKind.StringKeyword: return NullLiteral();
                default: return Literal(0);
            }
        }

        private static async Task<SyntaxNode> ReplacementForAsync(InvocationExpressionSyntax invocation, Document document, CancellationToken cancellationToken)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax maes && maes.Name is GenericNameSyntax generic)
            {
                return await ReplacementForAsync(generic.TypeArgumentList.Arguments.First(), document, cancellationToken).ConfigureAwait(false);
            }

            return NullLiteral();
        }

        private static async Task<SyntaxNode> ReplacementForAsync(TypeSyntax typeSyntax, Document document, CancellationToken cancellationToken)
        {
            if (typeSyntax is PredefinedTypeSyntax predefined)
            {
                return ReplacementFor(predefined);
            }

            // it might be a struct or an an enum, so let's check that
            var type = await typeSyntax.GetTypeSymbolAsync(document, cancellationToken).ConfigureAwait(false);

            if (type.IsValueType)
            {
                if (type.IsEnum())
                {
                    // take the first value
                    return Member(typeSyntax, type.GetFields()[0].Name);
                }

                // we have a struct
                return SyntaxFactory.DefaultExpression(typeSyntax);
            }

            return NullLiteral();
        }
    }
}