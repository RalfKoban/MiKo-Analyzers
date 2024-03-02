using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6030_CodeFixProvider)), Shared]
    public sealed class MiKo_6030_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6030";

        protected override string Title => Resources.MiKo_6030_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is InitializerExpressionSyntax initializer)
            {
                var position = GetProposedLinePosition(issue);

                var spaces = position.Character;

                return initializer.WithOpenBraceToken(initializer.OpenBraceToken.WithLeadingSpaces(spaces))
                                  .WithExpressions(GetUpdatedSyntax(initializer.Expressions, spaces + Constants.Indentation))
                                  .WithCloseBraceToken(initializer.CloseBraceToken.WithLeadingSpaces(spaces));
            }

            return syntax;
        }

        private static SeparatedSyntaxList<ExpressionSyntax> GetUpdatedSyntax(SeparatedSyntaxList<ExpressionSyntax> expressions, int leadingSpaces)
        {
            int? currentLine = null;

            var updatedExpressions = new List<ExpressionSyntax>();

            foreach (var expression in expressions)
            {
                var startingLine = expression.GetStartingLine();

                if (currentLine == startingLine)
                {
                    // it is on same line, so do not add any additional space
                    updatedExpressions.Add(expression);
                }
                else
                {
                    currentLine = startingLine;

                    // it seems to be on a different line, so add with spaces
                    updatedExpressions.Add(expression.WithLeadingSpaces(leadingSpaces));
                }
            }

            return SyntaxFactory.SeparatedList(updatedExpressions, expressions.GetSeparators());
        }
    }
}