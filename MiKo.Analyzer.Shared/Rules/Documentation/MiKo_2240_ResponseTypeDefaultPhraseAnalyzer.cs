using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2240_ResponseTypeDefaultPhraseAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2240";

        private static readonly string[] WrongStartingPhrases = { "Return", "Is return" };

        private static readonly Pair[] Replacements =
                                                      {
                                                          new Pair("Is returned when "),
                                                          new Pair("Is returned if "),
                                                          new Pair("Returned when "),
                                                          new Pair("Returned if "),
                                                          new Pair("Returned "),
                                                          new Pair("Returns when "),
                                                          new Pair("Returns if "),
                                                          new Pair("Returns "),
                                                          new Pair("Return when "),
                                                          new Pair("Return if "),
                                                          new Pair("Return "),
                                                      };

        public MiKo_2240_ResponseTypeDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var responseXmls = comment.GetXmlSyntax(Constants.XmlTag.Response);
            var responseXmlsCount = responseXmls.Count;

            if (responseXmlsCount > 0)
            {
                for (var index = 0; index < responseXmlsCount; index++)
                {
                    var content = responseXmls[index].Content;

                    if (content.Count > 0 && content[0] is XmlTextSyntax node)
                    {
                        var text = node.GetTextTrimmed();

                        if (text.StartsWithAny(WrongStartingPhrases, StringComparison.Ordinal))
                        {
                            var betterText = FindBetterText(text);

                            return new[] { Issue(node, betterText.AsSpan().HumanizedTakeFirst(50), CreatePhraseProposal(betterText)) };
                        }
                    }
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private static string FindBetterText(string text)
        {
            var betterText = text.AsCachedBuilder()
                                 .ReplaceAllWithProbe(Replacements)
                                 .AdjustFirstWord(FirstWordAdjustment.StartUpperCase)
                                 .ToStringAndRelease();

            return betterText;
        }
    }
}