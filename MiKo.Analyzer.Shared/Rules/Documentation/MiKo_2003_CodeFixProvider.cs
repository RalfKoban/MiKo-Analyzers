﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2003_CodeFixProvider)), Shared]
    public sealed class MiKo_2003_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly IReadOnlyCollection<string> ReplacementMapKeys = new[]
                                                                                     {
                                                                                         "Callback that is called by the ",
                                                                                         "Called by ",
                                                                                         "Called by the ",
                                                                                         "Called if ",
                                                                                         "Called if the ",
                                                                                         "Called when ",
                                                                                         "Called when the ",
                                                                                         "Event handler for ",
                                                                                         "Event Handler for ",
                                                                                         "Event handler for the ",
                                                                                         "Event Handler for the ",
                                                                                         "Eventhandler for ",
                                                                                         "EventHandler for ",
                                                                                         "Eventhandler for the ",
                                                                                         "EventHandler for the ",
                                                                                         "Handle the ",
                                                                                         "Handler for ",
                                                                                         "Handler for the ",
                                                                                         "Invoke the ",
                                                                                         "Invoke the event ",
                                                                                         "Invoked by ",
                                                                                         "Invoked by the ",
                                                                                         "Invoked by the event ",
                                                                                         "Invoked when ",
                                                                                         "Invoked when the ",
                                                                                         "Invoked when the event ",
                                                                                         "Raised by ",
                                                                                         "Raised by the ",
                                                                                         "Raised when ",
                                                                                         "Raised when the ",
                                                                                         "when the ",
                                                                                         "When the ",
                                                                                     };

        private static readonly IReadOnlyCollection<KeyValuePair<string, string>> ReplacementMap = ReplacementMapKeys.OrderByDescending(_ => _.Length)
                                                                                                                     .ThenBy(_ => _)
                                                                                                                     .Select(_ => new KeyValuePair<string, string>(_, Constants.Comments.EventHandlerSummaryStartingPhrase))
                                                                                                                     .ToArray();

        public override string FixableDiagnosticId => "MiKo_2003";

        protected override string Title => Resources.MiKo_2003_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;

            return Comment(comment, ReplacementMapKeys, ReplacementMap);
        }
    }
}