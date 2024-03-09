﻿using System;
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
        protected static readonly string[] AlmostCorrectTaskReturnTypeStartingPhrases = CreateAlmostCorrectTaskReturnTypeStartingPhrases().OrderByDescending(_ => _.Length)
                                                                                                                                          .ThenBy(_ => _)
                                                                                                                                          .ToArray();

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

        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
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

        protected virtual SyntaxNode Comment(Document document, XmlElementSyntax comment, PropertyDeclarationSyntax property) => Comment(document, comment, property.GetName(), property.Type);

//// ncrunch: rdi off
        private static IEnumerable<string> CreateAlmostCorrectTaskReturnTypeStartingPhrases()
        {
            const string Result = "result";

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
                        foreach (var verb in finalVerbs)
                        {
                            yield return $"{start} {continuation} {operation}. The {Result} {verb} ";
                            yield return $"{start} {continuation} {operation}. The {Result.ToUpperCaseAt(0)} {verb} ";
                            yield return $"{start.ToUpperCaseAt(0)} {continuation} {operation}. The {Result} {verb} ";
                            yield return $"{start.ToUpperCaseAt(0)} {continuation} {operation}. The {Result.ToUpperCaseAt(0)} {verb} ";

                            yield return $"Returns {start} {continuation} {operation}. The {Result} {verb} ";
                            yield return $"Returns {start} {continuation} {operation}. The {Result.ToUpperCaseAt(0)} {verb} ";
                        }
                    }
                }
            }
        }
//// ncrunch: rdi default

        private SyntaxNode Comment(Document document, XmlElementSyntax comment, string memberName, TypeSyntax returnType) => returnType is GenericNameSyntax genericReturnType
                                                                                                                             ? GenericComment(document, comment, memberName, genericReturnType)
                                                                                                                             : NonGenericComment(document, comment, memberName, returnType);
    }
}