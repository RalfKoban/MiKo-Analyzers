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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2242_CodeFixProvider)), Shared]
    public sealed class MiKo_2242_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2242";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<XmlTextSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            return Task.FromResult(syntax);
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntaxRoot(root, syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root, SyntaxNode syntax)
        {
            if (syntax is XmlTextSyntax text)
            {
                return root.ReplaceNode(syntax, GetReplacement(text));
            }

            return root;
        }

        private static XmlTextSyntax GetReplacement(XmlTextSyntax node)
        {
            var textTokens = node.TextTokens.ToList();

            for (var index = 0; index < textTokens.Count; index++)
            {
                var textToken = textTokens[index];

                if (textToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var valueText = textToken.ValueText
                                         .AsCachedBuilder()
                                         .ReplaceWithProbe("string representation", "textual representation")
                                         .ReplaceWithProbe("String representation", "textual representation")
                                         .ToStringAndRelease();

                textTokens[index] = textToken.WithText(valueText);
            }

            return node.WithTextTokens(textTokens.ToTokenList());
        }
    }
}