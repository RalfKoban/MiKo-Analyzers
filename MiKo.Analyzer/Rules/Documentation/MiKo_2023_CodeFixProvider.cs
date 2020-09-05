using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2023_CodeFixProvider)), Shared]
    public sealed class MiKo_2023_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2023_BooleanParamDefaultPhraseAnalyzer.Id;

        protected override string Title => "Fix comment start of Boolean parameter";

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index)
        {
            // TODO: Fix <see langword>
            var startPhraseParts = string.Format(Constants.Comments.BooleanParameterStartingPhraseTemplate, '|').Split('|');
            var endPhraseParts = string.Format(Constants.Comments.BooleanParameterEndingPhraseTemplate, '|').Split('|');

            var startFixed = CommentStartingWith(comment, startPhraseParts[0], SeeLangword_True(), startPhraseParts[1]);
            var bothFixed = CommentEndingWith(startFixed, endPhraseParts[0], SeeLangword_False(), endPhraseParts[1]);
            return bothFixed;
        }
    }
}