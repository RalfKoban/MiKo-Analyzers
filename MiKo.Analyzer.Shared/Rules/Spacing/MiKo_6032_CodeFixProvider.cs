using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6032_CodeFixProvider)), Shared]
    public sealed class MiKo_6032_CodeFixProvider : IndentedSpacingCodeFixProvider<ParameterSyntax>
    {
        public override string FixableDiagnosticId => "MiKo_6032";

        protected override TSyntaxNode GetUpdatedSyntax<TSyntaxNode>(TSyntaxNode syntax, in int leadingSpaces)
        {
            if (syntax is ParameterSyntax parameter)
            {
                var updatedParameter = parameter;

                // move attributes that are placed on different lines
                var attributeLists = updatedParameter.AttributeLists;
                var attributesCount = attributeLists.Count;

                if (attributesCount > 0)
                {
                    var updatedAttributeLists = attributeLists.ToArray();

                    for (var i = 0; i < attributesCount; i++)
                    {
                        updatedAttributeLists[i] = GetUpdatedSyntaxNode(attributeLists[i], leadingSpaces);
                    }

                    updatedParameter = updatedParameter.WithAttributeLists(updatedAttributeLists.ToSyntaxList());
                }

                var modifiers = updatedParameter.Modifiers;
                var modifiersCount = modifiers.Count;

                if (modifiersCount > 0)
                {
                    var updatedModifiers = modifiers.ToArray();

                    for (var i = 0; i < modifiersCount; i++)
                    {
                        updatedModifiers[i] = GetUpdatedSyntaxToken(modifiers[i], leadingSpaces);
                    }

                    updatedParameter = updatedParameter.WithModifiers(updatedModifiers.ToTokenList());
                }

                updatedParameter = updatedParameter.WithType(GetUpdatedSyntaxNode(updatedParameter.Type, leadingSpaces))
                                                   .WithIdentifier(GetUpdatedSyntaxToken(updatedParameter.Identifier, leadingSpaces))
                                                   .WithDefault(GetUpdatedSyntaxNode(updatedParameter.Default, leadingSpaces))
                                                   .WithLeadingSpaces(leadingSpaces);

                return updatedParameter as TSyntaxNode;
            }

            return base.GetUpdatedSyntax(syntax, leadingSpaces);
        }

        private static SyntaxToken GetUpdatedSyntaxToken(in SyntaxToken token, in int leadingSpaces)
        {
            if (token.ContainsDirectives)
            {
                var updatedToken = token;

                if (updatedToken.LeadingTrivia.HasDirective())
                {
                    updatedToken = updatedToken.WithLeadingSpaces(leadingSpaces);
                }

                if (updatedToken.TrailingTrivia.HasEndOfLine())
                {
                    return updatedToken.WithEndOfLine();
                }

                return updatedToken.WithoutTrailingTrivia();
            }

            var trailingTrivia = token.TrailingTrivia;

            if (trailingTrivia.HasComment())
            {
                if (trailingTrivia.Any(_ => _.IsMultiLineComment()))
                {
                    if (trailingTrivia.HasEndOfLine())
                    {
                        return token.WithoutTrailingEndOfLine().WithLeadingSpaces(leadingSpaces);
                    }

                    return token.WithLeadingSpaces(1);
                }

                if (trailingTrivia.Any(_ => _.IsSingleLineComment()))
                {
                    return token.WithLeadingSpaces(leadingSpaces);
                }
            }

            var previousToken = token.GetPreviousToken();

            if (previousToken.TrailingTrivia.Any(_ => _.IsSingleLineComment()))
            {
                return token.WithLeadingSpaces(leadingSpaces);
            }

            return token.WithoutTrailingTrivia().WithLeadingSpaces(1);
        }

        private static TSyntaxNode GetUpdatedSyntaxNode<TSyntaxNode>(TSyntaxNode node, in int leadingSpaces) where TSyntaxNode : SyntaxNode
        {
            if (node is null)
            {
                return null;
            }

            if (node.ContainsDirectives)
            {
                var updatedNode = node;

                if (updatedNode.GetLeadingTrivia().HasDirective())
                {
                    updatedNode = updatedNode.WithLeadingSpaces(leadingSpaces);
                }

                return updatedNode.WithEndOfLine();
            }

            var trailingTrivia = node.GetTrailingTrivia();

            if (trailingTrivia.HasComment())
            {
                if (trailingTrivia.Any(_ => _.IsMultiLineComment()))
                {
                    if (trailingTrivia.HasEndOfLine())
                    {
                        return node.WithoutTrailingEndOfLine().WithLeadingSpaces(leadingSpaces);
                    }

                    return node.WithLeadingSpaces(1);
                }

                if (trailingTrivia.Any(_ => _.IsSingleLineComment()))
                {
                    return node.WithLeadingSpaces(leadingSpaces);
                }
            }

            if (node.PreviousSiblingNodeOrToken() is SyntaxNodeOrToken sibling && sibling.GetTrailingTrivia().Any(_ => _.IsSingleLineComment()))
            {
                return node.WithoutTrailingTrivia().WithLeadingSpaces(leadingSpaces);
            }

            return node.WithoutTrailingTrivia().WithLeadingSpaces(1);
        }
    }
}