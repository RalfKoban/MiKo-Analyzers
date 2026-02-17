using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6061_CodeFixProvider)), Shared]
    public sealed class MiKo_6061_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6061";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<SwitchExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return updatedSyntax;
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is SwitchExpressionSyntax switchExpression)
            {
                var arms = switchExpression.Arms;

                var updatedArms = arms.GetWithSeparators()
                                      .Select(item =>
                                              {
                                                  if (item.IsNode)
                                                  {
                                                      var node = item.AsNode();

                                                      return node.PlacedOnSameLine().WithLeadingTriviaFrom(node).WithEndOfLine();
                                                  }

                                                  if (item.IsToken)
                                                  {
                                                      return item.AsToken().WithoutLeadingTrivia();
                                                  }

                                                  return item;
                                              });

                var spaces = switchExpression.SwitchKeyword.GetPositionWithinEndLine();

                return switchExpression.WithArms(SyntaxFactory.SeparatedList<SwitchExpressionArmSyntax>(updatedArms))
                                       .WithOpenBraceToken(switchExpression.OpenBraceToken.WithoutTrivia().WithLeadingSpaces(spaces).WithTrailingNewLine())
                                       .WithCloseBraceToken(switchExpression.CloseBraceToken.WithLeadingSpaces(spaces));
            }

            return syntax;
        }
    }
}