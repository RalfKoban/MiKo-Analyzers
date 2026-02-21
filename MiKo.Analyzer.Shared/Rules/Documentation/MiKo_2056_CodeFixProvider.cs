using System;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2056_CodeFixProvider)), Shared]
    public sealed class MiKo_2056_CodeFixProvider : ExceptionDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2056";

        protected override Task<DocumentationCommentTriviaSyntax> GetUpdatedSyntaxAsync(DocumentationCommentTriviaSyntax syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        protected override DocumentationCommentTriviaSyntax FixExceptionComment(SyntaxNode syntax, XmlElementSyntax exception, DocumentationCommentTriviaSyntax comment, Diagnostic issue)
        {
            if (exception.IsExceptionCommentFor<ObjectDisposedException>())
            {
                var phrase = GetPhraseProposal(issue);

                var exceptionComment = CommentEndingWith(exception, phrase);

                return comment.ReplaceNode(exception, exceptionComment);
            }

            return null;
        }

        private DocumentationCommentTriviaSyntax GetUpdatedSyntax(DocumentationCommentTriviaSyntax syntax, Diagnostic issue)
        {
            foreach (var ancestor in syntax.AncestorsAndSelf())
            {
                switch (ancestor)
                {
                    case ConstructorDeclarationSyntax _:
                    case PropertyDeclarationSyntax _:
                    case MethodDeclarationSyntax _:
                    {
                        return FixComment(ancestor, syntax, issue);
                    }
                }
            }

            return null;
        }
    }
}