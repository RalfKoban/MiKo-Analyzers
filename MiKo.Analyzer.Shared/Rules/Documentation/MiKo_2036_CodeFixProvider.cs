using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class MiKo_2036_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2036";

        protected override bool IsApplicable(in ImmutableArray<Diagnostic> issues)
        {
            for (int index = 0, length = issues.Length; index < length; index++)
            {
                var properties = issues[index].Properties;

                if (properties.Count > 0 && properties.ContainsKey(Constants.AnalyzerCodeFixSharedData.IsBoolean))
                {
                    return true;
                }
            }

            return false;
        }

        protected override Task<SyntaxNode> NonGenericCommentAsync(XmlElementSyntax comment, string memberName, TypeSyntax returnType, Document document, CancellationToken cancellationToken)
        {
            return WithDefaultCommentAsync(comment, returnType, document, cancellationToken);
        }

        protected override Task<SyntaxNode> GenericCommentAsync(XmlElementSyntax comment, string memberName, GenericNameSyntax returnType, Document document, CancellationToken cancellationToken)
        {
            return WithDefaultCommentAsync(comment, returnType, document, cancellationToken);
        }

        protected abstract Task<XmlNodeSyntax[]> GetDefaultCommentAsync(TypeSyntax returnType, Document document, CancellationToken cancellationToken);

        private async Task<SyntaxNode> WithDefaultCommentAsync(XmlElementSyntax comment, TypeSyntax returnType, Document document, CancellationToken cancellationToken)
        {
            var texts = await GetDefaultCommentAsync(returnType, document, cancellationToken).ConfigureAwait(false);

            var updatedContent = comment.Content
                                        .AddRange(texts)
                                        .Add(XmlText().WithLeadingXmlComment());

            return comment.WithContent(updatedContent);
        }
    }
}