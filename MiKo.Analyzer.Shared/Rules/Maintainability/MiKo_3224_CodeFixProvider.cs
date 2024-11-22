using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3224_CodeFixProvider)), Shared]
    public sealed class MiKo_3224_CodeFixProvider : LogicalConditionsSimplifierCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3224";

        protected override SyntaxKind PredefinedTypeKind => SyntaxKind.None;

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ExpressionSyntax expression && expression.WithoutParenthesis() is BinaryExpressionSyntax binary)
            {
                if (binary.Left.WithoutParenthesis() is BinaryExpressionSyntax left && left.IsKind(SyntaxKind.EqualsExpression))
                {
                    return left.WithoutTrivia()
                               .WithTriviaFrom(syntax);
                }
            }

            return syntax;
        }
    }
}