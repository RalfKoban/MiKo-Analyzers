using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2004_CodeFixProvider)), Shared]
    public sealed class MiKo_2004_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2004_EventHandlerParametersAnalyzer.Id;

        protected override string Title => "Fix comment of event handler parameter";

        protected override XmlElementSyntax Comment(XmlElementSyntax comment, ParameterSyntax parameter, int index)
        {
            if (index == 0)
            {
                // this is the sender
                return Comment(comment, Constants.Comments.EventSourcePhrase);
            }

            // this is the event args
            var name = parameter.Type.GetNameOnlyPart();

            return Comment(comment, name.StartsWithAnyChar("AEIOU") ? "An " : "A ", parameter.Type, " that contains the event data.");
        }
    }
}