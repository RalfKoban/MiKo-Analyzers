using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2218_CodeFixProvider)), Shared]
    public sealed class MiKo_2218_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2218";

        protected override string Title => Resources.MiKo_2218_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var location = diagnostic.Location;
            var properties = diagnostic.Properties;
            var textToReplace = properties[Constants.AnalyzerCodeFixSharedData.TextKey];
            var textToReplaceWith = properties[Constants.AnalyzerCodeFixSharedData.TextReplacementKey];

            var affectedNodes = syntax.DescendantNodes<XmlTextSyntax>(_ => _.GetLocation().Contains(location));

            return syntax.ReplaceNodes(affectedNodes, (_, rewritten) => GetBetterText(rewritten, textToReplace, textToReplaceWith));
        }

        private static XmlTextSyntax GetBetterText(XmlTextSyntax node, string textToReplace, string textToReplaceWith)
        {
            var tokens = node.TextTokens.OfKind(SyntaxKind.XmlTextLiteralToken);

            var tokensToReplace = new Dictionary<SyntaxToken, SyntaxToken>();

            foreach (var token in tokens)
            {
                var text = token.Text;

                if (text.Length <= Constants.EnvironmentNewLine.Length && text.IsNullOrWhiteSpace())
                {
                    // do not bother with only empty text
                    continue;
                }

                tokensToReplace[token] = token.WithText(text.Replace(textToReplace, textToReplaceWith));
            }

            if (tokensToReplace.Any())
            {
                return node.ReplaceTokens(tokensToReplace.Keys, (original, rewritten) => tokensToReplace[original]);
            }

            return node;
        }
    }
}