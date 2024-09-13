using System;
using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2049_CodeFixProvider)), Shared]
    public sealed class MiKo_2049_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private const string IsPhrase = "is";
        private const string ArePhrase = "are";
        private const string DoPhrase = "do";
        private const string DoesPhrase = "does";

        public override string FixableDiagnosticId => "MiKo_2049";

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
            var tokensToReplace = new Dictionary<SyntaxToken, SyntaxToken>();

            var textTokens = node.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            for (var index = 0; index < textTokensCount; index++)
            {
                var token = textTokens[index];

                if (token.IsKind(SyntaxKind.XmlTextLiteralToken))
                {
                    var text = token.Text;

                    if (text.Length <= Constants.EnvironmentNewLine.Length && text.IsNullOrWhiteSpace())
                    {
                        // do not bother with only empty text
                        continue;
                    }

                    var start = text.IndexOf(textToReplace, StringComparison.Ordinal);

                    if (start < 0)
                    {
                        // does not seem to fit
                        continue;
                    }

                    var startingPart = text.AsSpan(0, start);
                    var lastWord = startingPart.LastWord();

                    // let's see if we have to deal with 'does' or 'is' but need to have plural
                    if (Verbalizer.IsPlural(lastWord))
                    {
                        if (textToReplaceWith.StartsWith(IsPhrase, StringComparison.Ordinal))
                        {
                            textToReplaceWith = ArePhrase.ConcatenatedWith(textToReplaceWith.AsSpan(2));
                        }
                        else if (textToReplaceWith.StartsWith(DoesPhrase, StringComparison.Ordinal))
                        {
                            textToReplaceWith = DoPhrase.ConcatenatedWith(textToReplaceWith.AsSpan(4));
                        }
                    }

                    tokensToReplace[token] = token.WithText(text.Replace(textToReplace, textToReplaceWith));
                }
            }

            if (tokensToReplace.Count > 0)
            {
                return node.ReplaceTokens(tokensToReplace.Keys, (original, rewritten) => tokensToReplace[rewritten]);
            }

            return node;
        }
    }
}