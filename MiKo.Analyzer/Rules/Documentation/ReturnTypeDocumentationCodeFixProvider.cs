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
        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes, Constants.XmlTag.Returns).FirstOrDefault() // method
                                                                                                    ?? GetXmlSyntax(syntaxNodes, Constants.XmlTag.Value).FirstOrDefault(); // property

        protected sealed override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var comment = (XmlElementSyntax)syntax;
            foreach (var ancestor in comment.Ancestors())
            {
                switch (ancestor)
                {
                    case MethodDeclarationSyntax m:
                        return Comment(comment, m.ReturnType);

                    case PropertyDeclarationSyntax p:
                        return Comment(comment, p.Type);

                    default:
                        continue;
                }
            }

            return comment;
        }

        protected abstract XmlElementSyntax NonGenericComment(XmlElementSyntax comment);

        protected abstract SyntaxNode GenericComment(XmlElementSyntax comment);

        protected XmlEmptyElementSyntax SeeCrefTaskResult()
        {
            var type = SyntaxFactory.ParseTypeName("Task<TResult>");
            var member = SyntaxFactory.ParseName(nameof(Task<object>.Result));
            return Cref(Constants.XmlTag.See, type, member);
        }

        private SyntaxNode Comment(XmlElementSyntax comment, TypeSyntax returnType) => returnType.IsKind(SyntaxKind.GenericName)
                                                                                           ? GenericComment(comment)
                                                                                           : NonGenericComment(comment);
    }
}