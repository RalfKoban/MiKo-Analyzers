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
            if (syntax is SwitchExpressionSyntax switchExpression)
            {
                var arms = switchExpression.Arms;

                var updatedArms = arms.GetWithSeparators()
                                      .Select(token =>
                                                      {
                                                          if (token.IsNode)
                                                          {
                                                              var node = token.AsNode();

                                                              return PlacedOnSameLine(node).WithLeadingTriviaFrom(node);
                                                          }

                                                          if (token.IsToken)
                                                          {
                                                              return token.AsToken().WithoutLeadingTrivia();
                                                          }

                                                          return token;
                                                      });

                return switchExpression.WithArms(SyntaxFactory.SeparatedList<SwitchExpressionArmSyntax>(updatedArms));
            }

            return syntax;
        }
    }
}