using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2004_EventHandlerParametersAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2004";

        private const string DefaultEnding = " that contains the event data";

        public MiKo_2004_EventHandlerParametersAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsEventHandler() && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            if (commentXml.Contains(Constants.Comments.XmlElementStartingTag + Constants.XmlTag.Inheritdoc))
            {
                // nothing to report as the documentation is inherited
                return Enumerable.Empty<Diagnostic>();
            }

            return VerifyParameterComments(symbol, commentXml, comment);
        }

        private static string GetDefaultStartingPhrase(string name) => ArticleProvider.GetIndefiniteArticleFor(name);

        private static IEnumerable<string> CreatePhrases(IMethodSymbol method)
        {
            var type = method.Parameters[1].Type;
            var typeName = type.Name;
            var typeString = type.ToString();

            var defaultStart = GetDefaultStartingPhrase(typeName);

            var variantWithSpace = string.Concat(defaultStart, "<see cref=\"", typeString, "\" />" + DefaultEnding);
            var variantWithoutSpace = string.Concat(defaultStart, "<see cref=\"", typeString, "\"/>" + DefaultEnding);

            return new[]
                       {
                           string.Concat(defaultStart, "<see cref=\"", typeName, "\" />" + DefaultEnding + "."), // just used for the proposal
                           variantWithSpace + ".",
                           variantWithSpace,
                           variantWithoutSpace + ".",
                           variantWithoutSpace,
                       };
        }

        private IEnumerable<Diagnostic> VerifyParameterComments(IMethodSymbol method, string xml, DocumentationCommentTriviaSyntax comment)
        {
            var sender = method.Parameters[0];
            var senderComment = comment.GetParameterComment(sender.Name);

            if (senderComment != null)
            {
                var phrase = sender.GetComment(xml);

                if (Constants.Comments.EventSourcePhrase.None(_ => _ == phrase))
                {
                    var proposal = Constants.Comments.EventSourcePhrase[0];

                    yield return Issue(sender.Name, senderComment.GetContentsLocation(), proposal, CreatePhraseProposal(proposal));
                }
            }

            var eventArgs = method.Parameters[1];
            var eventArgsComment = comment.GetParameterComment(eventArgs.Name);

            if (eventArgsComment != null)
            {
                var phrases = CreatePhrases(method).Concat(Constants.Comments.UnusedPhrase).ToList();

                var phrase = eventArgs.GetComment(xml);

                if (phrases.None(_ => _ == phrase))
                {
                    var start = GetDefaultStartingPhrase(eventArgs.Type.Name);

                    yield return Issue(eventArgs.Name, eventArgsComment.GetContentsLocation(), phrases[0], CreateStartingEndingPhraseProposal(start, DefaultEnding));
                }
            }
        }
    }
}