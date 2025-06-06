﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2004_CodeFixProvider)), Shared]
    public sealed class MiKo_2004_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2004";

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, in int index, Diagnostic issue)
        {
            if (index is 0)
            {
                // this is the sender
                return Comment(comment, GetPhraseProposal(issue));
            }

            // this is the event args
            var startingPhrase = GetStartingPhraseProposal(issue);
            var endingPhrase = GetEndingPhraseProposal(issue) + ".";

            return Comment(comment, startingPhrase, parameter.Type, endingPhrase);
        }
    }
}