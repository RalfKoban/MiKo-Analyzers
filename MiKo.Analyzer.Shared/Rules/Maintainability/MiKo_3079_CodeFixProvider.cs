using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3079_CodeFixProvider)), Shared]
    public sealed class MiKo_3079_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3079";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault(_ => _.IsKind(SyntaxKind.UnaryMinusExpression));

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is PrefixUnaryExpressionSyntax unary)
            {
                if (unary.Operand is LiteralExpressionSyntax literal && literal.Token.Value is int i)
                {
                    var hresult = "0x" + ((-1) * i).ToString("X8");
                    var cast = SyntaxFactory.CastExpression(PredefinedType(SyntaxKind.IntKeyword), Literal(i, hresult));

                    return SyntaxFactory.CheckedExpression(SyntaxKind.UncheckedExpression, cast);
                }
            }

            return syntax;
        }
    }
}