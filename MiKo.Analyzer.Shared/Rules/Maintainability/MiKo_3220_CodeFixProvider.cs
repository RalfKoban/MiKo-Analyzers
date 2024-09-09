using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3220_CodeFixProvider)), Shared]
    public sealed class MiKo_3220_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3220";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<BinaryExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is BinaryExpressionSyntax binary)
            {
                var left = binary.Left;
                var right = binary.Right;

                if (binary.IsKind(SyntaxKind.LogicalAndExpression))
                {
                    if (left.IsKind(SyntaxKind.FalseLiteralExpression) || right.IsKind(SyntaxKind.FalseLiteralExpression))
                    {
                        return FalseLiteral();
                    }

                    if (left.IsKind(SyntaxKind.TrueLiteralExpression))
                    {
                        return right.WithoutTrivia();
                    }

                    if (right.IsKind(SyntaxKind.TrueLiteralExpression))
                    {
                        return left.WithoutTrivia();
                    }
                }
                else if (binary.IsKind(SyntaxKind.LogicalOrExpression))
                {
                    if (left.IsKind(SyntaxKind.TrueLiteralExpression) || left.IsKind(SyntaxKind.FalseLiteralExpression))
                    {
                        return right.WithoutTrivia();
                    }

                    if (right.IsKind(SyntaxKind.TrueLiteralExpression) || right.IsKind(SyntaxKind.FalseLiteralExpression))
                    {
                        return left.WithoutTrivia();
                    }
                }
            }

            return syntax;
        }
    }
}