using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class UsePatternMatchingCodeFixProvider : MaintainabilityCodeFixProvider
    {
        private readonly SyntaxKind m_syntaxKind;

        protected UsePatternMatchingCodeFixProvider(SyntaxKind syntaxKind = SyntaxKind.EqualsExpression) => m_syntaxKind = syntaxKind;

        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.First(_ => _.IsKind(m_syntaxKind));

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var binary = (BinaryExpressionSyntax)syntax;

            var operand = GetOperand(binary);
            var literal = GetLiteral(binary).WithoutTrailingTrivia(); // avoid unnecessary spaces at the end

            return GetUpdatedPatternSyntax(operand, literal);
        }

        protected virtual IsPatternExpressionSyntax GetUpdatedPatternSyntax(ExpressionSyntax operand, LiteralExpressionSyntax literal) => IsPattern(operand, literal);

        private static ExpressionSyntax GetOperand(BinaryExpressionSyntax binary) => binary.Right is LiteralExpressionSyntax
                                                                                     ? binary.Left
                                                                                     : binary.Right;

        private static LiteralExpressionSyntax GetLiteral(BinaryExpressionSyntax binary) => binary.Right as LiteralExpressionSyntax ?? (LiteralExpressionSyntax)binary.Left;
    }
}