using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class MiKo_2036_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        public sealed override string FixableDiagnosticId => MiKo_2036_PropertyDefaultValuePhraseAnalyzer.Id;

        protected override XmlElementSyntax NonGenericComment(XmlElementSyntax comment, TypeSyntax returnType) => WithDefaultComment(comment);

        protected override XmlElementSyntax GenericComment(XmlElementSyntax comment, GenericNameSyntax returnType) => WithDefaultComment(comment);

        protected abstract IEnumerable<XmlNodeSyntax> GetDefaultComment();

        private XmlElementSyntax WithDefaultComment(XmlElementSyntax comment) => comment.WithContent(comment.Content.AddRange(GetDefaultComment()));
    }
}