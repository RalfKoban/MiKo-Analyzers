using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2004_CodeFixProvider)), Shared]
    public sealed class MiKo_2004_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2004";

        protected override string Title => Resources.MiKo_2004_CodeFixTitle;

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index, Diagnostic issue)
        {
            if (index == 0)
            {
                // this is the sender
                return Comment(comment, Constants.Comments.EventSourcePhrase);
            }

            // this is the event args
            var startingPhrase = MiKo_2004_EventHandlerParametersAnalyzer.GetEventArgsStartingPhrase(parameter.Type.GetNameOnlyPart());
            var endingPhrase = MiKo_2004_EventHandlerParametersAnalyzer.GetEventArgsEndingPhrase() + ".";

            return Comment(comment, startingPhrase, parameter.Type, endingPhrase);
        }
    }
}