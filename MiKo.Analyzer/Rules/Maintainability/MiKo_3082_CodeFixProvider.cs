using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3082_CodeFixProvider)), Shared]
    public sealed class MiKo_3082_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3082_UsePatternMatchingForBooleanEqualsExpressionAnalyzer.Id;

        protected override string Title => "Apply 'is' pattern";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.First(_ => _.IsKind(SyntaxKind.EqualsExpression));

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var binary = (BinaryExpressionSyntax)syntax;

            var operand = GetOperand(binary);
            var literal = GetLiteral(binary);

            var pattern = SyntaxFactory.IsPatternExpression(operand, SyntaxFactory.ConstantPattern(literal));
            return pattern;
        }

        private static ExpressionSyntax GetOperand(BinaryExpressionSyntax binary) => binary.Right is LiteralExpressionSyntax
                                                                                         ? binary.Left
                                                                                         : binary.Right;

        private static LiteralExpressionSyntax GetLiteral(BinaryExpressionSyntax binary) => binary.Right is LiteralExpressionSyntax literal
                                                                                                ? literal
                                                                                                : (LiteralExpressionSyntax)binary.Left;
    }
}