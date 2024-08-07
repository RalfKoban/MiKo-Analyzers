using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2057_CodeFixProvider)), Shared]
    public sealed class MiKo_2057_CodeFixProvider : ExceptionDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2057";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
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