using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6052_CodeFixProvider)), Shared]
    public sealed class MiKo_6052_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6052";

        protected override string Title => Resources.MiKo_6052_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<BaseTypeDeclarationSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is BaseTypeDeclarationSyntax declaration)
            {
                var baseList = declaration.BaseList;

                if (baseList != null)
                {
                    var types = baseList.Types;
                    var firstType = types[0];

                    var updatedToken = baseList.ColonToken.WithLeadingEndOfLine().WithAdditionalLeadingTriviaFrom(firstType).WithTrailingSpace();
                    var updatedTypes = types.Replace(firstType, firstType.WithoutLeadingTrivia());
                    var updatedBaseList = baseList.WithColonToken(updatedToken).WithTypes(updatedTypes);

                    var updatedDeclaration = declaration;

                    var sibling = GetPreviousSibling(declaration);

                    if (sibling.IsToken)
                    {
                        // identifier
                        var token = sibling.AsToken();
                        updatedDeclaration = declaration.ReplaceToken(token, token.WithoutTrailingTrivia());
                    }
                    else if (sibling.IsNode)
                    {
                        // type list
                        var node = sibling.AsNode();

                        updatedDeclaration = declaration.ReplaceNode(node, node.WithoutTrailingTrivia());
                    }

                    return updatedDeclaration.WithBaseList(updatedBaseList);
                }
            }

            return syntax;
        }

        private static SyntaxNodeOrToken GetPreviousSibling(BaseTypeDeclarationSyntax declaration)
        {
            var typeParameterList = GetTypeParameterList();

            return typeParameterList != null
                   ? typeParameterList
                   : declaration.Identifier;

            SyntaxNodeOrToken GetTypeParameterList()
            {
                switch (declaration)
                {
                    case ClassDeclarationSyntax syntax: return syntax.TypeParameterList;
                    case InterfaceDeclarationSyntax syntax: return syntax.TypeParameterList;
                    case StructDeclarationSyntax syntax: return syntax.TypeParameterList;
                    case RecordDeclarationSyntax syntax: return syntax.TypeParameterList;
                    default:
                        return null;
                }
            }
        }
    }
}