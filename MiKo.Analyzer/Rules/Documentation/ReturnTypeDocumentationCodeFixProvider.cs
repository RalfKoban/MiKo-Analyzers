using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
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

        protected static bool IsSeeCrefTask(SyntaxNode value)
        {
            if (IsSeeCref(value, SyntaxFactory.ParseTypeName("Task")))
            {
                return true;
            }

            if (IsSeeCref(value, SyntaxFactory.ParseTypeName("Task<TResult>")))
            {
                return true;
            }

            return false;
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
                        var syntax = syntaxNode.GetXmlSyntax(Constants.XmlTag.Returns).FirstOrDefault()
                                  ?? syntaxNode.GetXmlSyntax(Constants.XmlTag.Value).FirstOrDefault();

                        return syntax;
                    }
                }
            }

            return null;
        }

        protected sealed override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;
            foreach (var ancestor in comment.Ancestors())
            {
                switch (ancestor)
                {
                    case MethodDeclarationSyntax m:
                        return Comment(context, comment, m);

                    case PropertyDeclarationSyntax p:
                        return Comment(context, comment, p);

                    default:
                        continue;
                }
            }

            return comment;
        }

        protected abstract XmlElementSyntax NonGenericComment(CodeFixContext context, XmlElementSyntax comment, TypeSyntax returnType);

        protected abstract XmlElementSyntax GenericComment(CodeFixContext context, XmlElementSyntax comment, GenericNameSyntax returnType);

        protected virtual SyntaxNode Comment(CodeFixContext context, XmlElementSyntax comment, MethodDeclarationSyntax method) => Comment(context, comment, method.ReturnType);

        protected virtual SyntaxNode Comment(CodeFixContext context, XmlElementSyntax comment, PropertyDeclarationSyntax propertySyntax) => Comment(context, comment, propertySyntax.Type);

        private SyntaxNode Comment(CodeFixContext context, XmlElementSyntax comment, TypeSyntax returnType) => returnType is GenericNameSyntax genericReturnType
                                                                                                                   ? GenericComment(context, comment, genericReturnType)
                                                                                                                   : NonGenericComment(context, comment, returnType);
    }
}