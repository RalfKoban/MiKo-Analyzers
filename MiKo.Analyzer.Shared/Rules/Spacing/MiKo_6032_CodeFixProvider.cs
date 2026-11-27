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
                return GetUpdatedParameter(parameter, leadingSpaces) as TSyntaxNode;
            }

            return base.GetUpdatedSyntax(syntax, leadingSpaces);
        }

        private static ParameterSyntax GetUpdatedParameter(ParameterSyntax parameter, in int leadingSpaces)
        {
            var updatedParameter = parameter.WithAttributeLists(GetUpdatedAttributes(parameter.AttributeLists, leadingSpaces))
                                            .WithModifiers(GetUpdatedModifiers(parameter.Modifiers, leadingSpaces))
                                            .WithType(GetUpdatedSyntaxNode(parameter.Type, leadingSpaces))
                                            .WithIdentifier(GetUpdatedSyntaxToken(parameter.Identifier, leadingSpaces))
                                            .WithDefault(GetUpdatedSyntaxNode(parameter.Default, leadingSpaces))
                                            .WithLeadingSpaces(leadingSpaces);

            return updatedParameter;
        }

        private static SyntaxList<AttributeListSyntax> GetUpdatedAttributes(in SyntaxList<AttributeListSyntax> attributeLists, in int leadingSpaces)
        {
            var attributesCount = attributeLists.Count;

            if (attributesCount is 0)
            {
                return attributeLists;
            }

            var updatedAttributeLists = attributeLists.ToArray();

            for (var i = 0; i < attributesCount; i++)
            {
                // move attributes that are placed on different lines
                updatedAttributeLists[i] = GetUpdatedSyntaxNode(attributeLists[i], leadingSpaces);
            }

            return updatedAttributeLists.ToSyntaxList();
        }

        private static SyntaxTokenList GetUpdatedModifiers(in SyntaxTokenList modifiers, in int leadingSpaces)
        {
            var modifiersCount = modifiers.Count;

            if (modifiersCount is 0)
            {
                return modifiers;
            }

            var updatedModifiers = modifiers.ToArray();

            for (var i = 0; i < modifiersCount; i++)
            {
                updatedModifiers[i] = GetUpdatedSyntaxToken(modifiers[i], leadingSpaces);
            }

            return updatedModifiers.ToTokenList();
        }

        private static SyntaxToken GetUpdatedSyntaxToken(in SyntaxToken token, in int leadingSpaces)
        {
            if (token.ContainsDirectives)
            {
                var updatedToken = token.HasLeadingDirective()
                                   ? token.WithLeadingSpaces(leadingSpaces)
                                   : token;

                if (updatedToken.HasTrailingEndOfLine())
                {
                    return updatedToken.WithEndOfLine();
                }

                return updatedToken.WithoutTrailingTrivia();
            }

            if (token.HasTrailingComment())
            {
                if (token.HasTrailingMultiLineComment())
                {
                    if (token.HasTrailingEndOfLine())
                    {
                        return token.WithoutTrailingEndOfLine()
                                    .WithLeadingSpaces(leadingSpaces);
                    }

                    return token.WithLeadingSpaces(1);
                }

                if (token.HasTrailingSingleLineComment())
                {
                    return token.WithLeadingSpaces(leadingSpaces);
                }
            }

            if (token.GetPreviousToken().HasTrailingSingleLineComment())
            {
                return token.WithLeadingSpaces(leadingSpaces);
            }

            return token.WithoutTrailingTrivia()
                        .WithLeadingSpaces(1);
        }

        private static TSyntaxNode GetUpdatedSyntaxNode<TSyntaxNode>(TSyntaxNode node, in int leadingSpaces) where TSyntaxNode : SyntaxNode
        {
            if (node is null)
            {
                return null;
            }

            if (node.ContainsDirectives)
            {
                var updatedNode = node.HasLeadingDirective()
                                  ? node.WithLeadingSpaces(leadingSpaces)
                                  : node;

                return updatedNode.WithEndOfLine();
            }

            if (node.HasTrailingComment())
            {
                if (node.HasTrailingMultiLineComment())
                {
                    if (node.HasTrailingEndOfLine())
                    {
                        return node.WithoutTrailingEndOfLine()
                                   .WithLeadingSpaces(leadingSpaces);
                    }

                    return node.WithLeadingSpaces(1);
                }

                if (node.HasTrailingSingleLineComment())
                {
                    return node.WithLeadingSpaces(leadingSpaces);
                }
            }

            if (node.PreviousSiblingNodeOrToken().HasTrailingSingleLineComment())
            {
                return node.WithoutTrailingTrivia()
                           .WithLeadingSpaces(leadingSpaces);
            }

            return node.WithoutTrailingTrivia()
                       .WithLeadingSpaces(1);
        }
    }
}