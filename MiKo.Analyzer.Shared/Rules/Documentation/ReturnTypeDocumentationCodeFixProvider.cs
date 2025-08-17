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
//// ncrunch: rdi off

        protected static readonly string[] AlmostCorrectTaskReturnTypeStartingPhrases = CreateAlmostCorrectTaskReturnTypeStartingPhrases().OrderByDescending(_ => _.Length)
                                                                                                                                          .ThenBy(_ => _)
                                                                                                                                          .ToArray();

//// ncrunch: rdi default

        protected static XmlEmptyElementSyntax SeeCrefTaskResult()
        {
            var type = "Task<TResult>".AsTypeSyntax();
            var member = SyntaxFactory.ParseName(nameof(Task<object>.Result));

            return Cref(Constants.XmlTag.See, type, member);
        }

        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var syntaxNode in syntaxNodes)
            {
                switch (syntaxNode)
                {
                    case MethodDeclarationSyntax _:
                    case PropertyDeclarationSyntax _:
                    {
                        var returnsSyntax = syntaxNode.GetXmlSyntax(Constants.XmlTag.Returns);

                        if (returnsSyntax.Count > 0)
                        {
                            return returnsSyntax[0];
                        }

                        var valuesSyntax = syntaxNode.GetXmlSyntax(Constants.XmlTag.Value);

                        if (valuesSyntax.Count > 0)
                        {
                            return valuesSyntax[0];
                        }

                        return null;
                    }
                }
            }

            return null;
        }

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;

            foreach (var ancestor in comment.Ancestors())
            {
                switch (ancestor)
                {
                    case MethodDeclarationSyntax m:
                        return Comment(document, comment, m);

                    case PropertyDeclarationSyntax p:
                        return Comment(document, comment, p);

                    default:
                        continue;
                }
            }

            return comment;
        }

        protected abstract XmlElementSyntax NonGenericComment(Document document, XmlElementSyntax comment, string memberName, TypeSyntax returnType);

        protected abstract XmlElementSyntax GenericComment(Document document, XmlElementSyntax comment, string memberName, GenericNameSyntax returnType);

        protected virtual SyntaxNode Comment(Document document, XmlElementSyntax comment, MethodDeclarationSyntax method) => Comment(document, comment, method.GetName(), method.ReturnType);

#pragma warning disable CA1716
        protected virtual SyntaxNode Comment(Document document, XmlElementSyntax comment, PropertyDeclarationSyntax property) => Comment(document, comment, property.GetName(), property.Type);
#pragma warning restore CA1716

//// ncrunch: rdi off
        private static IEnumerable<string> CreateAlmostCorrectTaskReturnTypeStartingPhrases()
        {
            var starts = new[] { "a task", "an task" };
            var continuations = new[] { "that represents", "which represents", "representing" };
            var operations = new[] { "the operation", "the asynchronous operation" };
            var finalVerbs = new[] { "is", "indicates if", "indicates whether", "indicates" };

            foreach (var start in starts)
            {
                foreach (var continuation in continuations)
                {
                    foreach (var operation in operations)
                    {
                        var middle = string.Concat(" ", continuation, " ", operation);

                        var middleLowerCase = middle + ". The result ";
                        var middleUpperCase = middle + ". The Result ";

                        foreach (var verb in finalVerbs)
                        {
                            var endingLowerCase = middleLowerCase + verb + " ";
                            var endingUpperCase = middleUpperCase + verb + " ";

                            yield return start + endingLowerCase;
                            yield return start + endingUpperCase;
                            yield return start.ToUpperCaseAt(0) + endingLowerCase;
                            yield return start.ToUpperCaseAt(0) + endingUpperCase;

                            yield return "Returns " + start + endingLowerCase;
                            yield return "Returns " + start + endingUpperCase;
                        }
                    }
                }
            }
        }
//// ncrunch: rdi default

        private XmlElementSyntax Comment(Document document, XmlElementSyntax comment, string memberName, TypeSyntax returnType) => returnType is GenericNameSyntax genericReturnType
                                                                                                                                   ? GenericComment(document, comment, memberName, genericReturnType)
                                                                                                                                   : NonGenericComment(document, comment, memberName, returnType);
    }
}