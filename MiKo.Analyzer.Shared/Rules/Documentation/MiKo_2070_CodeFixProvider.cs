using System;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2070_CodeFixProvider)), Shared]
    public sealed class MiKo_2070_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2070_ReturnsSummaryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2070_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var summary = (XmlElementSyntax)syntax;

            if (summary.Content.Count > 0 && CommentCanBeFixed(summary))
            {
                summary = AdjustBeginning(summary);
                summary = AdjustMiddle(summary);
                summary = AdjustEnding(summary);
            }

            return summary;
        }

        // introduced as workaround for issue #399
        private static bool CommentCanBeFixed(SyntaxNode syntax)
        {
            var comment = syntax.ToString();

            var falseIndex = comment.IndexOf("false", StringComparison.OrdinalIgnoreCase);

            if (falseIndex == -1)
            {
                return true;
            }

            var trueIndex = comment.IndexOf("true", StringComparison.OrdinalIgnoreCase);

            if (trueIndex == -1)
            {
                // cannot fix currently (false case comes as only case)
                if (comment.IndexOf("otherwise", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    return false;
                }
            }
            else
            {
                if (falseIndex < trueIndex)
                {
                    // cannot fix currently (false case comes before true case)
                    return false;
                }
            }

            return true;
        }

        private static XmlElementSyntax AdjustBeginning(XmlElementSyntax summary)
        {
            var contents = summary.Content;

            if (contents[0] is XmlTextSyntax text)
            {
                foreach (var token in text.TextTokens)
                {
                    var valueText = token.WithoutTrivia().ValueText.Without(Constants.Comments.AsynchrounouslyStartingPhrase).AsSpan().Trim();

                    if (valueText.StartsWithAny(MiKo_2070_ReturnsSummaryAnalyzer.Phrases))
                    {
                        var startText = GetCorrectStartText(summary);
                        var remainingText = valueText.WithoutFirstWord().WithoutFirstWords("true", "if", "whether").ToString();

                        var newText = " " + startText + " " + remainingText;

                        if (contents.Count > 1 && contents[1].IsKind(SyntaxKind.XmlText) is false)
                        {
                            // we have another non-text, so add a space
                            newText = newText.TrimEnd() + " ";
                        }

                        return summary.ReplaceToken(token, token.WithText(newText));
                    }
                }
            }

            return summary;
        }

        private static XmlElementSyntax AdjustMiddle(XmlElementSyntax summary)
        {
            SyntaxList<XmlNodeSyntax> contents;
            contents = summary.Content;

            if (contents.Count > 1)
            {
                // we might have some '<see langword="xyz"/>' in the summary
                var element = contents[1];

                if (element.IsBooleanTag())
                {
                    // remove the '<see langword="true"/>'
                    summary = summary.Without(element);

                    // remove follow up contents ' if ' or ' whether '
                    if (summary.Content.Count > 1 && summary.Content[1] is XmlTextSyntax followUpText)
                    {
                        foreach (var token in followUpText.TextTokens)
                        {
                            var valueText = token.WithoutTrivia().ValueText;
                            var newText = valueText.WithoutFirstWords("if", "whether");

                            summary = summary.ReplaceToken(token, token.WithText(newText));

                            break;
                        }
                    }
                }

                // combine texts that have been created due to the removal of the '<see langword="xyz"/>'
                contents = summary.Content;

                for (var i = 0; i < contents.Count - 1; i++)
                {
                    if (contents[i] is XmlTextSyntax t1 && contents[i + 1] is XmlTextSyntax t2)
                    {
                        summary = summary.ReplaceNodes(new[] { t1, t2 }, (original, rewritten) =>
                                                                             {
                                                                                 if (original == t1)
                                                                                 {
                                                                                     var tokens = t1.TextTokens.Concat(t2.TextTokens).ToList();

                                                                                     for (var index = 0; index < tokens.Count - 1; index++)
                                                                                     {
                                                                                         var token1 = tokens[index];
                                                                                         var token2 = tokens[index + 1];

                                                                                         if (token1.IsKind(SyntaxKind.XmlTextLiteralToken) && token2.IsKind(SyntaxKind.XmlTextLiteralToken))
                                                                                         {
                                                                                             var combinedText = token1.ValueText + token2.ValueText;
                                                                                             tokens[index] = token1.WithText(combinedText);
                                                                                             tokens.RemoveAt(index + 1);
                                                                                             index--;
                                                                                         }
                                                                                     }

                                                                                     return XmlText(tokens.ToArray());
                                                                                 }

                                                                                 return default;
                                                                             });
                    }
                }
            }

            return summary;
        }

        private static XmlElementSyntax AdjustEnding(XmlElementSyntax summary)
        {
            var contents = summary.Content;

            // remove last node if it is ending with a dot
            if (contents.LastOrDefault() is XmlTextSyntax sentenceEnding)
            {
                foreach (var token in sentenceEnding.TextTokens)
                {
                    if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                    {
                        continue;
                    }

                    var valueText = token.WithoutTrivia().ValueText;

                    if (valueText.IsNullOrWhiteSpace())
                    {
                        continue;
                    }

                    var newText = new StringBuilder(valueText).Without("otherwise").Without("false").Replace("; , .", ".");

                    if (valueText.Length > newText.Length)
                    {
                        foreach (var marker in Constants.TrailingSentenceMarkers)
                        {
                            newText = newText.Replace($"{marker}.", ".")
                                             .Replace($"{marker} .", ".");
                        }

                        summary = summary.ReplaceToken(token, token.WithText(newText.ToString()));
                    }
                }
            }

            return summary;
        }

        private static string GetCorrectStartText(XmlElementSyntax element)
        {
            foreach (var ancestor in element.AncestorsAndSelf())
            {
                switch (ancestor)
                {
                    case PropertyDeclarationSyntax p:
                        return GetCorrectStartText(p);

                    case MethodDeclarationSyntax m:
                        return GetCorrectStartText(m);
                }
            }

            return string.Empty;
        }

        private static string GetCorrectStartText(BasePropertyDeclarationSyntax property)
        {
            var isBool = property.Type.IsBoolean();
            var isAsync = property.IsAsync();

            if (property.Type is GenericNameSyntax g && g.Identifier.ValueText == nameof(Task))
            {
                isAsync = true;
                isBool = g.TypeArgumentList.Arguments.Count > 0 && g.TypeArgumentList.Arguments[0].IsBoolean();
            }

            var startText = isBool ? "Gets a value indicating whether" : "Gets";

            if (isAsync)
            {
                return Constants.Comments.AsynchrounouslyStartingPhrase + startText.ToLowerCaseAt(0);
            }

            return startText;
        }

        private static string GetCorrectStartText(MethodDeclarationSyntax method)
        {
            var isBool = method.ReturnType.IsBoolean();
            var isAsync = method.IsAsync();

            if (method.ReturnType is GenericNameSyntax g && g.Identifier.ValueText == nameof(Task))
            {
                isAsync = true;
                isBool = g.TypeArgumentList.Arguments.Count > 0 && g.TypeArgumentList.Arguments[0].IsBoolean();
            }

            var startText = isBool ? Constants.Comments.DeterminesWhetherPhrase : "Gets";

            if (isAsync)
            {
                return Constants.Comments.AsynchrounouslyStartingPhrase + startText.ToLowerCaseAt(0);
            }

            return startText;
        }
    }
}