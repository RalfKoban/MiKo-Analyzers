﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2081_CodeFixProvider)), Shared]
    public sealed class MiKo_2081_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2081_ReadOnlyFieldAnalyzer.Id;

        protected override string Title => Resources.MiKo_2081_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var comment = (XmlElementSyntax)syntax;

            const string Text = Constants.Comments.FieldIsReadOnly;

            return SyntaxFactory.XmlElement(
                                            comment.StartTag,
                                            comment.WithoutText(Text).Add(SyntaxFactory.XmlText(Text)),
                                            comment.EndTag.WithLeadingXmlComment()); // place on new line
        }
    }
}