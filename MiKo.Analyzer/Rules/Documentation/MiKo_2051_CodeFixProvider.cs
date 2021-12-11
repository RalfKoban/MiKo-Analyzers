using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2051_CodeFixProvider)), Shared]
    public sealed class MiKo_2051_CodeFixProvider : ExceptionDocumentationCodeFixProvider
    {
        // TODO RKN: see Constants.Comments.ExceptionForbiddenStartingPhrase
        private static readonly string[] Phrases =
            {
                "Thrown if the ",
                "Thrown if ",
                "Thrown when the ",
                "Thrown when ",
                "Throws if the ",
                "Throws if ",
                "Throws when the ",
                "Throws when ",
                "Is thrown when the ",
                "Is thrown when ",
                "Gets thrown when the ",
                "Gets thrown when ",
                "If the ",
                "In case the ",
                "In case ",
                "if the ",
                "in case the ",
                "in case ",
            };

        public override string FixableDiagnosticId => MiKo_2051_ExceptionTagDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2051_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var exceptionComments = GetExceptionXmls(syntax);

            foreach (var exceptionComment in exceptionComments)
            {
                if (exceptionComment.Content.First() is XmlTextSyntax text)
                {
                    // TODO: RKN Put this into a lookup
                    return ReplaceText(syntax, text, Phrases, string.Empty);
                }
            }

            return syntax;
        }
    }
}