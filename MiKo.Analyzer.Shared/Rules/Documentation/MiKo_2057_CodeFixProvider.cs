using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2057_CodeFixProvider)), Shared]
    public sealed class MiKo_2057_CodeFixProvider : ExceptionDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2057";

        protected override Task<DocumentationCommentTriviaSyntax> GetUpdatedSyntaxAsync(DocumentationCommentTriviaSyntax syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static DocumentationCommentTriviaSyntax GetUpdatedSyntax(DocumentationCommentTriviaSyntax syntax)
        {
            var children = syntax.ChildNodes().ToList();

            var nodesToRemove = new List<SyntaxNode>();

            foreach (var exceptionXml in syntax.GetExceptionXmls().Where(_ => _.IsExceptionCommentFor<ObjectDisposedException>()))
            {
                var index = children.IndexOf(exceptionXml);

                nodesToRemove.Add(exceptionXml);

                if (index > 0)
                {
                    var previousChild = children[index - 1];

                    if (previousChild.IsWhiteSpaceOnlyText())
                    {
                        nodesToRemove.Add(previousChild);
                    }
                }
            }

            return syntax.Without(nodesToRemove);
        }
    }
}