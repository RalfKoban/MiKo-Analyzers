using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class MiKo_2036_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        public sealed override string FixableDiagnosticId => MiKo_2036_PropertyDefaultValuePhraseAnalyzer.Id;

        protected override bool IsApplicable(IEnumerable<Diagnostic> diagnostics) => diagnostics.Any(MiKo_2036_PropertyDefaultValuePhraseAnalyzer.IsBooleanIssue);

        protected override XmlElementSyntax NonGenericComment(CodeFixContext context, XmlElementSyntax comment, TypeSyntax returnType) => WithDefaultComment(context, comment, returnType);

        protected override XmlElementSyntax GenericComment(CodeFixContext context, XmlElementSyntax comment, GenericNameSyntax returnType) => WithDefaultComment(context, comment, returnType);

        protected abstract IEnumerable<XmlNodeSyntax> GetDefaultComment(CodeFixContext context, TypeSyntax returnType);

        private XmlElementSyntax WithDefaultComment(CodeFixContext context, XmlElementSyntax comment, TypeSyntax returnType)
        {
            var texts = GetDefaultComment(context, returnType);

            var newContent = comment.Content
                                    .AddRange(texts)
                                    .Add(XmlText(string.Empty).WithLeadingXmlComment());

            return comment.WithContent(newContent);
        }
    }
}