using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public abstract class SpacingCodeFixProvider : MiKoCodeFixProvider
    {
        protected static StatementSyntax GetUpdatedStatement(StatementSyntax statement, int spaces)
        {
            switch (statement)
            {
                case BlockSyntax block: return GetUpdatedBlock(block, spaces);
                case IfStatementSyntax @if: return GetUpdatedIf(@if, spaces);
                case SwitchStatementSyntax @switch: return GetUpdatedSwitch(@switch, spaces);
                case DoStatementSyntax @do: return GetUpdatedDo(@do, spaces);
                case WhileStatementSyntax @while: return GetUpdatedWhile(@while, spaces);
                case ForStatementSyntax @for: return GetUpdatedFor(@for, spaces);
                case ForEachStatementSyntax @foreach: return GetUpdatedForEach(@foreach, spaces);
                default: return statement.WithLeadingSpaces(spaces);
            }
        }

        protected static DoStatementSyntax GetUpdatedDo(DoStatementSyntax @do, int spaces)
        {
            return @do.WithLeadingSpaces(spaces)
                      .WithStatement(GetUpdatedNestedStatement(@do.Statement, spaces))
                      .WithWhileKeyword(@do.WhileKeyword.WithLeadingSpaces(spaces));
        }

        protected static WhileStatementSyntax GetUpdatedWhile(WhileStatementSyntax @while, int spaces)
        {
            return @while.WithLeadingSpaces(spaces)
                         .WithStatement(GetUpdatedNestedStatement(@while.Statement, spaces));
        }

        protected static ForStatementSyntax GetUpdatedFor(ForStatementSyntax @for, int spaces)
        {
            return @for.WithLeadingSpaces(spaces)
                       .WithStatement(GetUpdatedNestedStatement(@for.Statement, spaces));
        }

        protected static ForEachStatementSyntax GetUpdatedForEach(ForEachStatementSyntax @foreach, int spaces)
        {
            return @foreach.WithLeadingSpaces(spaces)
                           .WithStatement(GetUpdatedNestedStatement(@foreach.Statement, spaces));
        }

        protected static BlockSyntax GetUpdatedBlock(BlockSyntax block, int spaces)
        {
            if (block is null)
            {
                return null;
            }

            var indentation = spaces + Constants.Indentation;

            return block.WithOpenBraceToken(block.OpenBraceToken.WithLeadingSpaces(spaces))
                        .WithStatements(SyntaxFactory.List(block.Statements.Select(_ => GetUpdatedStatement(_, indentation))))
                        .WithCloseBraceToken(block.CloseBraceToken.WithLeadingSpaces(spaces));
        }

        protected static IfStatementSyntax GetUpdatedIf(IfStatementSyntax @if, int spaces)
        {
            var updatedStatement = GetUpdatedNestedStatement(@if.Statement, spaces);

            return @if.WithLeadingSpaces(spaces)
                      .WithStatement(updatedStatement)
                      .WithElse(GetUpdatedElse(@if.Else, spaces));
        }

        protected static ElseClauseSyntax GetUpdatedElse(ElseClauseSyntax @else, int spaces)
        {
            if (@else is null)
            {
                return null;
            }

            var updatedStatement = GetUpdatedNestedStatement(@else.Statement, spaces);

            return @else.WithLeadingSpaces(spaces)
                        .WithStatement(updatedStatement);
        }

        protected static SwitchStatementSyntax GetUpdatedSwitch(SwitchStatementSyntax @switch, int spaces)
        {
            return @switch.WithLeadingSpaces(spaces)
                          .WithOpenBraceToken(@switch.OpenBraceToken.WithLeadingSpaces(spaces))
                          .WithSections(SyntaxFactory.List(@switch.Sections.Select(_ => GetUpdatedSection(_, spaces))))
                          .WithCloseBraceToken(@switch.CloseBraceToken.WithLeadingSpaces(spaces));
        }

        private static SwitchSectionSyntax GetUpdatedSection(SwitchSectionSyntax section, int spaces)
        {
            var indentation = spaces + Constants.Indentation;

            return section.WithLabels(SyntaxFactory.List(section.Labels.Select(_ => _.WithLeadingSpaces(indentation))))
                          .WithStatements(SyntaxFactory.List(section.Statements.Select(_ => GetUpdatedNestedStatement(_, indentation))));
        }

        private static StatementSyntax GetUpdatedNestedStatement(StatementSyntax statement, int spaces)
        {
            var indentation = spaces + Constants.Indentation;

            return statement is BlockSyntax block
                   ? GetUpdatedBlock(block, spaces)
                   : GetUpdatedStatement(statement, indentation);
        }
    }
}