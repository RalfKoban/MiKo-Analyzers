﻿using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2056_CodeFixProvider)), Shared]
    public sealed class MiKo_2056_CodeFixProvider : ExceptionDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2056";

        protected override string Title => Resources.MiKo_2056_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            foreach (var ancestor in syntax.AncestorsAndSelf())
            {
                switch (ancestor)
                {
                    case ConstructorDeclarationSyntax _:
                    case PropertyDeclarationSyntax _:
                    case MethodDeclarationSyntax _:
                    {
                        return FixComment(document, ancestor, syntax);
                    }
                }
            }

            return null;
        }

        protected override DocumentationCommentTriviaSyntax FixExceptionComment(Document document, SyntaxNode syntax, XmlElementSyntax exception, DocumentationCommentTriviaSyntax comment)
        {
            if (exception.IsExceptionCommentFor<ObjectDisposedException>())
            {
                var symbol = GetSymbol(document, syntax);
                var phrase = MiKo_2056_ObjectDisposedExceptionPhraseAnalyzer.GetEndingPhrase(symbol);

                var exceptionComment = CommentEndingWith(exception, phrase);

                return comment.ReplaceNode(exception, exceptionComment);
            }

            return null;
        }
    }
}