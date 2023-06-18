using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6033_CodeFixProvider)), Shared]
    public sealed class MiKo_6033_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6033_CaseBlockBracesAreOnSamePositionLikeCaseKeywordAnalyzer.Id;

        protected override string Title => Resources.MiKo_6033_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<BlockSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is BlockSyntax block && block.Parent is SwitchSectionSyntax section)
            {
                var position = MiKo_6033_CaseBlockBracesAreOnSamePositionLikeCaseKeywordAnalyzer.GetStartPosition(section);
                var spaces = position.Character;
                var nested = spaces + Constants.Indentation;

                return block.WithOpenBraceToken(block.OpenBraceToken.WithLeadingSpaces(spaces))
                            .WithStatements(SyntaxFactory.List(block.Statements.Select(_ => _.WithLeadingSpaces(nested))))
                            .WithCloseBraceToken(block.CloseBraceToken.WithLeadingSpaces(spaces));
            }

            return syntax;
        }
    }
}