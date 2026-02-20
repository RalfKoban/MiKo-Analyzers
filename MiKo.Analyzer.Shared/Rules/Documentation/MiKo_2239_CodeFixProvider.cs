using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2239_CodeFixProvider)), Shared]
    public sealed class MiKo_2239_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2239";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes, SyntaxKind.MultiLineDocumentationCommentTrivia);

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = GetUpdatedSyntax((DocumentationCommentTriviaSyntax)syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static DocumentationCommentTriviaSyntax GetUpdatedSyntax(DocumentationCommentTriviaSyntax comment)
        {
            var texts = comment.Content.OfType<XmlTextSyntax>()
                               .Select(CleanupText)
                               .ToSyntaxList<XmlNodeSyntax>();

            var summary = Comment(SyntaxFactory.XmlSummaryElement(), texts).WithLeadingXmlCommentExterior();

            return SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, summary.ToSyntaxList<XmlNodeSyntax>());
        }

        private static XmlTextSyntax CleanupText(XmlTextSyntax text) => XmlText(CleanupTextTokens(text.TextTokens));

        private static IEnumerable<SyntaxToken> CleanupTextTokens(SyntaxTokenList textTokens)
        {
            for (int index = 0, last = textTokens.Count - 1; index <= last; index++)
            {
                var textToken = textTokens[index];

                if (textToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    if (index > 0 && index < last)
                    {
                        yield return textToken.WithoutTrivia();
                    }
                }
                else
                {
                    var token = textToken.WithoutTrivia()
                                         .WithText(textToken.ValueText.Trim());

                    if (index > 1)
                    {
                        yield return token.WithLeadingXmlCommentExterior();
                    }
                    else
                    {
                        yield return token;
                    }
                }
            }
        }
    }
}