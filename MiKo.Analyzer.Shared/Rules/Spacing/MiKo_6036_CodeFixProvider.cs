using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6036_CodeFixProvider)), Shared]
    public sealed class MiKo_6036_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6036_OpenBracesAreOnSamePositionLikeArrowOfLambdaAnalyzer.Id;

        protected override string Title => Resources.MiKo_6036_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<LambdaExpressionSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is LambdaExpressionSyntax lambda)
            {
                var position = MiKo_6036_OpenBracesAreOnSamePositionLikeArrowOfLambdaAnalyzer.GetStartPosition(lambda);

                var spaces = position.Character;
                var expressionSpaces = spaces + Constants.Indentation;

                var blockSyntax = lambda.Block;
                var block = blockSyntax?.WithOpenBraceToken(blockSyntax.OpenBraceToken.WithLeadingSpaces(spaces))
                                        .WithStatements(SyntaxFactory.List(blockSyntax.Statements.Select(_ => _.WithLeadingSpaces(expressionSpaces))))
                                        .WithCloseBraceToken(blockSyntax.CloseBraceToken.WithLeadingSpaces(spaces));

                return lambda.WithBlock(block);
            }

            return syntax;
        }
    }
}