﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class MiKo_2036_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        public sealed override string FixableDiagnosticId => MiKo_2036_PropertyDefaultValuePhraseAnalyzer.Id;

        protected override bool IsApplicable(ImmutableArray<Diagnostic> diagnostics) => diagnostics.Any(MiKo_2036_PropertyDefaultValuePhraseAnalyzer.IsBooleanIssue);

        protected override XmlElementSyntax NonGenericComment(Document document, XmlElementSyntax comment, string memberName, TypeSyntax returnType) => WithDefaultComment(document, comment, returnType);

        protected override XmlElementSyntax GenericComment(Document document, XmlElementSyntax comment, string memberName, GenericNameSyntax returnType) => WithDefaultComment(document, comment, returnType);

        protected abstract IEnumerable<XmlNodeSyntax> GetDefaultComment(Document document, TypeSyntax returnType);

        private XmlElementSyntax WithDefaultComment(Document document, XmlElementSyntax comment, TypeSyntax returnType)
        {
            var texts = GetDefaultComment(document, returnType);

            var newContent = comment.Content
                                    .AddRange(texts)
                                    .Add(XmlText(string.Empty).WithLeadingXmlComment());

            return comment.WithContent(newContent);
        }
    }
}