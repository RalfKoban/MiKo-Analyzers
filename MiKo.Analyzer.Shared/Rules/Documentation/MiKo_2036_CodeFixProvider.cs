using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class MiKo_2036_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2036";

        protected override bool IsApplicable(ImmutableArray<Diagnostic> diagnostics)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < diagnostics.Length; index++)
            {
                var diagnostic = diagnostics[index];
                var properties = diagnostic.Properties;

                if (properties.Count > 0 && properties.ContainsKey(Constants.AnalyzerCodeFixSharedData.IsBoolean))
                {
                    return true;
                }
            }

            return false;
        }

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