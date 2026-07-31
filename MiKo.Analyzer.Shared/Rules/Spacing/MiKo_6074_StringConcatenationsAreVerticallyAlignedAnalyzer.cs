using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6074_StringConcatenationsAreVerticallyAlignedAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6074";

        public MiKo_6074_StringConcatenationsAreVerticallyAlignedAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.AddExpression);

        private static LinePosition FindOrientationPosition(BinaryExpressionSyntax node)
        {
            var clause = node.AncestorsWithinMethods<EqualsValueClauseSyntax>().FirstOrDefault();

            if (clause != null)
            {
                return clause.EqualsToken.GetStartPosition();
            }

            var attribute = node.AncestorsWithinMethods<AttributeArgumentSyntax>().FirstOrDefault();

            if (attribute?.NameEquals is NameEqualsSyntax nes)
            {
                return nes.EqualsToken.GetStartPosition();
            }

            var argument = node.AncestorsWithinMethods<ArgumentSyntax>().FirstOrDefault();

            if (argument != null)
            {
                var leftPosition = node.Left.GetStartPosition();
                var offset = node.OperatorToken.Text.Length + 1; // length of operator (such as '+') plus the following space

                return new LinePosition(leftPosition.Line, leftPosition.Character - offset);
            }

            return LinePosition.Zero;
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (BinaryExpressionSyntax)context.Node;

            var leftOperand = node.Left;
            var rightOperand = node.Right;

            if (rightOperand.IsOnSameLineAsEndOf(leftOperand))
            {
                // we do not need to report anything when placed on same line
                return;
            }

            if (node.IsStringConcatenation(context.SemanticModel))
            {
                var operatorToken = node.OperatorToken;

                if (operatorToken.IsOnSameLineAs(rightOperand))
                {
                    // operator is at begin of line
                    var position = FindOrientationPosition(node);

                    if (position != LinePosition.Zero && NotVerticallyAligned(operatorToken, position))
                    {
                        ReportDiagnostics(context, Issue(operatorToken, CreateProposalForSpaces(position.Character)));
                    }
                }
                else
                {
                    if (operatorToken.IsOnSameLineAsEndOf(leftOperand))
                    {
                        // operator is at end of line
                        var position = leftOperand.GetStartPosition();

                        if (NotVerticallyAligned(rightOperand.GetStartPosition(), position))
                        {
                            ReportDiagnostics(context, Issue(rightOperand, CreateProposalForSpaces(position.Character)));
                        }
                    }
                }
            }
        }
    }
}