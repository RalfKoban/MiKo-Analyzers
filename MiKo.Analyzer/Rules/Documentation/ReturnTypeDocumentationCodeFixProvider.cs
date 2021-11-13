using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ReturnTypeDocumentationCodeFixProvider : DocumentationCodeFixProvider
    {
        protected static XmlEmptyElementSyntax SeeCrefTaskResult()
        {
            var type = SyntaxFactory.ParseTypeName("Task<TResult>");
            var member = SyntaxFactory.ParseName(nameof(Task<object>.Result));

            return Cref(Constants.XmlTag.See, type, member);
        }

        protected static bool IsSeeCrefTaskResult(SyntaxNode value)
        {
            var type = SyntaxFactory.ParseTypeName("Task<TResult>");
            var member = SyntaxFactory.ParseName(nameof(Task<object>.Result));

            return IsSeeCref(value, type, member);
        }

        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            foreach (var syntaxNode in syntaxNodes)
            {
                switch (syntaxNode)
                {
                    case MethodDeclarationSyntax _:
                    case PropertyDeclarationSyntax _:
                    {
                        var syntax = GetXmlSyntax(Constants.XmlTag.Returns, syntaxNode).FirstOrDefault()
                                  ?? GetXmlSyntax(Constants.XmlTag.Value, syntaxNode).FirstOrDefault();

                        return syntax;
                    }
                }
            }

            return null;
        }

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var comment = (XmlElementSyntax)syntax;
            foreach (var ancestor in comment.Ancestors())
            {
                switch (ancestor)
                {
                    case MethodDeclarationSyntax m:
                        return Comment(document, comment, m.ReturnType);

                    case PropertyDeclarationSyntax p:
                        return Comment(document, comment, p.Type);

                    default:
                        continue;
                }
            }

            return comment;
        }

        protected abstract XmlElementSyntax NonGenericComment(Document document, XmlElementSyntax comment, TypeSyntax returnType);

        protected abstract XmlElementSyntax GenericComment(Document document, XmlElementSyntax comment, GenericNameSyntax returnType);

        private SyntaxNode Comment(Document document, XmlElementSyntax comment, TypeSyntax returnType) => returnType is GenericNameSyntax genericReturnType
                                                                                                              ? GenericComment(document, comment, genericReturnType)
                                                                                                              : NonGenericComment(document, comment, returnType);
    }
}