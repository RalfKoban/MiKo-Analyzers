using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2003_CodeFixProvider)), Shared]
    public sealed class MiKo_2003_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> ReplacementMap = new Dictionary<string, string>
                                                                                {
                                                                                    { "Called by the ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                    { "Called if the ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                    { "Called if ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                    { "Called when the ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                    { "Called when ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                    { "Callback that is called by the ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                    { "EventHandler for the ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                    { "EventHandler for ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                    { "Event handler for the ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                    { "Event handler for ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                    { "Handler for the ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                    { "Handler for ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                    { "Handle the ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                    { "Raised when the ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                    { "when the ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                    { "When the ", Constants.Comments.EventHandlerSummaryStartingPhrase },
                                                                                };

        public override string FixableDiagnosticId => MiKo_2003_EventHandlerSummaryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2003_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;

            return Comment(comment, ReplacementMap.Keys, ReplacementMap);
        }
    }
}