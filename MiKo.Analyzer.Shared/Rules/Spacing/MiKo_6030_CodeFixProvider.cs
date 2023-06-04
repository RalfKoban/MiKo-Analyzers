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
        public override string FixableDiagnosticId => MiKo_6030_InitializerBracesAreOnSamePositionLikeTypeAnalyzer.Id;

        protected override string Title => Resources.MiKo_6030_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is InitializerExpressionSyntax initializer)
            {
                var position = MiKo_6030_InitializerBracesAreOnSamePositionLikeTypeAnalyzer.GetStartPosition(initializer);

                var spaces = position.Character;
                var expressionSpaces = spaces + Constants.Indentation;

                return initializer.WithOpenBraceToken(initializer.OpenBraceToken.WithLeadingSpaces(spaces))
                                  .WithExpressions(SyntaxFactory.SeparatedList(
                                                                           initializer.Expressions.Select(_ => _.WithLeadingSpaces(expressionSpaces)),
                                                                           initializer.Expressions.GetSeparators()))
                                  .WithCloseBraceToken(initializer.CloseBraceToken.WithLeadingSpaces(spaces));
            }

            return syntax;
        }
    }
}