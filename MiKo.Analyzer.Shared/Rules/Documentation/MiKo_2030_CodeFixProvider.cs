using System;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2030_CodeFixProvider)), Shared]
    public sealed class MiKo_2030_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        private static readonly ReplacementMap ReplacementMap = new ReplacementMap(
                                                                               "MiKo_2030",
                                                                               new[]
                                                                                   {
                                                                                       new Pair("The a ", "A "),
                                                                                       new Pair("The an ", "An "),
                                                                                       new Pair("The the ", "The "),
                                                                                       new Pair("The this ", "The "),
                                                                                       new Pair("The always a ", "A "),
                                                                                       new Pair("The always an ", "An "),
                                                                                       new Pair("The always the ", "The "),
                                                                                       new Pair("The always this ", "The "),
                                                                                       new Pair("The always ", "The "),
                                                                                       new Pair("The return a ", "A "),
                                                                                       new Pair("The returns a ", "A "),
                                                                                       new Pair("The return an ", "An "),
                                                                                       new Pair("The returns an ", "An "),
                                                                                       new Pair("The return the ", "The "),
                                                                                       new Pair("The returns the ", "The "),
                                                                                       new Pair("The return this ", "The "),
                                                                                       new Pair("The returns this ", "The "),
                                                                                       new Pair("The return ", "The "),
                                                                                       new Pair("The returns ", "The "),
                                                                                   },
                                                                               _ => _.ToArray(__ => __.Key));

        public override string FixableDiagnosticId => "MiKo_2030";

        protected override Task<SyntaxNode> NonGenericCommentAsync(XmlElementSyntax comment, string memberName, TypeSyntax returnType, Document document, CancellationToken cancellationToken)
        {
            return Task.FromResult<SyntaxNode>(GetDefaultComment(comment));
        }

        protected override Task<SyntaxNode> GenericCommentAsync(XmlElementSyntax comment, string memberName, GenericNameSyntax returnType, Document document, CancellationToken cancellationToken)
        {
            return Task.FromResult<SyntaxNode>(GetDefaultComment(comment));
        }

        private static XmlElementSyntax GetDefaultComment(XmlElementSyntax comment)
        {
            var text = comment.GetTextTrimmed();

            if (text.IsNullOrEmpty())
            {
                return CommentStartingWith(comment, "The " + Constants.TODO);
            }

            if (text.StartsWith("If", StringComparison.OrdinalIgnoreCase))
            {
                // we cannot fix that
                return comment;
            }

            var preparedComment = CommentStartingWith(comment, "The ");

            return Comment(preparedComment, ReplacementMap);
        }
    }
}