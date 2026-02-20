using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ReturnTypeDocumentationCodeFixProvider : DocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        protected static readonly string[] AlmostCorrectTaskReturnTypeStartingPhrases = CreateAlmostCorrectTaskReturnTypeStartingPhrases().OrderDescendingByLengthAndText();

//// ncrunch: rdi default

        protected static XmlEmptyElementSyntax SeeCrefTaskResult() => SeeCref("Task<TResult>", nameof(Task<object>.Result));

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

        protected sealed override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var comment = (XmlElementSyntax)syntax;

            foreach (var ancestor in comment.Ancestors())
            {
                switch (ancestor)
                {
                    case MethodDeclarationSyntax m:
                        return CommentAsync(comment, m, document, cancellationToken);

                    case PropertyDeclarationSyntax p:
                        return CommentAsync(comment, p, document, cancellationToken);

                    default:
                        continue;
                }
            }

            return Task.FromResult(syntax);
        }

        protected abstract Task<SyntaxNode> NonGenericCommentAsync(XmlElementSyntax comment, string memberName, TypeSyntax returnType, Document document, CancellationToken cancellationToken);

        protected abstract Task<SyntaxNode> GenericCommentAsync(XmlElementSyntax comment, string memberName, GenericNameSyntax returnType, Document document, CancellationToken cancellationToken);

        protected virtual Task<SyntaxNode> CommentAsync(XmlElementSyntax comment, MethodDeclarationSyntax method, Document document, CancellationToken cancellationToken)
        {
            return CommentAsync(comment, method.GetName(), method.ReturnType, document, cancellationToken);
        }

#pragma warning disable CA1716
        protected virtual Task<SyntaxNode> CommentAsync(XmlElementSyntax comment, PropertyDeclarationSyntax property, Document document, CancellationToken cancellationToken)
        {
            return CommentAsync(comment, property.GetName(), property.Type, document, cancellationToken);
        }
#pragma warning restore CA1716

//// ncrunch: rdi off
        private static HashSet<string> CreateAlmostCorrectTaskReturnTypeStartingPhrases()
        {
            var starts = new[] { "a task", "an task" };
            var continuations = new[] { "that represents", "which represents", "representing" };
            var operations = new[] { "the operation", "the asynchronous operation" };
            var finalVerbs = new[] { "is", "indicates if", "indicates whether", "indicates", "contains" };
            var sentences = new[]
                                {
                                    //// dot start
                                    ". The result ",
                                    ". The Result ",
                                    ". The task result ",
                                    ". The task Result ",
                                    ". The Task result ",
                                    ". The Task Result ",
                                    ". The Task's result ",
                                    ". The Task's Result ",
                                    ". The task's result ",
                                    ". The task's Result ",
                                    //// comma start
                                    ", the result ",
                                    ", the Result ",
                                    ", the task result ",
                                    ", the task Result ",
                                    ", the Task result ",
                                    ", the Task Result ",
                                    ", the task's result ",
                                    ", the task's Result ",
                                    ", the Task's result ",
                                    ", the Task's Result ",
                                    //// semicolon start
                                    "; the result ",
                                    "; the Result ",
                                    "; the task result ",
                                    "; the task Result ",
                                    "; the Task result ",
                                    "; the Task Result ",
                                    "; the task's result ",
                                    "; the task's Result ",
                                    "; the Task's result ",
                                    "; the Task's Result ",
                                    //// where start
                                    " where the result ",
                                    " where the Result ",
                                    " where the task result ",
                                    " where the task Result ",
                                    " where the Task result ",
                                    " where the Task Result ",
                                    " where the task's result ",
                                    " where the task's Result ",
                                    " where the Task's result ",
                                    " where the Task's Result ",
                                };

            var results = new HashSet<string>();

            foreach (var start in starts)
            {
                foreach (var continuation in continuations)
                {
                    foreach (var operation in operations)
                    {
                        var middle = string.Concat(" ", continuation, " ", operation);

                        foreach (var sentence in sentences)
                        {
                            var beginning = string.Concat(start, middle, sentence);

                            foreach (var verb in finalVerbs)
                            {
                                var final = string.Concat(beginning, verb, " ");

                                results.Add(final);
                                results.Add(final.ToUpperCaseAt(0));
                                results.Add("Returns " + final);
                            }
                        }
                    }
                }
            }

            return results;
        }
//// ncrunch: rdi default

        private Task<SyntaxNode> CommentAsync(XmlElementSyntax comment, string memberName, TypeSyntax returnType, Document document, CancellationToken cancellationToken)
        {
            return returnType is GenericNameSyntax genericReturnType
                   ? GenericCommentAsync(comment, memberName, genericReturnType, document, cancellationToken)
                   : NonGenericCommentAsync(comment, memberName, returnType, document, cancellationToken);
        }
    }
}