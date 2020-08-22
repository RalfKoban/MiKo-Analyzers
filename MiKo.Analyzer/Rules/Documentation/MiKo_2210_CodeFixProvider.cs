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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2210_CodeFixProvider)), Shared]
    public sealed class MiKo_2210_CodeFixProvider : DocumentationCodeFixProvider
    {
        private const string Replacement = "information";

        private static readonly Dictionary<string, string> Terms = MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer.Terms.ToDictionary(_ => _, _ => _.Replace(MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer.Term, Replacement));

        public override string FixableDiagnosticId => MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer.Id;

        protected override string Title => "Change '" + MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer.Term + "' into '" + Replacement + "'";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var result = (DocumentationCommentTriviaSyntax)syntax;

            foreach (var text in syntax.DescendantNodes().OfType<XmlTextSyntax>())
            {
                var newText = text;

                // replace token in text
                foreach (var token in text.TextTokens)
                {
                    var originalText = token.Text;

                    if (originalText.ContainsAny(Terms.Keys))
                    {
                        var replacedText = originalText;

                        foreach (var term in Terms)
                        {
                            replacedText = replacedText.Replace(term.Key, term.Value)
                                                       .Replace(term.Key.Replace('i', 'I'), term.Value);
                        }

                        var newToken = SyntaxFactory.Token(token.LeadingTrivia, token.Kind(), replacedText, replacedText, token.TrailingTrivia);

                        newText = newText.ReplaceToken(token, newToken);
                    }
                }

                result = result.ReplaceNode(text, newText);
            }

            return result;
        }
    }
}